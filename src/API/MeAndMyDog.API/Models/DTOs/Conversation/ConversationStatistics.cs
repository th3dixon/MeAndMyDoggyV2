using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Conversation statistics
/// </summary>
public class ConversationStatistics
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of messages
    /// </summary>
    public int TotalMessages { get; set; }

    /// <summary>
    /// Number of active participants
    /// </summary>
    public int ActiveParticipants { get; set; }

    /// <summary>
    /// When conversation was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When last message was sent
    /// </summary>
    public DateTimeOffset? LastMessageAt { get; set; }

    /// <summary>
    /// Average messages per day (last 30 days)
    /// </summary>
    public double AverageMessagesPerDay { get; set; }

    /// <summary>
    /// Most active participant
    /// </summary>
    public ParticipantActivity? MostActiveParticipant { get; set; }

    /// <summary>
    /// Message type breakdown
    /// </summary>
    public Dictionary<MessageType, int> MessageTypeBreakdown { get; set; } = new();
}