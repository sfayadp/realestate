using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RealEstate.Api.Endpoints
{
    /// <summary>
    /// Endpoints para health checks y monitoreo del sistema
    /// </summary>
    public static class HealthCheckEndpoints
    {
        public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/health")
                .WithTags("Health Checks");

            /// <summary>
            /// Health check básico que retorna el estado general del sistema
            /// </summary>
            /// <returns>Estado de salud del sistema</returns>
            /// <response code="200">Sistema funcionando correctamente</response>
            /// <response code="503">Sistema con problemas</response>
            group.MapGet("/", async (HealthCheckService healthCheckService) =>
            {
                var result = await healthCheckService.CheckHealthAsync();
                
                var response = new
                {
                    status = result.Status.ToString(),
                    totalDuration = result.TotalDuration.TotalMilliseconds,
                    timestamp = DateTime.UtcNow,
                    checks = result.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        duration = entry.Value.Duration.TotalMilliseconds,
                        description = entry.Value.Description,
                        data = entry.Value.Data
                    })
                };

                return result.Status == HealthStatus.Healthy 
                    ? Results.Ok(response)
                    : Results.StatusCode(503);
            })
            .WithName("GetHealth");

            /// <summary>
            /// Health check detallado con información específica de cada componente
            /// </summary>
            /// <returns>Estado detallado de todos los health checks</returns>
            /// <response code="200">Información detallada de salud del sistema</response>
            group.MapGet("/detailed", async (HealthCheckService healthCheckService) =>
            {
                var result = await healthCheckService.CheckHealthAsync();
                
                var response = new
                {
                    status = result.Status.ToString(),
                    totalDuration = result.TotalDuration.TotalMilliseconds,
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    checks = result.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        duration = entry.Value.Duration.TotalMilliseconds,
                        description = entry.Value.Description,
                        exception = entry.Value.Exception?.Message,
                        data = entry.Value.Data
                    })
                };

                return Results.Ok(response);
            })
            .WithName("GetDetailedHealth");

            /// <summary>
            /// Health check simple para load balancers (solo retorna OK/FAIL)
            /// </summary>
            /// <returns>Estado simple del sistema</returns>
            /// <response code="200">Sistema funcionando</response>
            /// <response code="503">Sistema con problemas</response>
            group.MapGet("/ping", async (HealthCheckService healthCheckService) =>
            {
                var result = await healthCheckService.CheckHealthAsync();
                return result.Status == HealthStatus.Healthy 
                    ? Results.Ok(new { status = "OK", timestamp = DateTime.UtcNow })
                    : Results.StatusCode(503);
            })
            .WithName("Ping");
        }
    }
}
