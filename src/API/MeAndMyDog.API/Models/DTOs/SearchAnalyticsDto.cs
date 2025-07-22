namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Search analytics data
/// </summary>
public class SearchAnalyticsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of searches performed
    /// </summary>
    public int TotalSearches { get; set; }

    /// <summary>
    /// Number of unique search queries
    /// </summary>
    public int UniqueQueries { get; set; }

    /// <summary>
    /// Average results per search
    /// </summary>
    public double AverageResults { get; set; }

    /// <summary>
    /// Average search execution time (ms)
    /// </summary>
    public double AverageExecutionTime { get; set; }

    /// <summary>
    /// Most active search hour (0-23)
    /// </summary>
    public int MostActiveHour { get; set; }

    /// <summary>
    /// Most active day of week (0=Sunday, 6=Saturday)
    /// </summary>
    public int MostActiveDayOfWeek { get; set; }

    /// <summary>
    /// Percentage of searches that had interactions
    /// </summary>
    public double InteractionRate { get; set; }

    /// <summary>
    /// Top search queries with counts
    /// </summary>
    public List<PopularQueryDto> TopQueries { get; set; } = new();

    /// <summary>
    /// Search volume by day
    /// </summary>
    public List<SearchVolumeByDateDto> VolumeByDay { get; set; } = new();

    /// <summary>
    /// Date range for analytics
    /// </summary>
    public DateTimeOffset FromDate { get; set; }

    /// <summary>
    /// Date range end for analytics
    /// </summary>
    public DateTimeOffset ToDate { get; set; }
}