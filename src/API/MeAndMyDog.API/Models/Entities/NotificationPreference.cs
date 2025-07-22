using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents user notification preferences
/// </summary>
public class NotificationPreference
{
    /// <summary>
    /// Preference unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID these preferences belong to
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Notification category/type
    /// </summary>
    [Required]
    [StringLength(50)]
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this notification type is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether to send push notifications
    /// </summary>
    public bool PushEnabled { get; set; } = true;

    /// <summary>
    /// Whether to send email notifications
    /// </summary>
    public bool EmailEnabled { get; set; } = true;

    /// <summary>
    /// Whether to send SMS notifications
    /// </summary>
    public bool SmsEnabled { get; set; } = false;

    /// <summary>
    /// Whether to show in-app notifications
    /// </summary>
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Custom sound for this notification type
    /// </summary>
    [StringLength(100)]
    public string? CustomSound { get; set; }

    /// <summary>
    /// Quiet hours start time (24-hour format, e.g., "22:00")
    /// </summary>
    [StringLength(5)]
    public string? QuietHoursStart { get; set; }

    /// <summary>
    /// Quiet hours end time (24-hour format, e.g., "08:00")
    /// </summary>
    [StringLength(5)]
    public string? QuietHoursEnd { get; set; }

    /// <summary>
    /// Time zone for quiet hours
    /// </summary>
    [StringLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Days of week when quiet hours apply (comma-separated)
    /// </summary>
    [StringLength(50)]
    public string? QuietHoursDays { get; set; }

    /// <summary>
    /// Minimum interval between notifications of this type (in minutes)
    /// </summary>
    public int? MinInterval { get; set; }

    /// <summary>
    /// Maximum number of notifications per day for this type
    /// </summary>
    public int? MaxPerDay { get; set; }

    /// <summary>
    /// Priority threshold (only send notifications above this priority)
    /// </summary>
    [StringLength(10)]
    public string? MinPriority { get; set; } = "normal";

    /// <summary>
    /// Custom configuration (JSON)
    /// </summary>
    public string? CustomConfig { get; set; }

    /// <summary>
    /// When preference was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When preference was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}