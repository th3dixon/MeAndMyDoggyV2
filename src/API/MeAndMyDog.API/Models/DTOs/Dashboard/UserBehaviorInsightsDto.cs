namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// User behavior insights
/// </summary>
public class UserBehaviorInsightsDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> PreferredWidgets { get; set; } = new();
    public Dictionary<string, int> ServicePreferences { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public List<string> PeakUsageHours { get; set; } = new();
    public Dictionary<string, double> ConversionRates { get; set; } = new();
    public string UserSegment { get; set; } = "Standard";
    public double EngagementScore { get; set; }
    public List<BehaviorPatternDto> BehaviorPatterns { get; set; } = new();
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}