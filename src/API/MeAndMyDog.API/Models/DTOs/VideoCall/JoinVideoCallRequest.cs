namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for joining a video call
/// </summary>
public class JoinVideoCallRequest
{
    /// <summary>
    /// ID of the video call session to join
    /// </summary>
    public string CallId { get; set; } = string.Empty;
    
    /// <summary>
    /// Device information for the participant
    /// </summary>
    public string? DeviceInfo { get; set; }
    
    /// <summary>
    /// Whether to join with video enabled
    /// </summary>
    public bool VideoEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether to join with audio enabled
    /// </summary>
    public bool AudioEnabled { get; set; } = true;
}