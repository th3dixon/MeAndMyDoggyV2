namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Search index statistics
/// </summary>
public class SearchIndexStatsDto
{
    /// <summary>
    /// Total number of indexed messages
    /// </summary>
    public long IndexedMessages { get; set; }

    /// <summary>
    /// Total number of conversations indexed
    /// </summary>
    public long IndexedConversations { get; set; }

    /// <summary>
    /// Index size in bytes
    /// </summary>
    public long IndexSizeBytes { get; set; }

    /// <summary>
    /// Last time index was updated
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// Average search response time in milliseconds
    /// </summary>
    public double AverageSearchTimeMs { get; set; }

    /// <summary>
    /// Number of pending indexing operations
    /// </summary>
    public int PendingOperations { get; set; }

    /// <summary>
    /// Index health status
    /// </summary>
    public string HealthStatus { get; set; } = "healthy";

    /// <summary>
    /// Memory usage by index in MB
    /// </summary>
    public double MemoryUsageMB { get; set; }
}