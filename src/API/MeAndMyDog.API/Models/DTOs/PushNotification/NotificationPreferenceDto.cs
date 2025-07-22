using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for notification preferences
/// </summary>
public class NotificationPreferenceDto
{
    /// <summary>
    /// Preference unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID these preferences belong to
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Notification category/type
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this notification type is enabled
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Whether to send push notifications
    /// </summary>
    public bool PushEnabled { get; set; }

    /// <summary>
    /// Whether to send email notifications
    /// </summary>
    public bool EmailEnabled { get; set; }

    /// <summary>
    /// Whether to send SMS notifications
    /// </summary>
    public bool SmsEnabled { get; set; }

    /// <summary>
    /// Whether to show in-app notifications
    /// </summary>
    public bool InAppEnabled { get; set; }

    /// <summary>
    /// Custom sound for this notification type
    /// </summary>
    public string? CustomSound { get; set; }

    /// <summary>
    /// Quiet hours configuration
    /// </summary>
    public QuietHoursDto? QuietHours { get; set; }

    /// <summary>
    /// Minimum interval between notifications (in minutes)
    /// </summary>
    public int? MinInterval { get; set; }

    /// <summary>
    /// Maximum notifications per day
    /// </summary>
    public int? MaxPerDay { get; set; }

    /// <summary>
    /// Minimum priority level
    /// </summary>
    public string MinPriority { get; set; } = "normal";

    /// <summary>
    /// When preference was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}