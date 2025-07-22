namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for location updates in live sharing
/// </summary>
public class LocationUpdateDto
{
    /// <summary>
    /// Update ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Location share ID
    /// </summary>
    public string LocationShareId { get; set; } = string.Empty;

    /// <summary>
    /// Updated latitude
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Updated longitude
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Location accuracy in meters
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Altitude in meters
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// Speed in meters per second
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Bearing/heading in degrees
    /// </summary>
    public double? Bearing { get; set; }

    /// <summary>
    /// Battery level percentage
    /// </summary>
    public int? BatteryLevel { get; set; }

    /// <summary>
    /// Location data source
    /// </summary>
    public string? LocationSource { get; set; }

    /// <summary>
    /// When location was captured
    /// </summary>
    public DateTimeOffset CapturedAt { get; set; }

    /// <summary>
    /// When update was received
    /// </summary>
    public DateTimeOffset ReceivedAt { get; set; }
}