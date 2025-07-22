namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for message list requests
/// </summary>
public class MessageListResponse
{
    /// <summary>
    /// List of messages
    /// </summary>
    public List<MessageDto> Messages { get; set; } = new();

    /// <summary>
    /// Total number of messages
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
    /// Whether there are more messages available
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Whether messages are in chronological order
    /// </summary>
    public bool IsChronological { get; set; } = true;
}