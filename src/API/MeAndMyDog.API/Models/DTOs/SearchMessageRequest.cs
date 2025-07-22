using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for searching messages
/// </summary>
public class SearchMessageRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    [StringLength(500, MinimumLength = 1)]
    public string? Query { get; set; }

    /// <summary>
    /// Specific conversation to search within (optional)
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Filter by sender ID (optional)
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// Filter by message type (optional)
    /// </summary>
    public MessageType? MessageType { get; set; }

    /// <summary>
    /// Predefined date range filter
    /// </summary>
    public SearchDateRange DateRange { get; set; } = SearchDateRange.AllTime;

    /// <summary>
    /// Custom date range start (used when DateRange is Custom)
    /// </summary>
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>
    /// Custom date range end (used when DateRange is Custom)
    /// </summary>
    public DateTimeOffset? DateTo { get; set; }

    /// <summary>
    /// Include messages with attachments
    /// </summary>
    public bool IncludeAttachments { get; set; } = true;

    /// <summary>
    /// Include voice messages in results
    /// </summary>
    public bool IncludeVoiceMessages { get; set; } = true;

    /// <summary>
    /// Include encrypted messages in results
    /// </summary>
    public bool IncludeEncryptedMessages { get; set; } = true;

    /// <summary>
    /// Only include messages with specific tags (comma-separated)
    /// </summary>
    [StringLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Sort results by
    /// </summary>
    public SearchSortBy SortBy { get; set; } = SearchSortBy.Relevance;

    /// <summary>
    /// Number of results to skip (for pagination)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Maximum number of results to return
    /// </summary>
    [Range(1, 100)]
    public int Take { get; set; } = 20;

    /// <summary>
    /// Highlight matched text in results
    /// </summary>
    public bool HighlightMatches { get; set; } = true;

    /// <summary>
    /// Include message context (previous/next messages)
    /// </summary>
    public bool IncludeContext { get; set; } = false;

    /// <summary>
    /// Search in message content only (vs. all searchable fields)
    /// </summary>
    public bool ContentOnly { get; set; } = false;
}