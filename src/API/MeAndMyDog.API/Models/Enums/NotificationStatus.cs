namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of a push notification
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Notification is queued and waiting to be sent
    /// </summary>
    Pending,

    /// <summary>
    /// Notification is being processed
    /// </summary>
    Processing,

    /// <summary>
    /// Notification has been sent successfully
    /// </summary>
    Sent,

    /// <summary>
    /// Notification was delivered to device
    /// </summary>
    Delivered,

    /// <summary>
    /// Notification was opened by user
    /// </summary>
    Opened,

    /// <summary>
    /// Notification failed to send
    /// </summary>
    Failed,

    /// <summary>
    /// Notification was cancelled before sending
    /// </summary>
    Cancelled,

    /// <summary>
    /// Notification expired (TTL exceeded)
    /// </summary>
    Expired,

    /// <summary>
    /// Notification is scheduled for later delivery
    /// </summary>
    Scheduled
}