namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Behavior pattern data
/// </summary>
public class BehaviorPatternDto
{
    public string PatternName { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime LastObserved { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}