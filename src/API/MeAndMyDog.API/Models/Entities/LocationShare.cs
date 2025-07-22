using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a shared location in a conversation
/// </summary>
public class LocationShare
{
    /// <summary>
    /// Unique identifier for the location share
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Message ID that contains this location share
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who shared the location
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation where location was shared
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
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
    /// Formatted address (if available)
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Place name or description
    /// </summary>
    [MaxLength(200)]
    public string? PlaceName { get; set; }

    /// <summary>
    /// Custom label for the location
    /// </summary>
    [MaxLength(100)]
    public string? Label { get; set; }

    /// <summary>
    /// Type of location share (current, static, live)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string LocationType { get; set; } = "static";

    /// <summary>
    /// Whether this is a live location that updates
    /// </summary>
    public bool IsLive { get; set; } = false;

    /// <summary>
    /// When live location sharing expires
    /// </summary>
    public DateTimeOffset? LiveExpiresAt { get; set; }

    /// <summary>
    /// How often live location updates (in seconds)
    /// </summary>
    public int? LiveUpdateIntervalSeconds { get; set; }

    /// <summary>
    /// Whether location is currently active for live sharing
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Privacy setting for who can see the location
    /// </summary>
    [MaxLength(20)]
    public string Visibility { get; set; } = "conversation";

    /// <summary>
    /// When the location was shared
    /// </summary>
    public DateTimeOffset SharedAt { get; set; }

    /// <summary>
    /// When the location data was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the message
    /// </summary>
    [ForeignKey(nameof(MessageId))]
    public Message Message { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who shared
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } = null!;

    /// <summary>
    /// Navigation property to location updates (for live sharing)
    /// </summary>
    public ICollection<LocationUpdate> LocationUpdates { get; set; } = new List<LocationUpdate>();
}