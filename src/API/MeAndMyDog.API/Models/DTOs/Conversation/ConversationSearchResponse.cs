namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for conversation search
/// </summary>
public class ConversationSearchResponse
{
    /// <summary>
    /// Search results
    /// </summary>
    public List<ConversationSearchResultDto> Results { get; set; } = new();

    /// <summary>
    /// Total number of matching conversations
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
    /// Whether there are more results available
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Search query used
    /// </summary>
    public string Query { get; set; } = string.Empty;
}