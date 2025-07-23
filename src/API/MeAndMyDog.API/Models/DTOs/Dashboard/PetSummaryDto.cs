namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Pet summary for dashboard
/// </summary>
public class PetSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string Age { get; set; } = "Unknown age";
    public string? Image { get; set; }
    public string HealthStatus { get; set; } = "Unknown";
    public DateTimeOffset? LastCheckup { get; set; }
    public string LastCheckupFormatted { get; set; } = "No checkup recorded";
    public DateTimeOffset? NextAppointment { get; set; }
    public int PendingHealthActions { get; set; }
    public decimal? Weight { get; set; }
    public string VaccinationStatus { get; set; } = "Unknown";
    public int HealthScore { get; set; }
}