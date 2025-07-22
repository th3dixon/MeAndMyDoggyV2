using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity for caching common translations to improve performance and reduce costs
/// </summary>
public class TranslationCache
{
    /// <summary>
    /// Unique identifier for the cached translation
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the source text for quick lookup
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string TextHash { get; set; } = string.Empty;

    /// <summary>
    /// Source language code (ISO 639-1)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Target language code (ISO 639-1)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Original text that was translated
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string SourceText { get; set; } = string.Empty;

    /// <summary>
    /// Cached translated text
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// Translation confidence score (0-1)
    /// </summary>
    public double? ConfidenceScore { get; set; }

    /// <summary>
    /// Translation provider that created this cache entry
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TranslationProvider { get; set; } = string.Empty;

    /// <summary>
    /// Number of characters in source text
    /// </summary>
    public int CharacterCount { get; set; }

    /// <summary>
    /// Number of times this cached translation has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// Average quality rating from users (1-5)
    /// </summary>
    public double? AverageQualityRating { get; set; }

    /// <summary>
    /// Number of quality ratings received
    /// </summary>
    public int QualityRatingCount { get; set; } = 0;

    /// <summary>
    /// When the cache entry was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the cache entry was last used
    /// </summary>
    public DateTimeOffset LastUsedAt { get; set; }

    /// <summary>
    /// When the cache entry expires (if applicable)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Whether this cache entry is still valid
    /// </summary>
    public bool IsActive { get; set; } = true;
}