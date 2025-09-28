using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace RealEstate.Api.HealthChecks
{
    /// <summary>
    /// Health check para verificar el estado general del sistema
    /// </summary>
    public class SystemHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar memoria disponible
                var process = Process.GetCurrentProcess();
                var memoryUsage = process.WorkingSet64 / 1024 / 1024; // MB
                var memoryLimit = 500; // MB límite

                // Verificar uptime
                var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();

                // Verificar espacio en disco (simulado)
                var diskSpace = GetDiskSpace();

                var data = new Dictionary<string, object>
                {
                    ["memory_usage_mb"] = memoryUsage,
                    ["memory_limit_mb"] = memoryLimit,
                    ["uptime_hours"] = Math.Round(uptime.TotalHours, 2),
                    ["disk_space_gb"] = diskSpace,
                    ["timestamp"] = DateTime.UtcNow,
                    ["version"] = "1.0.0"
                };

                // Determinar estado basado en métricas
                if (memoryUsage > memoryLimit)
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Alto uso de memoria", data: data));
                }

                if (diskSpace < 1) // Menos de 1GB
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Poco espacio en disco", data: data));
                }

                return Task.FromResult(HealthCheckResult.Healthy("Sistema funcionando correctamente", data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Error al verificar el sistema: {ex.Message}", ex));
            }
        }

        private static double GetDiskSpace()
        {
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:");
                return Math.Round(drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0, 2); // GB
            }
            catch
            {
                return 10; // Valor por defecto
            }
        }
    }
}
