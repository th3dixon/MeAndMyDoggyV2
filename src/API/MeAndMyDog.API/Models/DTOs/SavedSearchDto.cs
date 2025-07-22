using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for saved search
/// </summary>
public class SavedSearchDto
{
    /// <summary>
    /// Search ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns the search
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Search name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Search query
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Conversation filter
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Sender filter
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// Message type filter
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// Date from filter
    /// </summary>
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>
    /// Date to filter
    /// </summary>
    public DateTimeOffset? DateTo { get; set; }

    /// <summary>
    /// Include attachments flag
    /// </summary>
    public bool IncludeAttachments { get; set; }

    /// <summary>
    /// Include voice messages flag
    /// </summary>
    public bool IncludeVoiceMessages { get; set; }

    /// <summary>
    /// Include encrypted messages flag
    /// </summary>
    public bool IncludeEncryptedMessages { get; set; }

    /// <summary>
    /// Tags filter
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Whether search is pinned
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Usage count
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Last used timestamp
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Updated timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
