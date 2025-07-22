using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for message translation
/// </summary>
public class MessageTranslationDto
{
    /// <summary>
    /// Translation ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Original message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who requested the translation
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Source language code
    /// </summary>
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Source language display name
    /// </summary>
    public string SourceLanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Target language code
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Target language display name
    /// </summary>
    public string TargetLanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Original text
    /// </summary>
    public string SourceText { get; set; } = string.Empty;

    /// <summary>
    /// Translated text
    /// </summary>
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// Translation confidence score (0-1)
    /// </summary>
    public double? ConfidenceScore { get; set; }

    /// <summary>
    /// Translation provider used
    /// </summary>
    public TranslationProvider TranslationProvider { get; set; }

    /// <summary>
    /// Translation method
    /// </summary>
    public TranslationMethod TranslationMethod { get; set; }

    /// <summary>
    /// Whether this translation was retrieved from cache
    /// </summary>
    public bool IsCached { get; set; }

    /// <summary>
    /// Character count of translated text
    /// </summary>
    public int CharacterCount { get; set; }

    /// <summary>
    /// Translation cost in credits
    /// </summary>
    public decimal TranslationCost { get; set; }

    /// <summary>
    /// User quality rating (1-5)
    /// </summary>
    public int? QualityRating { get; set; }

    /// <summary>
    /// User feedback on quality
    /// </summary>
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
    /// Number of times accessed
    /// </summary>
    public int AccessCount { get; set; }
}