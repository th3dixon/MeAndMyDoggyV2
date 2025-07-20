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
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user who initiated the call
    /// </summary>
    public virtual ApplicationUser Initiator { get; set; } = null!;
}