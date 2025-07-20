namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a participant in a conversation
/// </summary>
public class ConversationParticipant
{
    /// <summary>
    /// Unique identifier for the conversation participant
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the conversation
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the user joined the conversation
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }
    
    /// <summary>
    /// When the user left the conversation (if applicable)
    /// </summary>
    public DateTimeOffset? LeftAt { get; set; }
    
    /// <summary>
    /// Whether the user is active in the conversation
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}