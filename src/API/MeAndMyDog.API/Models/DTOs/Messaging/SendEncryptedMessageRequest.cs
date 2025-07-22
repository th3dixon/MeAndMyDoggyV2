using System.ComponentModel.DataAnnotations;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for sending an encrypted message
/// </summary>
public class SendEncryptedMessageRequest
{
    /// <summary>
    /// Conversation ID to send message to
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Encrypted message content
    /// </summary>
    [Required]
    public string EncryptedContent { get; set; } = string.Empty;

    /// <summary>
    /// Encryption key ID used
    /// </summary>
    [Required]
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// Encryption algorithm used
    /// </summary>
    [Required]
    public string Algorithm { get; set; } = string.Empty;

    /// <summary>
    /// Parent message ID for replies
    /// </summary>
    public string? ParentMessageId { get; set; }

    /// <summary>
    /// List of encrypted attachment URLs
    /// </summary>
    public List<string>? EncryptedAttachmentUrls { get; set; }
    
    /// <summary>
    /// Plain text content (before encryption)
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// Type of message
    /// </summary>
    public MessageType MessageType { get; set; } = MessageType.Text;
    
    /// <summary>
    /// Whether to use end-to-end encryption
    /// </summary>
    public bool UseEndToEndEncryption { get; set; } = true;
}