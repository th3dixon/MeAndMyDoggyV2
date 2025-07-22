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
    /// Conversation name (alias for Title)
    /// </summary>
    public string? Name
    {
        get => Title;
        set => Title = value;
    }
    
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
    
    // Additional properties for comprehensive messaging
    
    /// <summary>
    /// Conversation type enum property for API consistency
    /// </summary>
    public string ConversationType
    {
        get => Type;
        set => Type = value;
    }
    
    /// <summary>
    /// Conversation description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL to conversation image/avatar
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// When the conversation was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// ID of the last message in the conversation
    /// </summary>
    public string? LastMessageId { get; set; }
    
    /// <summary>
    /// Preview of the last message
    /// </summary>
    public string? LastMessagePreview { get; set; }
    
    /// <summary>
    /// Total number of messages in the conversation
    /// </summary>
    public int MessageCount { get; set; } = 0;
    
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