namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Summary of filters that were applied to the search
/// </summary>
public class SearchFiltersAppliedDto
{
    /// <summary>
    /// Conversation filter applied
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Sender filter applied
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// Message type filter applied
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// Date range filter applied
    /// </summary>
    public string? DateRange { get; set; }

    /// <summary>
    /// Tags filter applied
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Sort criteria applied
    /// </summary>
    public string SortBy { get; set; } = string.Empty;
}