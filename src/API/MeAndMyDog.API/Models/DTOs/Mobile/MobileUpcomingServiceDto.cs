namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile upcoming service
/// </summary>
public class MobileUpcomingServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? PetName { get; set; }
}