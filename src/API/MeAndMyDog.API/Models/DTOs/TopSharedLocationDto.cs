namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Top shared location information
/// </summary>
public class TopSharedLocationDto
{
    /// <summary>
    /// Location name or address
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Number of times shared
    /// </summary>
    public int ShareCount { get; set; }

    /// <summary>
    /// Last time this location was shared
    /// </summary>
    public DateTimeOffset LastShared { get; set; }
}