using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for sending a push notification
/// </summary>
public class SendPushNotificationRequest
{
    /// <summary>
    /// Target user ID (optional - can be null for broadcast)
    /// </summary>
    public string? TargetUserId { get; set; }

    /// <summary>
    /// Target device ID (optional - sends to all user devices if null)
    /// </summary>
    public string? TargetDeviceId { get; set; }

    /// <summary>
    /// Notification type/category
    /// </summary>
    [Required]
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
    /// Priority level (high, normal, low)
    /// </summary>
    public string Priority { get; set; } = "normal";

    /// <summary>
    /// Scheduled send time (null for immediate)
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }
}