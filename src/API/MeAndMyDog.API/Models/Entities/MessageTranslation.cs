using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a message translation
/// </summary>
public class MessageTranslation
{
    /// <summary>
    /// Unique identifier for the translation
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Original message that was translated
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who requested the translation
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Language code of the source text (ISO 639-1)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Language code of the target translation (ISO 639-1)
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
    /// Translated text
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// Translation confidence score (0-1)
    /// </summary>
    public double? ConfidenceScore { get; set; }

    /// <summary>
    /// Translation provider used (Google, Azure, AWS, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TranslationProvider { get; set; } = string.Empty;

    /// <summary>
    /// Translation method (automatic, manual, cached)
    /// </summary>
    [MaxLength(20)]
    public string TranslationMethod { get; set; } = "automatic";

    /// <summary>
    /// Whether this translation was cached for reuse
    /// </summary>
    public bool IsCached { get; set; } = false;

    /// <summary>
    /// Number of characters translated
    /// </summary>
    public int CharacterCount { get; set; }

    /// <summary>
    /// Cost of translation in credits/tokens
    /// </summary>
    public decimal TranslationCost { get; set; } = 0;

    /// <summary>
    /// Translation quality rating (1-5) if provided by user
    /// </summary>
    public int? QualityRating { get; set; }

    /// <summary>
    /// User feedback on translation quality
    /// </summary>
    [MaxLength(500)]
    public string? QualityFeedback { get; set; }

    /// <summary>
    /// When the translation was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the translation was last accessed
    /// </summary>
    public DateTimeOffset? LastAccessedAt { get; set; }

    /// <summary>
    /// Number of times this translation has been accessed
    /// </summary>
    public int AccessCount { get; set; } = 0;

    /// <summary>
    /// Navigation property to the original message
    /// </summary>
    [ForeignKey(nameof(MessageId))]
    public Message Message { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who requested translation
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}