namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a read receipt for a message
/// </summary>
public class MessageReadReceipt
{
    /// <summary>
    /// Unique identifier for the read receipt
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the message
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the user who read the message
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the message was read
    /// </summary>
    public DateTimeOffset ReadAt { get; set; }
    
    /// <summary>
    /// Navigation property to the message
    /// </summary>
    public virtual Message Message { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user who read the message
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}