using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Logging;

/// <summary>
/// Individual log entry from frontend applications
/// </summary>
public class FrontendLogEntry
{
    /// <summary>
    /// Timestamp when the log entry was created
    /// </summary>
    [Required]
    public string Timestamp { get; set; } = string.Empty;

    /// <summary>
    /// Log level (0=Debug, 1=Info, 2=Warning, 3=Error, 4=Critical)
    /// </summary>
    [Range(0, 4)]
    public int Level { get; set; }

    /// <summary>
    /// Log message content
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional context data for the log entry
    /// </summary>
    public Dictionary<string, object?>? Context { get; set; }
    
    /// <summary>
    /// Error information if this is an error log entry
    /// </summary>
    public FrontendErrorInfo? Error { get; set; }
    
    /// <summary>
    /// User ID associated with the log entry
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Session ID for tracking user sessions
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// User agent string from the browser
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// URL where the log entry was generated
    /// </summary>
    public string? Url { get; set; }
}