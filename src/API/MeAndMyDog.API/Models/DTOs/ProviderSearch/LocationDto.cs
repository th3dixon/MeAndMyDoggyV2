namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for location information
/// </summary>
public class LocationDto
{
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double Longitude { get; set; }
    
    /// <summary>
    /// Full address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// UK postcode
    /// </summary>
    public string? Postcode { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County or region
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Country (typically "United Kingdom")
    /// </summary>
    public string Country { get; set; } = "United Kingdom";
}