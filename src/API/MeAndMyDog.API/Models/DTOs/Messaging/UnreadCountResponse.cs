namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for unread count requests
/// </summary>
public class UnreadCountResponse
{
    /// <summary>
    /// Total unread messages across all conversations
    /// </summary>
    public int TotalUnreadMessages { get; set; }

    /// <summary>
    /// Number of conversations with unread messages
    /// </summary>
    public int UnreadConversations { get; set; }

    /// <summary>
    /// Unread counts per conversation
    /// </summary>
    public Dictionary<string, int> ConversationUnreadCounts { get; set; } = new();

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// Specific conversation ID (for single conversation requests)
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Unread count for specific conversation (for single conversation requests)
    /// </summary>
    public int UnreadCount { get; set; }
}