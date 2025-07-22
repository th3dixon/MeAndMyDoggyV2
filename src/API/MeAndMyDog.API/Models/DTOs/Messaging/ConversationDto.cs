using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for conversations
/// </summary>
public class ConversationDto
{
    /// <summary>
    /// Conversation unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of conversation
    /// </summary>
    public ConversationType ConversationType { get; set; }

    /// <summary>
    /// Conversation title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Conversation description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Conversation image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// When conversation was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When last message was sent
    /// </summary>
    public DateTimeOffset? LastMessageAt { get; set; }

    /// <summary>
    /// Preview of last message
    /// </summary>
    public string? LastMessagePreview { get; set; }

    /// <summary>
    /// Total number of messages
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Number of unread messages for current user
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// List of conversation participants
    /// </summary>
    public List<ParticipantDto> Participants { get; set; } = new();

    /// <summary>
    /// Whether conversation is archived for current user
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Whether conversation is pinned for current user
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Whether conversation is muted for current user
    /// </summary>
    public bool IsMuted { get; set; }
}