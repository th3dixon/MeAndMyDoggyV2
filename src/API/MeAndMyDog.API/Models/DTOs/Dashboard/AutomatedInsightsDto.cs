namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Automated insights
/// </summary>
public class AutomatedInsightsDto
{
    public List<InsightDto> Insights { get; set; } = new();
    public List<AnomalyDto> Anomalies { get; set; } = new();
    public List<OpportunityDto> Opportunities { get; set; } = new();
    public List<AlertDto> Alerts { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string InsightsVersion { get; set; } = "1.0";
}