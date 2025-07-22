using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Translation provider usage statistics
/// </summary>
public class ProviderStatsDto
{
    /// <summary>
    /// Translation provider
    /// </summary>
    public TranslationProvider Provider { get; set; }

    /// <summary>
    /// Provider name
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// Number of translations using this provider
    /// </summary>
    public int TranslationCount { get; set; }

    /// <summary>
    /// Total characters translated by this provider
    /// </summary>
    public long CharactersTranslated { get; set; }

    /// <summary>
    /// Total cost for this provider
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average confidence score for this provider
    /// </summary>
    public double AverageConfidence { get; set; }

    /// <summary>
    /// Average quality rating for this provider
    /// </summary>
    public double AverageQuality { get; set; }

    /// <summary>
    /// Success rate for this provider (0-1)
    /// </summary>
    public double SuccessRate { get; set; }
}