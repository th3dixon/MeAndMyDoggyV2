namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Cache statistics
/// </summary>
public class DashboardCacheStatsDto
{
    public long TotalKeys { get; set; }
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public double HitRatio { get; set; }
    public Dictionary<string, long> KeysByType { get; set; } = new();
    public long MemoryUsageBytes { get; set; }
}