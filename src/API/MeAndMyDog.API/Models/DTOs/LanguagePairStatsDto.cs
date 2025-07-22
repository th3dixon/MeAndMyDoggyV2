namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Language pair usage statistics
/// </summary>
public class LanguagePairStatsDto
{
    /// <summary>
    /// Source language code
    /// </summary>
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Target language code
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Source language display name
    /// </summary>
    public string SourceLanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Target language display name
    /// </summary>
    public string TargetLanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Number of translations for this pair
    /// </summary>
    public int TranslationCount { get; set; }

    /// <summary>
    /// Average confidence score for this pair
    /// </summary>
    public double AverageConfidence { get; set; }

    /// <summary>
    /// Average quality rating for this pair
    /// </summary>
    public double AverageQuality { get; set; }

    /// <summary>
    /// Last time this pair was used
    /// </summary>
    public DateTimeOffset LastUsed { get; set; }
}