namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for message search results
/// </summary>
public class SearchMessageResponse
{
    /// <summary>
    /// Whether the search was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Search results
    /// </summary>
    public List<MessageSearchResultDto> Results { get; set; } = new();

    /// <summary>
    /// Total number of matches found
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of results returned in this response
    /// </summary>
    public int ReturnedCount { get; set; }

    /// <summary>
    /// Current page/skip offset
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Results per page
    /// </summary>
    public int Take { get; set; }

    /// <summary>
    /// Whether there are more results available
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public int ExecutionTimeMs { get; set; }

    /// <summary>
    /// Search query that was executed
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Filters that were applied
    /// </summary>
    public SearchFiltersAppliedDto? FiltersApplied { get; set; }

    /// <summary>
    /// Suggested corrections or alternative queries
    /// </summary>
    public List<string> Suggestions { get; set; } = new();

    /// <summary>
    /// Any search warnings or messages
    /// </summary>
    public List<string> Messages { get; set; } = new();

    /// <summary>
    /// Error message if search failed
    /// </summary>
    public string? Error { get; set; }
}