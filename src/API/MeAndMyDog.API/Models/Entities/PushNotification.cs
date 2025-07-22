using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a push notification message
/// </summary>
public class PushNotification
{
    /// <summary>
    /// Notification unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Target user ID (optional - can be null for broadcast)
    /// </summary>
    public string? TargetUserId { get; set; }

    /// <summary>
    /// Target device ID (optional - can be null to send to all user devices)
    /// </summary>
    public string? TargetDeviceId { get; set; }

    /// <summary>
    /// Notification type/category
    /// </summary>
    [Required]
    [StringLength(50)]
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body/message
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Optional notification icon URL
    /// </summary>
    [StringLength(500)]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Optional notification image URL
    /// </summary>
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Optional sound to play
    /// </summary>
    [StringLength(100)]
    public string? Sound { get; set; } = "default";

    /// <summary>
    /// Notification badge count
    /// </summary>
    public int? Badge { get; set; }

    /// <summary>
    /// Custom data payload (JSON)
    /// </summary>
    public string? CustomData { get; set; }

    /// <summary>
    /// Action URL/deep link
    /// </summary>
    [StringLength(500)]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Priority level (high, normal, low)
    /// </summary>
    [StringLength(10)]
    public string Priority { get; set; } = "normal";

    /// <summary>
    /// Time to live in seconds
    /// </summary>
    public int? TimeToLive { get; set; } = 3600; // 1 hour default

    /// <summary>
    /// Scheduled send time (null for immediate)
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// When notification was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Who created the notification (system or user ID)
    /// </summary>
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Overall notification status
    /// </summary>
    [StringLength(50)]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// When notification was sent
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Next retry time if failed
    /// </summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>
    /// Navigation property to target user
    /// </summary>
    public ApplicationUser? TargetUser { get; set; }

    /// <summary>
    /// Navigation property to target device
    /// </summary>
    public NotificationDevice? TargetDevice { get; set; }

    /// <summary>
    /// Navigation property to notification deliveries
    /// </summary>
    public ICollection<NotificationDelivery> NotificationDeliveries { get; set; } = new List<NotificationDelivery>();
}