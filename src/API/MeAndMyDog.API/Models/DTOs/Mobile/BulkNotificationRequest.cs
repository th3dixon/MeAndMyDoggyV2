namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Bulk notification request
/// </summary>
public class BulkNotificationRequest
{
    public List<string> UserIds { get; set; } = new();
    public MobilePushNotificationDto Notification { get; set; } = new();
    public Dictionary<string, string>? UserSpecificData { get; set; }
    public bool AllowDuplicates { get; set; } = false;
}