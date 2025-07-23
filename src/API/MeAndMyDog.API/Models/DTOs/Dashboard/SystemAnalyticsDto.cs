namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// System-wide analytics data
/// </summary>
public class SystemAnalyticsDto
{
    public int TotalActiveUsers { get; set; }
    public int NewUsersThisPeriod { get; set; }
    public Dictionary<string, int> WidgetPopularity { get; set; } = new();
    public Dictionary<string, double> FeatureAdoptionRates { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public List<UsageTrendDto> UserGrowthTrend { get; set; } = new();
    public Dictionary<string, int> DeviceBreakdown { get; set; } = new();
    public Dictionary<string, int> GeographicDistribution { get; set; } = new();
    public double CustomerSatisfactionScore { get; set; }
    public int TotalBookingsCompleted { get; set; }
    public decimal TotalRevenue { get; set; }
}