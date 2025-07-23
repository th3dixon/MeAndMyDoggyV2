namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// A/B testing insights
/// </summary>
public class ABTestInsightsDto
{
    public string TestName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public Dictionary<string, ABTestVariantDto> Variants { get; set; } = new();
    public string WinningVariant { get; set; } = string.Empty;
    public double ConfidenceLevel { get; set; }
    public Dictionary<string, double> ConversionRates { get; set; } = new();
    public int TotalParticipants { get; set; }
    public string StatisticalSignificance { get; set; } = string.Empty;
}