using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing an update to a live location share
/// </summary>
public class LocationUpdate
{
    /// <summary>
    /// Unique identifier for the location update
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Location share this update belongs to
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string LocationShareId { get; set; } = string.Empty;

    /// <summary>
    /// Updated latitude coordinate
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// Updated longitude coordinate
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    /// <summary>
    /// Location accuracy in meters
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Altitude in meters (optional)
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// Speed in meters per second (if available)
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Bearing/heading in degrees (if available)
    /// </summary>
    public double? Bearing { get; set; }

    /// <summary>
    /// Battery level of the device when location was captured
    /// </summary>
    public int? BatteryLevel { get; set; }

    /// <summary>
    /// Source of location data (GPS, Network, Passive)
    /// </summary>
    [MaxLength(20)]
    public string? LocationSource { get; set; }

    /// <summary>
    /// When this location update was captured
    /// </summary>
    public DateTimeOffset CapturedAt { get; set; }

    /// <summary>
    /// When this update was received by the server
    /// </summary>
    public DateTimeOffset ReceivedAt { get; set; }

    /// <summary>
    /// Navigation property to the location share
    /// </summary>
    [ForeignKey(nameof(LocationShareId))]
    public LocationShare LocationShare { get; set; } = null!;
}