namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for conversation list requests
/// </summary>
public class ConversationListResponse
{
    /// <summary>
    /// List of conversations
    /// </summary>
    public List<ConversationDto> Conversations { get; set; } = new();

    /// <summary>
    /// Total number of conversations
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Whether there are more conversations available
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Total number of unread messages across all conversations
    /// </summary>
    public int UnreadTotal { get; set; }
}