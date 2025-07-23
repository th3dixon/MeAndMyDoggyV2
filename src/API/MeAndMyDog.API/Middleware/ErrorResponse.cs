namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Standard error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Stack trace (development only)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Timestamp of the error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validation errors (for model validation failures)
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}