using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for user language preferences
/// </summary>
public class LanguagePreferenceDto
{
    /// <summary>
    /// Preference ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Primary language code
    /// </summary>
    public string PrimaryLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Primary language display name
    /// </summary>
    public string PrimaryLanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Secondary language codes
    /// </summary>
    public List<string> SecondaryLanguages { get; set; } = new();

    /// <summary>
    /// Secondary language display names
    /// </summary>
    public List<string> SecondaryLanguageNames { get; set; } = new();

    /// <summary>
    /// Whether to auto-translate incoming messages
    /// </summary>
    public bool AutoTranslateIncoming { get; set; }

    /// <summary>
    /// Whether to auto-detect language of outgoing messages
    /// </summary>
    public bool AutoDetectOutgoing { get; set; }

    /// <summary>
    /// Preferred translation provider
    /// </summary>
    public TranslationProvider PreferredProvider { get; set; }

    /// <summary>
    /// Minimum confidence threshold for auto-translation
    /// </summary>
    public double MinConfidenceThreshold { get; set; }

    /// <summary>
    /// Whether to show confidence scores
    /// </summary>
    public bool ShowConfidenceScores { get; set; }

    /// <summary>
    /// Whether translation caching is enabled
    /// </summary>
    public bool EnableTranslationCache { get; set; }

    /// <summary>
    /// Languages to exclude from auto-translation
    /// </summary>
    public List<string> ExcludeLanguages { get; set; } = new();

    /// <summary>
    /// Whether translation suggestions are enabled
    /// </summary>
    public bool EnableSuggestions { get; set; }

    /// <summary>
    /// Whether this preference is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the preference was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the preference was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}

