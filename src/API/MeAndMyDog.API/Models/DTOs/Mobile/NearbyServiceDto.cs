namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Nearby service data
/// </summary>
public class NearbyServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public double Distance { get; set; }
    public decimal? Price { get; set; }
    public double Rating { get; set; }
    public bool Available { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
}