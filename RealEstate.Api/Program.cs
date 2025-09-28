using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Api.Middleware;
using RealEstate.Api.HealthChecks;
using RealEstate.Api.Services;
using RealEstate.Application.DepedencyInjection;
using RealEstate.Infrastructure.DependencyInjection;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstate.Api.Endpoints;

namespace RealEstate.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var loggerFactory = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            var logger = loggerFactory.CreateLogger("Program");
            logger.LogInformation("Iniciando aplicación...");

            builder.Services.AddRealEstateApplication();
            builder.Services.AddRealEstateInfrastructure(builder.Configuration);

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IPaginationService, PaginationService>();
            builder.Services.AddSingleton<IMetricsService, MetricsService>();

            var jwtSettings = builder.Configuration.GetSection("JWT");
            var secretKey = jwtSettings["secret"] ?? throw new InvalidOperationException("JWT secret is not configured");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["issuer"],
                        ValidAudience = jwtSettings["audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database")
                .AddCheck<SystemHealthCheck>("system")
                .AddCheck("memory", () => 
                {
                    var process = Process.GetCurrentProcess();
                    var memoryUsage = process.WorkingSet64 / 1024 / 1024; // MB
                    return memoryUsage < 500 
                        ? HealthCheckResult.Healthy($"Memoria OK: {memoryUsage}MB")
                        : HealthCheckResult.Degraded($"Alto uso de memoria: {memoryUsage}MB");
                });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                { 
                    Title = "RealEstate API", 
                    Version = "v1" 
                });
                
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            app.UseMiddleware<MetricsMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Habilitar Swagger en todos los ambientes
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstate API v1");
                c.RoutePrefix = "swagger"; // Swagger UI estará disponible en /swagger
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapRealEstateEndpoints();
            app.MapAuthenticationEndpoints();
            app.MapHealthCheckEndpoints();
            app.MapMetricsEndpoints();

            app.Run();
        }
    }
}