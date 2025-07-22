namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for translation suggestions
/// </summary>
public class TranslationSuggestionDto
{
    /// <summary>
    /// Suggested target language code
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// Suggested target language display name
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Reason for the suggestion
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score for this suggestion (0-1)
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Number of messages in this language in the conversation
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Whether this language is in user's preferences
    /// </summary>
    public bool IsUserPreferred { get; set; }

    /// <summary>
    /// Last time a message was translated to this language
    /// </summary>
    public DateTimeOffset? LastUsed { get; set; }
}