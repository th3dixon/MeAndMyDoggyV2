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
    
    // Additional properties for comprehensive messaging
    
    /// <summary>
    /// User's role in the conversation (Member, Admin, Owner, Moderator)
    /// </summary>
    public string Role { get; set; } = "Member";
    
    /// <summary>
    /// Number of unread messages for this participant
    /// </summary>
    public int UnreadCount { get; set; } = 0;
    
    /// <summary>
    /// When the participant last read messages
    /// </summary>
    public DateTimeOffset? LastReadAt { get; set; }
    
    /// <summary>
    /// ID of the last message read by this participant
    /// </summary>
    public string? LastReadMessageId { get; set; }
    
    /// <summary>
    /// Whether the conversation is archived for this participant
    /// </summary>
    public bool IsArchived { get; set; } = false;
    
    /// <summary>
    /// Whether the conversation is pinned for this participant
    /// </summary>
    public bool IsPinned { get; set; } = false;
    
    /// <summary>
    /// Whether the conversation is muted for this participant
    /// </summary>
    public bool IsMuted { get; set; } = false;
    
    /// <summary>
    /// When the conversation should be unmuted (if temporarily muted)
    /// </summary>
    public DateTimeOffset? MutedUntil { get; set; }
    
    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}