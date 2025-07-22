namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Alternative language detection option
/// </summary>
public class LanguageDetectionOption
{
    /// <summary>
    /// Language code (ISO 639-1)
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// Language display name
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Detection confidence score (0-1)
    /// </summary>
    public double ConfidenceScore { get; set; }
}