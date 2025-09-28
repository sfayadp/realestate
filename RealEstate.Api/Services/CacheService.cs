using Microsoft.Extensions.Caching.Memory;

namespace RealEstate.Api.Services
{
    /// <summary>
    /// Servicio de cache personalizado para optimizar consultas frecuentes
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var result = _memoryCache.Get<T>(key);
                _logger.LogDebug("Cache {Action} for key: {Key}, Found: {Found}", "GET", key, result != null);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(15),
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, value, options);
                _logger.LogDebug("Cache {Action} for key: {Key}, Expiration: {Expiration}", "SET", key, expiration);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Cache {Action} for key: {Key}", "REMOVE", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            // Nota: IMemoryCache no soporta pattern matching nativo
            // En un escenario real, usaríamos Redis o implementaríamos un tracking de keys
            _logger.LogWarning("Pattern-based cache removal not supported with IMemoryCache. Pattern: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                _logger.LogDebug("Cache {Action} for key: {Key}, Exists: {Exists}", "EXISTS", key, exists);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return Task.FromResult(false);
            }
        }
    }
}
