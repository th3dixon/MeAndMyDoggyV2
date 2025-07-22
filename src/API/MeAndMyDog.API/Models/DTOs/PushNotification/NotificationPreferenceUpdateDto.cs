using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Individual preference update
/// </summary>
public class NotificationPreferenceUpdateDto
{
    /// <summary>
    /// Notification type/category
    /// </summary>
    [Required]
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
}