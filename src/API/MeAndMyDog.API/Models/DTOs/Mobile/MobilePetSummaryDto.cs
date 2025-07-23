namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile pet summary
/// </summary>
public class MobilePetSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string HealthStatus { get; set; } = "Unknown";
    public string? NextAppointment { get; set; }
    public List<string> HealthAlerts { get; set; } = new();
}