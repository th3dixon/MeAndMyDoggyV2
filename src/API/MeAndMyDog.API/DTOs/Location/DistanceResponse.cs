namespace MeAndMyDog.API.DTOs.Location;

/// <summary>
/// Response object for distance calculation
/// </summary>
public class DistanceResponse
{
    /// <summary>
    /// Distance in meters
    /// </summary>
    public double DistanceMeters { get; set; }

    /// <summary>
    /// Distance in kilometers
    /// </summary>
    public double DistanceKilometers { get; set; }

    /// <summary>
    /// Distance in miles
    /// </summary>
    public double DistanceMiles { get; set; }
}