using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using FluentValidation;
using System.Diagnostics;

namespace RealEstate.Api.Middleware
{
    /// <summary>
    /// Middleware global para manejo centralizado de excepciones con logging estructurado
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = Guid.NewGuid().ToString();
            
            // Agregar correlation ID al contexto
            context.Items["CorrelationId"] = correlationId;
            
            try
            {
                await _next(context);
                stopwatch.Stop();
                
                // Log de requests exitosos
                _logger.LogInformation(
                    "Request completed successfully. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                // Log estructurado del error
                _logger.LogError(ex,
                    "Unhandled exception occurred. Method: {Method}, Path: {Path}, Duration: {Duration}ms, CorrelationId: {CorrelationId}, ExceptionType: {ExceptionType}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    correlationId,
                    ex.GetType().Name);
                
                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path,
                Method = context.Request.Method
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Errores de validación";
                    response.Details = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                    response.ErrorCode = "VALIDATION_ERROR";
                    break;

                case DbUpdateException dbEx:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    
                    // Mejorar el mensaje basado en el tipo de error
                    if (dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        switch (sqlEx.Number)
                        {
                            case 2627: // Violación de restricción única
                                response.Message = "El valor ya existe en la base de datos";
                                response.Details = new List<string> 
                                { 
                                    "El valor que intentas guardar ya existe",
                                    "Por favor, elige un valor diferente"
                                };
                                response.ErrorCode = "DUPLICATE_VALUE";
                                break;
                                
                            case 547: // Violación de clave foránea
                                response.Message = "No se puede realizar la operación debido a restricciones de datos";
                                response.Details = new List<string> 
                                { 
                                    "El registro está siendo referenciado por otros datos",
                                    "Elimina primero las referencias antes de continuar"
                                };
                                response.ErrorCode = "FOREIGN_KEY_CONSTRAINT";
                                break;
                                
                            default:
                                response.Message = "Error en la base de datos";
                                response.Details = new List<string> { "No se pudo completar la operación en la base de datos" };
                                response.ErrorCode = "DATABASE_ERROR";
                                break;
                        }
                    }
                    else
                    {
                        response.Message = "Error en la base de datos";
                        response.Details = new List<string> { "No se pudo completar la operación en la base de datos" };
                        response.ErrorCode = "DATABASE_ERROR";
                    }
                    
                    // Log específico para errores de BD
                    _logger.LogError(dbEx, "Database error occurred. CorrelationId: {CorrelationId}", correlationId);
                    break;

                case InvalidOperationException invalidOpEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Recurso no encontrado";
                    response.Details = new List<string> { "No se encontró el registro solicitado" };
                    response.ErrorCode = "RESOURCE_NOT_FOUND";
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Argumento inválido";
                    response.Details = new List<string> { argEx.Message };
                    response.ErrorCode = "INVALID_ARGUMENT";
                    break;

                case FormatException formatEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Formato de datos inválido";
                    response.Details = new List<string> { "El formato de los datos enviados no es válido" };
                    response.ErrorCode = "INVALID_FORMAT";
                    break;

                case TimeoutException timeoutEx:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Message = "Tiempo de espera agotado";
                    response.Details = new List<string> { "La operación tardó demasiado tiempo en completarse" };
                    response.ErrorCode = "TIMEOUT";
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "No autorizado";
                    response.Details = new List<string> { "No tiene permisos para realizar esta operación" };
                    response.ErrorCode = "UNAUTHORIZED";
                    break;

                case NotImplementedException:
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    response.Message = "Funcionalidad no implementada";
                    response.Details = new List<string> { "Esta funcionalidad aún no está disponible" };
                    response.ErrorCode = "NOT_IMPLEMENTED";
                    break;

                case TaskCanceledException:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Message = "Operación cancelada";
                    response.Details = new List<string> { "La operación fue cancelada" };
                    response.ErrorCode = "OPERATION_CANCELLED";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Error interno del servidor";
                    response.Details = new List<string> { "Ha ocurrido un error inesperado" };
                    response.ErrorCode = "INTERNAL_SERVER_ERROR";
                    
                    // Log crítico para errores no manejados
                    _logger.LogCritical(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            // Agregar headers útiles para debugging
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            context.Response.Headers["X-Error-Code"] = response.ErrorCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
