using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to share a location
/// </summary>
public class ShareLocationRequest
{
    /// <summary>
    /// Conversation to share location in
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
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
    /// Custom label for the location
    /// </summary>
    [MaxLength(100)]
    public string? Label { get; set; }

    /// <summary>
    /// Type of location share
    /// </summary>
    public LocationShareType LocationType { get; set; } = LocationShareType.Current;

    /// <summary>
    /// Whether to enable live location sharing
    /// </summary>
    public bool EnableLiveSharing { get; set; } = false;

    /// <summary>
    /// How long to share live location (in minutes)
    /// </summary>
    [Range(1, 480, ErrorMessage = "Live sharing duration must be between 1 and 480 minutes")]
    public int? LiveSharingDurationMinutes { get; set; }

    /// <summary>
    /// Live location update interval (in seconds)
    /// </summary>
    [Range(30, 300, ErrorMessage = "Update interval must be between 30 and 300 seconds")]
    public int? LiveUpdateIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Visibility setting
    /// </summary>
    public LocationVisibility Visibility { get; set; } = LocationVisibility.Conversation;

    /// <summary>
    /// Optional message content to send with location
    /// </summary>
    [MaxLength(1000)]
    public string? MessageContent { get; set; }

    /// <summary>
    /// Bookmark ID if sharing a saved location
    /// </summary>
    public string? BookmarkId { get; set; }
}