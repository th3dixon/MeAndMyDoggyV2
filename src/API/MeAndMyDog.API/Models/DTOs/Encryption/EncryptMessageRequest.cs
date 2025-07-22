namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for encrypting a message
/// </summary>
public class EncryptMessageRequest
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Plain text message content
    /// </summary>
    public string PlainTextContent { get; set; } = string.Empty;

    /// <summary>
    /// Message type
    /// </summary>
    public string MessageType { get; set; } = "Text";

    /// <summary>
    /// Parent message ID for replies
    /// </summary>
    public string? ParentMessageId { get; set; }

    /// <summary>
    /// Preferred encryption algorithm
    /// </summary>
    public string? PreferredAlgorithm { get; set; }

    /// <summary>
    /// Key ID to use (null for auto-select)
    /// </summary>
    public string? KeyId { get; set; }

    /// <summary>
    /// Whether to use end-to-end encryption
    /// </summary>
    public bool UseEndToEndEncryption { get; set; } = true;
}