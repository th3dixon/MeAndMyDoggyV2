namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile device registration data
/// </summary>
public class MobileDeviceRegistrationDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // iOS, Android
    public string AppVersion { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public Dictionary<string, bool> NotificationPermissions { get; set; } = new();
    public TimeZoneInfo? TimeZone { get; set; }
    public string? Language { get; set; }
}