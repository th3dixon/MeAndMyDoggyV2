using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create or update language preferences
/// </summary>
public class UpdateLanguagePreferenceRequest
{
    /// <summary>
    /// Primary language code (ISO 639-1)
    /// </summary>
    [Required]
    [StringLength(10, MinimumLength = 2)]
    public string PrimaryLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Secondary language codes (ISO 639-1)
    /// </summary>
    public List<string> SecondaryLanguages { get; set; } = new();

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
    public TranslationProvider PreferredProvider { get; set; } = TranslationProvider.Azure;

    /// <summary>
    /// Minimum confidence threshold for auto-translation (0-1)
    /// </summary>
    [Range(0.0, 1.0)]
    public double MinConfidenceThreshold { get; set; } = 0.7;

    /// <summary>
    /// Whether to show confidence scores
    /// </summary>
    public bool ShowConfidenceScores { get; set; } = false;

    /// <summary>
    /// Whether to enable translation caching
    /// </summary>
    public bool EnableTranslationCache { get; set; } = true;

    /// <summary>
    /// Languages to exclude from auto-translation
    /// </summary>
    public List<string> ExcludeLanguages { get; set; } = new();

    /// <summary>
    /// Whether to enable translation suggestions
    /// </summary>
    public bool EnableSuggestions { get; set; } = true;
}