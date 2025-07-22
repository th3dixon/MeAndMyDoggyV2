namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Cache statistics by provider
/// </summary>
public class CacheProviderStatsDto
{
    /// <summary>
    /// Provider name
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Number of cache entries from this provider
    /// </summary>
    public int CacheEntries { get; set; }

    /// <summary>
    /// Total usage count for this provider's cached translations
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Average quality rating for this provider's cached translations
    /// </summary>
    public double AverageQuality { get; set; }
}