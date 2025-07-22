namespace MeAndMyDog.API.DTOs.Location;

/// <summary>
/// Response object for geocoding operations
/// </summary>
public class GeocodeResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Address (for reverse geocoding) or input address (for forward geocoding)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double? Longitude { get; set; }
}