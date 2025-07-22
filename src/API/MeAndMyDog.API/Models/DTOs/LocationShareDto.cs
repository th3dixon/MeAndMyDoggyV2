using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for location sharing information
/// </summary>
public class LocationShareDto
{
    /// <summary>
    /// Location share ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Message ID containing this location
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User ID who shared the location
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// User display name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
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
    /// Formatted address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Place name
    /// </summary>
    public string? PlaceName { get; set; }

    /// <summary>
    /// Custom label
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Type of location share
    /// </summary>
    public LocationShareType LocationType { get; set; }

    /// <summary>
    /// Whether this is live location
    /// </summary>
    public bool IsLive { get; set; }

    /// <summary>
    /// Live location expiry
    /// </summary>
    public DateTimeOffset? LiveExpiresAt { get; set; }

    /// <summary>
    /// Update interval in seconds
    /// </summary>
    public int? LiveUpdateIntervalSeconds { get; set; }

    /// <summary>
    /// Visibility setting
    /// </summary>
    public LocationVisibility Visibility { get; set; }

    /// <summary>
    /// Whether location is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When location was shared
    /// </summary>
    public DateTimeOffset SharedAt { get; set; }

    /// <summary>
    /// Last update time
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Recent location updates (for live sharing)
    /// </summary>
    public List<LocationUpdateDto> RecentUpdates { get; set; } = new();

    /// <summary>
    /// Distance from current user's location (if available)
    /// </summary>
    public double? DistanceFromUser { get; set; }
}