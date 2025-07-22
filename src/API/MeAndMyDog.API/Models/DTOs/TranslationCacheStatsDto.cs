namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for translation cache statistics
/// </summary>
public class TranslationCacheStatsDto
{
    /// <summary>
    /// Total number of cache entries
    /// </summary>
    public int TotalCacheEntries { get; set; }

    /// <summary>
    /// Number of active cache entries
    /// </summary>
    public int ActiveCacheEntries { get; set; }

    /// <summary>
    /// Total cache hits
    /// </summary>
    public long TotalCacheHits { get; set; }

    /// <summary>
    /// Cache hit ratio (0-1)
    /// </summary>
    public double CacheHitRatio { get; set; }

    /// <summary>
    /// Total characters cached
    /// </summary>
    public long TotalCharactersCached { get; set; }

    /// <summary>
    /// Average quality rating of cached translations
    /// </summary>
    public double AverageCacheQuality { get; set; }

    /// <summary>
    /// Most frequently used language pairs in cache
    /// </summary>
    public List<CacheLanguagePairDto> TopLanguagePairs { get; set; } = new();

    /// <summary>
    /// Cache entries by provider
    /// </summary>
    public List<CacheProviderStatsDto> ProviderStats { get; set; } = new();

    /// <summary>
    /// When cache statistics were calculated
    /// </summary>
    public DateTimeOffset CalculatedAt { get; set; }
}