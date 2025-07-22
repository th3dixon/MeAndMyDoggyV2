using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update live location
/// </summary>
public class UpdateLiveLocationRequest
{
    /// <summary>
    /// Location share ID to update
    /// </summary>
    [Required]
    public string LocationShareId { get; set; } = string.Empty;

    /// <summary>
    /// Updated latitude coordinate
    /// </summary>
    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// Updated longitude coordinate
    /// </summary>
    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// Location accuracy in meters
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? Accuracy { get; set; }

    /// <summary>
    /// Altitude in meters
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// Speed in meters per second
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? Speed { get; set; }

    /// <summary>
    /// Bearing/heading in degrees
    /// </summary>
    [Range(0, 360)]
    public double? Bearing { get; set; }

    /// <summary>
    /// Device battery level (0-100)
    /// </summary>
    [Range(0, 100)]
    public int? BatteryLevel { get; set; }

    /// <summary>
    /// Location data source (GPS, Network, Passive)
    /// </summary>
    [MaxLength(20)]
    public string? LocationSource { get; set; }

    /// <summary>
    /// When this location was captured on the device
    /// </summary>
    public DateTimeOffset? CapturedAt { get; set; }
}