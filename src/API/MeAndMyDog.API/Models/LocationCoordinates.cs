namespace MeAndMyDog.API.Models;

/// <summary>
/// Represents geographical coordinates with location information
/// </summary>
public class LocationCoordinates
{
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public decimal Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public decimal Longitude { get; set; }
    
    /// <summary>
    /// City name (optional)
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County name (optional)
    /// </summary>
    public string? County { get; set; }
}