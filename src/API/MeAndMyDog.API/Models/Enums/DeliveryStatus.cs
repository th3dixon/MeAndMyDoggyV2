namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of notification delivery to a specific device
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Delivery is pending
    /// </summary>
    Pending,

    /// <summary>
    /// Delivery is in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Successfully delivered to push service
    /// </summary>
    Delivered,

    /// <summary>
    /// Notification was opened by user
    /// </summary>
    Opened,

    /// <summary>
    /// Delivery failed
    /// </summary>
    Failed,

    /// <summary>
    /// Device token is invalid
    /// </summary>
    InvalidToken,

    /// <summary>
    /// Delivery was rejected by push service
    /// </summary>
    Rejected,

    /// <summary>
    /// Delivery expired (TTL exceeded)
    /// </summary>
    Expired,

    /// <summary>
    /// Retrying delivery after failure
    /// </summary>
    Retrying
}