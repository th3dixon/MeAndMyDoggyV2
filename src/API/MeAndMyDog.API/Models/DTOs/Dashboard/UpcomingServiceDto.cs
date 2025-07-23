namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Upcoming service summary
/// </summary>
public class UpcomingServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceCategory { get; set; } = string.Empty;
    public string ServiceCategoryIcon { get; set; } = "fas fa-paw";
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderImage { get; set; }
    public double ProviderRating { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Pet { get; set; } = string.Empty;
    public string? PetBreed { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}