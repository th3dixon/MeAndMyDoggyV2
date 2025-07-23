namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Anomaly detection
/// </summary>
public class AnomalyDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MetricName { get; set; } = string.Empty;
    public double ExpectedValue { get; set; }
    public double ActualValue { get; set; }
    public double DeviationScore { get; set; }
    public string Severity { get; set; } = "Medium";
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public string PossibleCause { get; set; } = string.Empty;
    public List<string> SuggestedInvestigations { get; set; } = new();
}