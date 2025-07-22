using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for messages
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Message unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Conversation ID this message belongs to
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// User ID of the message sender
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the message sender
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the message (Text, Image, File, etc.)
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the message was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Message delivery status
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// Whether the message has been edited
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// When the message was last edited (if edited)
    /// </summary>
    public DateTimeOffset? EditedAt { get; set; }

    /// <summary>
    /// Whether the message is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; }

    /// <summary>
    /// File attachments associated with this message
    /// </summary>
    public List<MessageAttachmentDto> Attachments { get; set; } = new();

    /// <summary>
    /// Reactions to this message
    /// </summary>
    public List<MessageReactionDto> Reactions { get; set; } = new();
}