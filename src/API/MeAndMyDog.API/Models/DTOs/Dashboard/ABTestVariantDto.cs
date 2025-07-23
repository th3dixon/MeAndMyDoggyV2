namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// A/B test variant data
/// </summary>
public class ABTestVariantDto
{
    public string Name { get; set; } = string.Empty;
    public int Participants { get; set; }
    public int Conversions { get; set; }
    public double ConversionRate { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
}