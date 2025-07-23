using MeAndMyDog.API.Models.DTOs.Mobile;

namespace MeAndMyDog.API.Models.DTOs.MobileIntegration;

/// <summary>
/// Send notification request
/// </summary>
public class SendNotificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public MobilePushNotificationDto Notification { get; set; } = new();
}