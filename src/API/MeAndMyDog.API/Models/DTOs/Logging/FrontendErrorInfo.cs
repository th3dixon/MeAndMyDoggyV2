namespace MeAndMyDog.API.Models.DTOs.Logging;

/// <summary>
/// Error information from frontend JavaScript errors
/// </summary>
public class FrontendErrorInfo
{
    /// <summary>
    /// Error message
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// Stack trace of the error
    /// </summary>
    public string? Stack { get; set; }
    
    /// <summary>
    /// Name/type of the error
    /// </summary>
    public string? Name { get; set; }
}