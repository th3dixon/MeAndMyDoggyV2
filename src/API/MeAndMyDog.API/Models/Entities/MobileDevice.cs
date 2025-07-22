namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Mobile device registration and management
/// </summary>
public class MobileDevice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string? NotificationPermissions { get; set; }
    public string Language { get; set; } = "en";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}