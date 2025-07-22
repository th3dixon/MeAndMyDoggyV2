namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for provider location information
/// </summary>
public class ProviderLocationDto
{
    /// <summary>
    /// Postcode
    /// </summary>
    public string Postcode { get; set; } = string.Empty;
    
    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Service radius in miles
    /// </summary>
    public int ServiceRadiusMiles { get; set; }
    
    /// <summary>
    /// Latitude (optional for privacy)
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitude (optional for privacy)
    /// </summary>
    public double? Longitude { get; set; }
}