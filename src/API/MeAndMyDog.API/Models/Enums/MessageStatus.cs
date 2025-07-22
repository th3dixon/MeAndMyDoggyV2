namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of a message
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// Message has been sent
    /// </summary>
    Sent = 0,
    
    /// <summary>
    /// Message has been delivered to recipient's device
    /// </summary>
    Delivered = 1,
    
    /// <summary>
    /// Message has been read by recipient
    /// </summary>
    Read = 2,
    
    /// <summary>
    /// Message failed to send
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Message is pending moderation
    /// </summary>
    Pending = 4,
    
    /// <summary>
    /// Message was blocked by moderation
    /// </summary>
    Blocked = 5
}