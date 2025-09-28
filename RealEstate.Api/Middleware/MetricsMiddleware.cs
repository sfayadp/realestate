using RealEstate.Api.Services;
using System.Diagnostics;

namespace RealEstate.Api.Middleware
{
    /// <summary>
    /// Middleware para registrar automáticamente métricas de requests
    /// </summary>
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MetricsMiddleware> _logger;

        public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IMetricsService metricsService)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            
            // Agregar correlation ID si no existe
            if (!context.Items.ContainsKey("CorrelationId"))
            {
                context.Items["CorrelationId"] = correlationId;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Registrar métricas del request
                var endpoint = GetEndpointName(context);
                var method = context.Request.Method;
                var statusCode = context.Response.StatusCode;
                var duration = stopwatch.ElapsedMilliseconds;

                // Registrar métricas
                metricsService.RecordRequestCount(endpoint, method, statusCode);
                metricsService.RecordRequestDuration(endpoint, method, duration);

                // Log estructurado
                _logger.LogInformation(
                    "Request processed. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                    method,
                    context.Request.Path,
                    statusCode,
                    duration,
                    correlationId);
            }
        }

        private static string GetEndpointName(HttpContext context)
        {
            // Intentar obtener el nombre del endpoint desde el routing
            var endpoint = context.GetEndpoint();
            if (endpoint?.DisplayName != null)
            {
                return endpoint.DisplayName;
            }

            // Fallback: usar el path
            var path = context.Request.Path.Value ?? "/";
            
            // Limpiar el path para métricas más legibles
            if (path.StartsWith("/api/"))
            {
                return path.Substring(5); // Remover "/api/"
            }
            
            return path;
        }
    }
}
