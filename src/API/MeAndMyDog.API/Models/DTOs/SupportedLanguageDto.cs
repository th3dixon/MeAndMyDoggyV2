using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Information about a supported language
/// </summary>
public class SupportedLanguageDto
{
    /// <summary>
    /// Language code (ISO 639-1)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Language name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Language name in native language
    /// </summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>
    /// Language direction (ltr or rtl)
    /// </summary>
    public string Direction { get; set; } = "ltr";

    /// <summary>
    /// Supported translation providers for this language
    /// </summary>
    public List<TranslationProvider> SupportedProviders { get; set; } = new();

    /// <summary>
    /// Whether this language supports auto-detection
    /// </summary>
    public bool SupportsAutoDetection { get; set; } = true;

    /// <summary>
    /// Quality score for translations to/from this language (0-1)
    /// </summary>
    public double QualityScore { get; set; } = 1.0;

    /// <summary>
    /// Whether this language is commonly used
    /// </summary>
    public bool IsCommon { get; set; } = false;
}