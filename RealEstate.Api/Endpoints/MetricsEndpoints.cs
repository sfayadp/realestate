using RealEstate.Api.Services;

namespace RealEstate.Api.Endpoints
{
    /// <summary>
    /// Endpoints para métricas y observabilidad de la API
    /// </summary>
    public static class MetricsEndpoints
    {
        public static void MapMetricsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/metrics")
                .WithTags("Metrics");

            /// <summary>
            /// Obtiene métricas detalladas de performance y uso de la API
            /// </summary>
            /// <returns>Métricas del sistema</returns>
            /// <response code="200">Métricas obtenidas exitosamente</response>
            group.MapGet("/", (IMetricsService metricsService) =>
            {
                var metrics = metricsService.GetMetrics();
                return Results.Ok(metrics);
            })
            .WithName("GetMetrics");

            /// <summary>
            /// Obtiene métricas de cache (hits, misses, hit rate)
            /// </summary>
            /// <returns>Métricas de cache</returns>
            /// <response code="200">Métricas de cache obtenidas</response>
            group.MapGet("/cache", (IMetricsService metricsService) =>
            {
                var metrics = metricsService.GetMetrics();
                var cacheMetrics = new
                {
                    hits = metrics.GetValueOrDefault("cache_hits", new Dictionary<string, long>()),
                    misses = metrics.GetValueOrDefault("cache_misses", new Dictionary<string, long>()),
                    hitRate = metrics.GetValueOrDefault("cache_hit_rate_percent", 0),
                    timestamp = metrics.GetValueOrDefault("timestamp", DateTime.UtcNow)
                };
                
                return Results.Ok(cacheMetrics);
            })
            .WithName("GetCacheMetrics");

            /// <summary>
            /// Obtiene métricas de requests (counts, durations)
            /// </summary>
            /// <returns>Métricas de requests</returns>
            /// <response code="200">Métricas de requests obtenidas</response>
            group.MapGet("/requests", (IMetricsService metricsService) =>
            {
                var metrics = metricsService.GetMetrics();
                var requestMetrics = new
                {
                    totalRequests = metrics.GetValueOrDefault("total_requests", 0),
                    requestCounts = metrics.GetValueOrDefault("request_counts", new Dictionary<string, long>()),
                    averageDurations = metrics.GetValueOrDefault("average_request_durations", new Dictionary<string, long>()),
                    timestamp = metrics.GetValueOrDefault("timestamp", DateTime.UtcNow)
                };
                
                return Results.Ok(requestMetrics);
            })
            .WithName("GetRequestMetrics");
        }
    }
}
