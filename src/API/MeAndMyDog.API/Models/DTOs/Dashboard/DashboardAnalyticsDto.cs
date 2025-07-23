namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Dashboard analytics data
/// </summary>
public class DashboardAnalyticsDto
{
    public int TotalLogins { get; set; }
    public int WeeklyLogins { get; set; }
    public Dictionary<string, int> WidgetUsage { get; set; } = new();
    public List<string> MostUsedFeatures { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public Dictionary<string, double> ServiceUsageByCategory { get; set; } = new();
    public decimal MonthlySpending { get; set; }
    public int CompletedBookings { get; set; }
    public List<UsageTrendDto> UsageTrends { get; set; } = new();
}