namespace RealEstate.Api.Middleware
{
    /// <summary>
    /// Respuesta de error estructurada con información detallada para debugging
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Details { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }
}
