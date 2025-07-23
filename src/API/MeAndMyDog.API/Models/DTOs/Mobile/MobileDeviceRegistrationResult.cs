namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Device registration result
/// </summary>
public class MobileDeviceRegistrationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string DeviceRegistrationId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}