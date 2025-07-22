using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Popular search terms for suggestions
/// </summary>
[Table("PopularSearchTerms")]
public class PopularSearchTerm
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Search term
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Term { get; set; } = string.Empty;

    /// <summary>
    /// Normalized term for matching
    /// </summary>
    [MaxLength(200)]
    public string NormalizedTerm { get; set; } = string.Empty;

    /// <summary>
    /// Number of times searched
    /// </summary>
    public int SearchCount { get; set; } = 1;

    /// <summary>
    /// Number of unique users who searched this term
    /// </summary>
    public int UniqueUserCount { get; set; } = 1;

    /// <summary>
    /// Average result count for this term
    /// </summary>
    public double AverageResultCount { get; set; }

    /// <summary>
    /// Success rate (percentage of searches that found results)
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Language of the term
    /// </summary>
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Term category (if categorized)
    /// </summary>
    [MaxLength(50)]
    public string? Category { get; set; }

    /// <summary>
    /// First time this term was searched
    /// </summary>
    public DateTimeOffset FirstSearchedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Last time this term was searched
    /// </summary>
    public DateTimeOffset LastSearchedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When statistics were last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Whether term is trending (recently popular)
    /// </summary>
    public bool IsTrending { get; set; }

    /// <summary>
    /// Whether term should be suggested to users
    /// </summary>
    public bool IsSuggestion { get; set; } = true;
}