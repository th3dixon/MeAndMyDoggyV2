namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a reaction to a message
/// </summary>
public class MessageReaction
{
    /// <summary>
    /// Unique identifier for the reaction
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the message
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the user who reacted
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// The reaction type (emoji or text)
    /// </summary>
    public string Reaction { get; set; } = string.Empty;
    
    /// <summary>
    /// When the reaction was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the message
    /// </summary>
    public virtual Message Message { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user who reacted
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}