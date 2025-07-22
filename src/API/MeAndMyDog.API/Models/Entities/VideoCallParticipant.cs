namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a participant in a video call session
/// </summary>
public class VideoCallParticipant
{
    /// <summary>
    /// Unique identifier for the call participant
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the video call session
    /// </summary>
    public string VideoCallSessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the user participating in the call
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
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
    public bool VideoEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether the participant's audio is enabled
    /// </summary>
    public bool AudioEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether the participant is sharing their screen
    /// </summary>
    public bool ScreenSharing { get; set; } = false;
    
    /// <summary>
    /// Participant's connection status (Connecting, Connected, Disconnected, Reconnecting)
    /// </summary>
    public string ConnectionStatus { get; set; } = "Connecting";
    
    /// <summary>
    /// Participant's network quality (Poor, Fair, Good, Excellent)
    /// </summary>
    public string? NetworkQuality { get; set; }
    
    /// <summary>
    /// Whether the participant accepted the call invitation
    /// </summary>
    public bool CallAccepted { get; set; } = false;
    
    /// <summary>
    /// Reason for rejecting the call (if rejected)
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// WebRTC peer connection ID for this participant
    /// </summary>
    public string? PeerConnectionId { get; set; }
    
    /// <summary>
    /// Audio level/volume for this participant (0-100)
    /// </summary>
    public int AudioLevel { get; set; } = 0;
    
    /// <summary>
    /// Whether this participant is currently speaking
    /// </summary>
    public bool IsSpeaking { get; set; } = false;
    
    /// <summary>
    /// Device information for the participant
    /// </summary>
    public string? DeviceInfo { get; set; }
    
    /// <summary>
    /// Navigation property to the video call session
    /// </summary>
    public virtual VideoCallSession VideoCallSession { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}