namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Roles that participants can have in a conversation
/// </summary>
public enum ConversationRole
{
    /// <summary>
    /// Regular member of the conversation
    /// </summary>
    Member = 0,
    
    /// <summary>
    /// Administrator with elevated permissions
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// Owner of the conversation with full control
    /// </summary>
    Owner = 2,
    
    /// <summary>
    /// Moderator with moderation permissions
    /// </summary>
    Moderator = 3
}