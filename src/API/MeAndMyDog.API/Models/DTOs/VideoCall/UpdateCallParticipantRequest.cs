using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for updating call participant settings
/// </summary>
public class UpdateCallParticipantRequest
{
    /// <summary>
    /// ID of the video call session
    /// </summary>
    public string CallId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether video is enabled
    /// </summary>
    public bool? VideoEnabled { get; set; }
    
    /// <summary>
    /// Whether audio is enabled
    /// </summary>
    public bool? AudioEnabled { get; set; }
    
    /// <summary>
    /// Whether screen sharing is active
    /// </summary>
    public bool? ScreenSharing { get; set; }
    
    /// <summary>
    /// Audio level (0-100)
    /// </summary>
    public int? AudioLevel { get; set; }
    
    /// <summary>
    /// Whether participant is speaking
    /// </summary>
    public bool? IsSpeaking { get; set; }
    
    /// <summary>
    /// Network quality assessment
    /// </summary>
    public NetworkQuality? NetworkQuality { get; set; }
}