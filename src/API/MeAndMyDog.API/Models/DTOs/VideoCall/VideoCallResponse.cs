namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for video call operations
/// </summary>
public class VideoCallResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Video call data (if applicable)
    /// </summary>
    public VideoCallDto? Call { get; set; }
    
    /// <summary>
    /// Additional data for the response
    /// </summary>
    public object? Data { get; set; }
}