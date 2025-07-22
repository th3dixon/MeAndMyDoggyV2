using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for video call sessions
/// </summary>
public class VideoCallSessionDto
{
    /// <summary>
    /// Session unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Call session identifier
    /// </summary>
    public string CallId { get; set; } = string.Empty;

    /// <summary>
    /// User who initiated the call
    /// </summary>
    public string InitiatorId { get; set; } = string.Empty;

    /// <summary>
    /// User receiving the call
    /// </summary>
    public string RecipientId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation ID associated with the call
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Current status of the call
    /// </summary>
    public CallStatus Status { get; set; }

    /// <summary>
    /// When the call was started
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// When the call ended (if ended)
    /// </summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>
    /// Duration of the call in seconds
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// List of participants in the call
    /// </summary>
    public List<VideoCallParticipantDto> Participants { get; set; } = new();

    /// <summary>
    /// WebRTC configuration for the session
    /// </summary>
    public string? WebRTCConfig { get; set; }

    /// <summary>
    /// Maximum number of participants allowed
    /// </summary>
    public int MaxParticipants { get; set; } = 10;

    /// <summary>
    /// Whether the call is being recorded
    /// </summary>
    public bool IsRecording { get; set; }

    /// <summary>
    /// Quality metrics for the call
    /// </summary>
    public CallQualityMetrics? QualityMetrics { get; set; }
}