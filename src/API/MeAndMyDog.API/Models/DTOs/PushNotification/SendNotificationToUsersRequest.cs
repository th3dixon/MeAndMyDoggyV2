namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for sending notification to multiple users
/// </summary>
public class SendNotificationToUsersRequest : SendPushNotificationRequest
{
    /// <summary>
    /// List of target user IDs
    /// </summary>
    public List<string> TargetUserIds { get; set; } = new();
}