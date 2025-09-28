using System.Diagnostics;

namespace RealEstate.Api.Services
{
    /// <summary>
    /// Servicio para recopilar métricas de performance y uso de la API
    /// </summary>
    public interface IMetricsService
    {
        void RecordRequestDuration(string endpoint, string method, long durationMs);
        void RecordRequestCount(string endpoint, string method, int statusCode);
        void RecordCacheHit(string cacheKey);
        void RecordCacheMiss(string cacheKey);
        void RecordDatabaseQuery(string queryType, long durationMs);
        Dictionary<string, object> GetMetrics();
    }

    public class MetricsService : IMetricsService
    {
        private readonly ILogger<MetricsService> _logger;
        private readonly Dictionary<string, long> _requestCounts = new();
        private readonly Dictionary<string, long> _requestDurations = new();
        private readonly Dictionary<string, long> _cacheHits = new();
        private readonly Dictionary<string, long> _cacheMisses = new();
        private readonly Dictionary<string, long> _databaseQueries = new();
        private readonly object _lock = new();

        public MetricsService(ILogger<MetricsService> logger)
        {
            _logger = logger;
        }

        public void RecordRequestDuration(string endpoint, string method, long durationMs)
        {
            lock (_lock)
            {
                var key = $"{method}_{endpoint}";
                if (_requestDurations.ContainsKey(key))
                {
                    _requestDurations[key] = (_requestDurations[key] + durationMs) / 2; // Promedio móvil
                }
                else
                {
                    _requestDurations[key] = durationMs;
                }
            }

            _logger.LogDebug("Request duration recorded. Endpoint: {Endpoint}, Method: {Method}, Duration: {Duration}ms", 
                endpoint, method, durationMs);
        }

        public void RecordRequestCount(string endpoint, string method, int statusCode)
        {
            lock (_lock)
            {
                var key = $"{method}_{endpoint}_{statusCode}";
                _requestCounts[key] = _requestCounts.GetValueOrDefault(key, 0) + 1;
            }

            _logger.LogDebug("Request count recorded. Endpoint: {Endpoint}, Method: {Method}, StatusCode: {StatusCode}", 
                endpoint, method, statusCode);
        }

        public void RecordCacheHit(string cacheKey)
        {
            lock (_lock)
            {
                _cacheHits[cacheKey] = _cacheHits.GetValueOrDefault(cacheKey, 0) + 1;
            }

            _logger.LogDebug("Cache hit recorded. Key: {Key}", cacheKey);
        }

        public void RecordCacheMiss(string cacheKey)
        {
            lock (_lock)
            {
                _cacheMisses[cacheKey] = _cacheMisses.GetValueOrDefault(cacheKey, 0) + 1;
            }

            _logger.LogDebug("Cache miss recorded. Key: {Key}", cacheKey);
        }

        public void RecordDatabaseQuery(string queryType, long durationMs)
        {
            lock (_lock)
            {
                var key = $"{queryType}";
                if (_databaseQueries.ContainsKey(key))
                {
                    _databaseQueries[key] = (_databaseQueries[key] + durationMs) / 2; // Promedio móvil
                }
                else
                {
                    _databaseQueries[key] = durationMs;
                }
            }

            _logger.LogDebug("Database query recorded. Type: {Type}, Duration: {Duration}ms", queryType, durationMs);
        }

        public Dictionary<string, object> GetMetrics()
        {
            lock (_lock)
            {
                var totalRequests = _requestCounts.Values.Sum();
                var totalCacheHits = _cacheHits.Values.Sum();
                var totalCacheMisses = _cacheMisses.Values.Sum();
                var cacheHitRate = totalCacheHits + totalCacheMisses > 0 
                    ? (double)totalCacheHits / (totalCacheHits + totalCacheMisses) * 100 
                    : 0;

                return new Dictionary<string, object>
                {
                    ["timestamp"] = DateTime.UtcNow,
                    ["total_requests"] = totalRequests,
                    ["request_counts"] = new Dictionary<string, long>(_requestCounts),
                    ["average_request_durations"] = new Dictionary<string, long>(_requestDurations),
                    ["cache_hits"] = new Dictionary<string, long>(_cacheHits),
                    ["cache_misses"] = new Dictionary<string, long>(_cacheMisses),
                    ["cache_hit_rate_percent"] = Math.Round(cacheHitRate, 2),
                    ["database_query_durations"] = new Dictionary<string, long>(_databaseQueries),
                    ["memory_usage_mb"] = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024
                };
            }
        }
    }
}
