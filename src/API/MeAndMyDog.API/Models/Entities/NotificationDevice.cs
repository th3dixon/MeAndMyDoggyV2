using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a user device for push notifications
/// </summary>
public class NotificationDevice
{
    /// <summary>
    /// Device unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns this device
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Device token for push notifications
    /// </summary>
    [Required]
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Platform type (iOS, Android, Web)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Device model/name
    /// </summary>
    [StringLength(200)]
    public string? DeviceName { get; set; }

    /// <summary>
    /// App version installed on device
    /// </summary>
    [StringLength(50)]
    public string? AppVersion { get; set; }

    /// <summary>
    /// Operating system version
    /// </summary>
    [StringLength(50)]
    public string? OsVersion { get; set; }

    /// <summary>
    /// Device language/locale
    /// </summary>
    [StringLength(10)]
    public string? Language { get; set; } = "en";

    /// <summary>
    /// Device timezone
    /// </summary>
    [StringLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Whether device is active for notifications
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether user has enabled notifications on this device
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Last time device was seen/updated
    /// </summary>
    public DateTimeOffset LastSeenAt { get; set; }

    /// <summary>
    /// When device was registered
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When device was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to notification deliveries
    /// </summary>
    public ICollection<NotificationDelivery> NotificationDeliveries { get; set; } = new List<NotificationDelivery>();
}