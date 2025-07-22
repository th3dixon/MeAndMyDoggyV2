using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Message search request
/// </summary>
public class MessageSearchRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Conversation IDs to search within (null = all accessible conversations)
    /// </summary>
    public List<string>? ConversationIds { get; set; }

    /// <summary>
    /// Message types to include
    /// </summary>
    public List<MessageType>? MessageTypes { get; set; }

    /// <summary>
    /// User ID to search messages from (null = all users)
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// Start date for search range
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// End date for search range
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Sort results by
    /// </summary>
    public SearchSortBy SortBy { get; set; } = SearchSortBy.Relevance;

    /// <summary>
    /// Sort direction (ascending/descending)
    /// </summary>
    public bool Ascending { get; set; } = false;

    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Whether to include message context
    /// </summary>
    public bool IncludeContext { get; set; } = false;

    /// <summary>
    /// Whether to highlight matching text
    /// </summary>
    public bool HighlightMatches { get; set; } = true;

    /// <summary>
    /// Search within encrypted messages (if user has decryption rights)
    /// </summary>
    public bool SearchEncrypted { get; set; } = false;

    /// <summary>
    /// Include deleted messages in results
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;

    /// <summary>
    /// Fuzzy search tolerance (0-1, where 1 is exact match only)
    /// </summary>
    public double FuzzyTolerance { get; set; } = 0.8;

    /// <summary>
    /// Tags to filter by
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Search filters
    /// </summary>
    public SearchFilterDto? Filters { get; set; }
}