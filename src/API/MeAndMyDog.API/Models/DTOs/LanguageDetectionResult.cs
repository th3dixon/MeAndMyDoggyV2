namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Result of language detection
/// </summary>
public class LanguageDetectionResult
{
    /// <summary>
    /// Whether detection was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Detected language code (ISO 639-1)
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Detected language display name
    /// </summary>
    public string? LanguageName { get; set; }

    /// <summary>
    /// Detection confidence score (0-1)
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Alternative language detections
    /// </summary>
    public List<LanguageDetectionOption> Alternatives { get; set; } = new();

    /// <summary>
    /// Error message if detection failed
    /// </summary>
    public string? Error { get; set; }
}