using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for notification devices
/// </summary>
public class NotificationDeviceDto
{
    /// <summary>
    /// Device unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns this device
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Device token for push notifications
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Platform type (iOS, Android, Web)
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Device model/name
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// App version installed on device
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Operating system version
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// Device language/locale
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Device timezone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Whether device is active for notifications
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether user has enabled notifications on this device
    /// </summary>
    public bool NotificationsEnabled { get; set; }

    /// <summary>
    /// Last time device was seen/updated
    /// </summary>
    public DateTimeOffset LastSeenAt { get; set; }

    /// <summary>
    /// When device was registered
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}