namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Performance trend data point
/// </summary>
public class PerformanceTrendDto
{
    public DateTime Timestamp { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}