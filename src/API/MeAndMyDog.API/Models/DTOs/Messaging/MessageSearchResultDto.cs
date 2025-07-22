using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Search result containing a message match
/// </summary>
public class MessageSearchResultDto
{
    /// <summary>
    /// Message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation ID where the message was found
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation name/title
    /// </summary>
    public string ConversationName { get; set; } = string.Empty;

    /// <summary>
    /// Conversation title (alias for ConversationName)
    /// </summary>
    public string ConversationTitle { get; set; } = string.Empty;

    /// <summary>
    /// Message sender ID
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Message content (possibly highlighted)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Original content without highlighting
    /// </summary>
    public string OriginalContent { get; set; } = string.Empty;

    /// <summary>
    /// Message type
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// When the message was sent
    /// </summary>
    public DateTimeOffset SentAt { get; set; }

    /// <summary>
    /// When the message was created (alias for SentAt)
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Search relevance score (0-100)
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Whether message is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; }

    /// <summary>
    /// Whether message has attachments
    /// </summary>
    public bool HasAttachments { get; set; }

    /// <summary>
    /// Number of attachments
    /// </summary>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// Message tags
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Context messages (if requested)
    /// </summary>
    public List<MessageContextDto>? Context { get; set; }

    /// <summary>
    /// Matched text snippets
    /// </summary>
    public List<string> MatchedSnippets { get; set; } = new();

    /// <summary>
    /// Text snippet for search results
    /// </summary>
    public string Snippet { get; set; } = string.Empty;
}