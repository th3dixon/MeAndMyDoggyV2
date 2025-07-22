using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for video call sessions
/// </summary>
public class VideoCallDto
{
    /// <summary>
    /// Unique identifier for the video call session
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the conversation this call belongs to
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the user who initiated the call
    /// </summary>
    public string InitiatorId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the user who initiated the call
    /// </summary>
    public string InitiatorName { get; set; } = string.Empty;
    
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
    /// Status of the call
    /// </summary>
    public CallStatus Status { get; set; }
    
    /// <summary>
    /// Room identifier for the video call
    /// </summary>
    public string? RoomId { get; set; }
    
    /// <summary>
    /// Call quality rating (1-5 stars)
    /// </summary>
    public int? QualityRating { get; set; }
    
    /// <summary>
    /// Whether the call was recorded
    /// </summary>
    public bool IsRecorded { get; set; }
    
    /// <summary>
    /// URL to the call recording (if recorded)
    /// </summary>
    public string? RecordingUrl { get; set; }
    
    /// <summary>
    /// Whether screen sharing was used during the call
    /// </summary>
    public bool ScreenSharingUsed { get; set; }
    
    /// <summary>
    /// Maximum number of participants in the call
    /// </summary>
    public int MaxParticipants { get; set; }
    
    /// <summary>
    /// Network quality during the call
    /// </summary>
    public NetworkQuality? NetworkQuality { get; set; }
    
    /// <summary>
    /// When the call session was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// List of participants in the call
    /// </summary>
    public List<VideoCallParticipantDto> Participants { get; set; } = new();
}