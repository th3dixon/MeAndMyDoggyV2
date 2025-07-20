namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a conversation between users
/// </summary>
public class Conversation
{
    /// <summary>
    /// Unique identifier for the conversation
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Conversation title
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Conversation type (Direct, Group, Support)
    /// </summary>
    public string Type { get; set; } = "Direct";
    
    /// <summary>
    /// User who created the conversation
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// When the conversation was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the last message was sent
    /// </summary>
    public DateTimeOffset? LastMessageAt { get; set; }
    
    /// <summary>
    /// Whether the conversation is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Navigation property to the creator
    /// </summary>
    public virtual ApplicationUser Creator { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to participants
    /// </summary>
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    
    /// <summary>
    /// Navigation property to messages
    /// </summary>
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}