namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a video call session between users
/// </summary>
public class VideoCallSession
{
    /// <summary>
    /// Unique identifier for the video call session
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the conversation
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the user who initiated the call
    /// </summary>
    public string InitiatorId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the call started
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
    
    /// <summary>
    /// When the call ended
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }
    
    /// <summary>
    /// Duration of the call in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }
    
    /// <summary>
    /// Status of the call (Pending, Active, Ended, Cancelled)
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Room or session identifier for the video call service
    /// </summary>
    public string? RoomId { get; set; }
    
    /// <summary>
    /// WebRTC session description protocol offer
    /// </summary>
    public string? SdpOffer { get; set; }
    
    /// <summary>
    /// WebRTC session description protocol answer
    /// </summary>
    public string? SdpAnswer { get; set; }
    
    /// <summary>
    /// ICE candidates for WebRTC connection (stored as JSON)
    /// </summary>
    public string? IceCandidates { get; set; }
    
    /// <summary>
    /// Call quality rating (1-5 stars)
    /// </summary>
    public int? QualityRating { get; set; }
    
    /// <summary>
    /// Whether the call was recorded
    /// </summary>
    public bool IsRecorded { get; set; } = false;
    
    /// <summary>
    /// URL to the call recording (if recorded)
    /// </summary>
    public string? RecordingUrl { get; set; }
    
    /// <summary>
    /// Whether screen sharing was used during the call
    /// </summary>
    public bool ScreenSharingUsed { get; set; } = false;
    
    /// <summary>
    /// Maximum number of participants in the call
    /// </summary>
    public int MaxParticipants { get; set; } = 2;
    
    /// <summary>
    /// Call rejection reason (if call was rejected)
    /// </summary>
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Network quality during the call (Poor, Fair, Good, Excellent)
    /// </summary>
    public string? NetworkQuality { get; set; }
    
    /// <summary>
    /// When the call session was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// When the call session was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user who initiated the call
    /// </summary>
    public virtual ApplicationUser Initiator { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to call participants
    /// </summary>
    public virtual ICollection<VideoCallParticipant> Participants { get; set; } = new List<VideoCallParticipant>();
}