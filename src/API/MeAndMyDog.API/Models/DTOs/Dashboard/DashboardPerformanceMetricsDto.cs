namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Dashboard performance metrics
/// </summary>
public class DashboardPerformanceMetricsDto
{
    public Dictionary<string, double> AverageLoadTimes { get; set; } = new();
    public Dictionary<string, int> ErrorRates { get; set; } = new();
    public Dictionary<string, double> CacheHitRatios { get; set; } = new();
    public List<PerformanceTrendDto> PerformanceTrends { get; set; } = new();
    public Dictionary<string, int> APICallVolumes { get; set; } = new();
    public double SystemUptime { get; set; }
    public int TotalErrors { get; set; }
    public Dictionary<string, double> DatabaseQueryPerformance { get; set; } = new();
}