namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a message in a conversation
/// </summary>
public class Message
{
    /// <summary>
    /// Unique identifier for the message
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the conversation
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the sender
    /// </summary>
    public string SenderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Message content
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Message type (Text, Image, File, etc.)
    /// </summary>
    public string MessageType { get; set; } = "Text";
    
    /// <summary>
    /// Delivery status
    /// </summary>
    public string DeliveryStatus { get; set; } = "Sent";
    
    /// <summary>
    /// Whether the message was edited
    /// </summary>
    public bool IsEdited { get; set; } = false;
    
    /// <summary>
    /// AI moderation result
    /// </summary>
    public string? ModerationResult { get; set; }
    
    /// <summary>
    /// When the message was sent
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
    
    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the sender
    /// </summary>
    public virtual ApplicationUser Sender { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to attachments
    /// </summary>
    public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
    
    /// <summary>
    /// Navigation property to reactions
    /// </summary>
    public virtual ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
    
    /// <summary>
    /// Navigation property to read receipts
    /// </summary>
    public virtual ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
}