using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for video call participants
/// </summary>
public class VideoCallParticipantDto
{
    /// <summary>
    /// Unique identifier for the call participant
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User ID of the participant
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the participant
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// When the participant joined the call
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }
    
    /// <summary>
    /// When the participant left the call
    /// </summary>
    public DateTimeOffset? LeftAt { get; set; }
    
    /// <summary>
    /// Whether the participant's video is enabled
    /// </summary>
    public bool VideoEnabled { get; set; }
    
    /// <summary>
    /// Whether the participant's audio is enabled
    /// </summary>
    public bool AudioEnabled { get; set; }
    
    /// <summary>
    /// Whether the participant is sharing their screen
    /// </summary>
    public bool ScreenSharing { get; set; }
    
    /// <summary>
    /// Participant's connection status
    /// </summary>
    public ConnectionStatus ConnectionStatus { get; set; }
    
    /// <summary>
    /// Participant's network quality
    /// </summary>
    public NetworkQuality? NetworkQuality { get; set; }
    
    /// <summary>
    /// Whether the participant accepted the call invitation
    /// </summary>
    public bool CallAccepted { get; set; }
    
    /// <summary>
    /// Audio level/volume for this participant (0-100)
    /// </summary>
    public int AudioLevel { get; set; }
    
    /// <summary>
    /// Whether this participant is currently speaking
    /// </summary>
    public bool IsSpeaking { get; set; }
    
    /// <summary>
    /// Device information for the participant
    /// </summary>
    public string? DeviceInfo { get; set; }
}