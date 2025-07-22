using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for conversation search results
/// </summary>
public class ConversationSearchResultDto
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Conversation type
    /// </summary>
    public ConversationType ConversationType { get; set; }

    /// <summary>
    /// When the last message was sent
    /// </summary>
    public DateTimeOffset? LastMessageAt { get; set; }

    /// <summary>
    /// Number of active participants
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// Number of unread messages for current user
    /// </summary>
    public int UnreadCount { get; set; }
}