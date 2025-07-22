using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for registering a device
/// </summary>
public class RegisterDeviceRequest
{
    /// <summary>
    /// Device token for push notifications
    /// </summary>
    [Required]
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Platform type (iOS, Android, Web)
    /// </summary>
    [Required]
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
    public string Language { get; set; } = "en";

    /// <summary>
    /// Device timezone
    /// </summary>
    public string? TimeZone { get; set; }
}