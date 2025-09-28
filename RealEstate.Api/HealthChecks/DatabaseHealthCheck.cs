using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstate.Infrastructure.Contexts;

namespace RealEstate.Api.HealthChecks
{
    /// <summary>
    /// Health check personalizado para verificar la conectividad de la base de datos
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly RealEstateDbContext _context;

        public DatabaseHealthCheck(RealEstateDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar conectividad a la base de datos
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                
                if (!canConnect)
                {
                    return HealthCheckResult.Unhealthy("No se puede conectar a la base de datos");
                }

                // Verificar que las tablas principales existan usando una consulta simple
                try
                {
                    var propertyCount = await _context.Property.CountAsync(cancellationToken);
                    
                    var data = new Dictionary<string, object>
                    {
                        ["database"] = "RealEstateDB",
                        ["property_count"] = propertyCount,
                        ["connection_status"] = "Connected",
                        ["timestamp"] = DateTime.UtcNow
                    };

                    return HealthCheckResult.Healthy("Base de datos conectada y funcionando correctamente", data);
                }
                catch (Exception tableEx)
                {
                    // Si no puede acceder a la tabla Property, pero la conexión funciona
                    var data = new Dictionary<string, object>
                    {
                        ["database"] = "RealEstateDB",
                        ["connection_status"] = "Connected",
                        ["table_access"] = "Limited",
                        ["timestamp"] = DateTime.UtcNow
                    };

                    return HealthCheckResult.Degraded($"Base de datos conectada pero con limitaciones: {tableEx.Message}", tableEx);
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Error al verificar la base de datos: {ex.Message}", ex);
            }
        }
    }
}
