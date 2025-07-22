namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Priority levels for push notifications
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Low priority - may be delivered with delay
    /// </summary>
    Low,

    /// <summary>
    /// Normal priority - standard delivery
    /// </summary>
    Normal,

    /// <summary>
    /// High priority - immediate delivery, may override quiet hours
    /// </summary>
    High,

    /// <summary>
    /// Critical priority - emergency notifications
    /// </summary>
    Critical
}