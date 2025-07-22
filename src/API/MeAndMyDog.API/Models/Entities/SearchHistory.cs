using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing search history for analytics and suggestions
/// </summary>
public class SearchHistory
{
    /// <summary>
    /// Unique identifier for the search history entry
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User who performed the search
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Search query that was performed
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Filters applied during the search (JSON serialized)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? SearchFilters { get; set; }

    /// <summary>
    /// Number of results returned
    /// </summary>
    public int ResultCount { get; set; } = 0;

    /// <summary>
    /// Time taken to execute the search (in milliseconds)
    /// </summary>
    public int ExecutionTimeMs { get; set; } = 0;

    /// <summary>
    /// Whether the user clicked on any results
    /// </summary>
    public bool HasInteraction { get; set; } = false;

    /// <summary>
    /// IP address of the search request
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent of the search request
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// When the search was performed
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}