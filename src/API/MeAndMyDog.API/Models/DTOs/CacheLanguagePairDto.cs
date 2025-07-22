namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Language pair statistics in cache
/// </summary>
public class CacheLanguagePairDto
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
    /// Number of cached translations for this pair
    /// </summary>
    public int CacheEntries { get; set; }

    /// <summary>
    /// Total usage count for this pair
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Average quality rating
    /// </summary>
    public double AverageQuality { get; set; }
}