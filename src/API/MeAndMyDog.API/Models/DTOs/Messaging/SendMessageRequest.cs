using System.ComponentModel.DataAnnotations;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for sending a message
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// Conversation ID to send message to
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Message content
    /// </summary>
    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Type of message
    /// </summary>
    public MessageType MessageType { get; set; } = MessageType.Text;

    /// <summary>
    /// Parent message ID for replies
    /// </summary>
    public string? ParentMessageId { get; set; }

    /// <summary>
    /// List of file attachment URLs
    /// </summary>
    public List<string>? AttachmentUrls { get; set; }

    /// <summary>
    /// Scheduled time to send message (null for immediate)
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// Whether to require end-to-end encryption
    /// </summary>
    public bool RequireEncryption { get; set; } = false;
}