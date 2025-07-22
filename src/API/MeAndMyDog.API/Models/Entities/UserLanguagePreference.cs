using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing user language preferences for translation
/// </summary>
public class UserLanguagePreference
{
    /// <summary>
    /// Unique identifier for the language preference
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User this preference belongs to
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Primary language code (ISO 639-1)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string PrimaryLanguage { get; set; } = "en";

    /// <summary>
    /// Secondary languages (comma-separated ISO 639-1 codes)
    /// </summary>
    [MaxLength(200)]
    public string? SecondaryLanguages { get; set; }

    /// <summary>
    /// Whether to auto-translate incoming messages
    /// </summary>
    public bool AutoTranslateIncoming { get; set; } = false;

    /// <summary>
    /// Whether to auto-detect language of outgoing messages
    /// </summary>
    public bool AutoDetectOutgoing { get; set; } = true;

    /// <summary>
    /// Preferred translation provider
    /// </summary>
    [MaxLength(50)]
    public string PreferredProvider { get; set; } = "Azure";

    /// <summary>
    /// Minimum confidence threshold for auto-translation (0-1)
    /// </summary>
    public double MinConfidenceThreshold { get; set; } = 0.7;

    /// <summary>
    /// Whether to show translation confidence scores
    /// </summary>
    public bool ShowConfidenceScores { get; set; } = false;

    /// <summary>
    /// Whether to cache translations for reuse
    /// </summary>
    public bool EnableTranslationCache { get; set; } = true;

    /// <summary>
    /// Languages to never auto-translate from (comma-separated)
    /// </summary>
    [MaxLength(100)]
    public string? ExcludeLanguages { get; set; }

    /// <summary>
    /// Whether user wants translation suggestions
    /// </summary>
    public bool EnableSuggestions { get; set; } = true;

    /// <summary>
    /// Whether this preference is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the preference was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the preference was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}