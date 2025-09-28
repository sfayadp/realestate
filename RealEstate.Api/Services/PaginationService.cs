using Microsoft.EntityFrameworkCore;
using RealEstate.Shared.DTO;

namespace RealEstate.Api.Services
{
    /// <summary>
    /// Servicio para optimizar consultas de paginación y filtrado
    /// </summary>
    public interface IPaginationService
    {
        Task<PagedResult<T>> GetPagedResultAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResult<T>> GetPagedResultWithCacheAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize,
            string cacheKey,
            TimeSpan? cacheExpiration = null,
            CancellationToken cancellationToken = default);
    }

    public class PaginationService : IPaginationService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PaginationService> _logger;

        public PaginationService(ICacheService cacheService, ILogger<PaginationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PagedResult<T>> GetPagedResultAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validar parámetros
                page = Math.Max(1, page);
                pageSize = Math.Max(1, Math.Min(pageSize, 100)); // Máximo 100 items por página

                // Calcular offset
                var skip = (page - 1) * pageSize;

                // Ejecutar consultas en paralelo para mejor performance
                var countTask = query.CountAsync(cancellationToken);
                var itemsTask = query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

                await Task.WhenAll(countTask, itemsTask);

                var total = await countTask;
                var items = await itemsTask;

                var result = new PagedResult<T>
                {
                    Items = items,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };

                _logger.LogDebug("Pagination completed. Page: {Page}, PageSize: {PageSize}, Total: {Total}", 
                    page, pageSize, total);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in pagination. Page: {Page}, PageSize: {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<PagedResult<T>> GetPagedResultWithCacheAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize,
            string cacheKey,
            TimeSpan? cacheExpiration = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Generar clave de cache única para esta consulta
                var fullCacheKey = $"{cacheKey}_page_{page}_size_{pageSize}";

                // Intentar obtener del cache primero
                var cachedResult = await _cacheService.GetAsync<PagedResult<T>>(fullCacheKey);
                if (cachedResult != null)
                {
                    _logger.LogDebug("Cache hit for pagination. Key: {Key}", fullCacheKey);
                    return cachedResult;
                }

                // Si no está en cache, ejecutar consulta
                var result = await GetPagedResultAsync(query, page, pageSize, cancellationToken);

                // Guardar en cache
                await _cacheService.SetAsync(fullCacheKey, result, cacheExpiration);

                _logger.LogDebug("Cache miss for pagination. Key: {Key}, Cached result", fullCacheKey);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cached pagination. Page: {Page}, PageSize: {PageSize}, CacheKey: {CacheKey}", 
                    page, pageSize, cacheKey);
                throw;
            }
        }
    }
}
