namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Push notification result
/// </summary>
public class PushNotificationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string NotificationId { get; set; } = string.Empty;
    public int DevicesReached { get; set; }
    public List<string> FailedDevices { get; set; } = new();
}