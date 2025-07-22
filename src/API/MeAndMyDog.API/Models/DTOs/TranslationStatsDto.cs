using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for translation statistics
/// </summary>
public class TranslationStatsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of translations performed
    /// </summary>
    public int TotalTranslations { get; set; }

    /// <summary>
    /// Number of cached translations used
    /// </summary>
    public int CachedTranslations { get; set; }

    /// <summary>
    /// Number of automatic translations
    /// </summary>
    public int AutomaticTranslations { get; set; }

    /// <summary>
    /// Number of manual translations
    /// </summary>
    public int ManualTranslations { get; set; }

    /// <summary>
    /// Total characters translated
    /// </summary>
    public long TotalCharactersTranslated { get; set; }

    /// <summary>
    /// Total translation cost
    /// </summary>
    public decimal TotalTranslationCost { get; set; }

    /// <summary>
    /// Average translation confidence score
    /// </summary>
    public double AverageConfidenceScore { get; set; }

    /// <summary>
    /// Average quality rating from user feedback
    /// </summary>
    public double AverageQualityRating { get; set; }

    /// <summary>
    /// Most frequently used source language
    /// </summary>
    public string MostUsedSourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Most frequently used target language
    /// </summary>
    public string MostUsedTargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Most used translation provider
    /// </summary>
    public TranslationProvider MostUsedProvider { get; set; }

    /// <summary>
    /// Cache hit ratio (0-1)
    /// </summary>
    public double CacheHitRatio { get; set; }

    /// <summary>
    /// Statistics date range start
    /// </summary>
    public DateTimeOffset FromDate { get; set; }

    /// <summary>
    /// Statistics date range end
    /// </summary>
    public DateTimeOffset ToDate { get; set; }

    /// <summary>
    /// Translation activity by day
    /// </summary>
    public List<TranslationDayStatsDto> DailyStats { get; set; } = new();

    /// <summary>
    /// Top language pairs used
    /// </summary>
    public List<LanguagePairStatsDto> TopLanguagePairs { get; set; } = new();

    /// <summary>
    /// Provider usage statistics
    /// </summary>
    public List<ProviderStatsDto> ProviderStats { get; set; } = new();
}