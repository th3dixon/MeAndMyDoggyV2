using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Search query log for analytics and optimization
/// </summary>
[Table("SearchQueryLogs")]
public class SearchQueryLog
{
    /// <summary>
    /// Unique log entry identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// User who performed the search
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Search query text
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string QueryText { get; set; } = string.Empty;

    /// <summary>
    /// Normalized/processed query
    /// </summary>
    [MaxLength(1000)]
    public string NormalizedQuery { get; set; } = string.Empty;

    /// <summary>
    /// Conversation IDs searched (JSON array)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ConversationIds { get; set; }

    /// <summary>
    /// Search filters applied (JSON object)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? SearchFilters { get; set; }

    /// <summary>
    /// Number of results returned
    /// </summary>
    public int ResultCount { get; set; }

    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// When the search was performed
    /// </summary>
    public DateTimeOffset SearchedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// User's IP address (for analytics)
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Whether user found what they were looking for
    /// </summary>
    public bool? FoundResult { get; set; }

    /// <summary>
    /// Which result was clicked (if any)
    /// </summary>
    public int? ClickedResultIndex { get; set; }

    #region Navigation Properties

    /// <summary>
    /// User who performed the search
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    #endregion
}