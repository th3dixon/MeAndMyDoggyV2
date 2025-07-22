using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for push notifications
/// </summary>
public class PushNotificationDto
{
    /// <summary>
    /// Notification unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Target user ID
    /// </summary>
    public string? TargetUserId { get; set; }

    /// <summary>
    /// Target device ID
    /// </summary>
    public string? TargetDeviceId { get; set; }

    /// <summary>
    /// Notification type/category
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body/message
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Optional notification icon URL
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Optional notification image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Optional sound to play
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Notification badge count
    /// </summary>
    public int? Badge { get; set; }

    /// <summary>
    /// Custom data payload
    /// </summary>
    public Dictionary<string, object>? CustomData { get; set; }

    /// <summary>
    /// Action URL/deep link
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; set; } = "normal";

    /// <summary>
    /// Scheduled send time
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// When notification was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Overall notification status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// When notification was sent
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// Delivery information
    /// </summary>
    public List<NotificationDeliveryDto> Deliveries { get; set; } = new();
}