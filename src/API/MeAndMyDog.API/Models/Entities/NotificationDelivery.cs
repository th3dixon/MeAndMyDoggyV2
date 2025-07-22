using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a notification delivery attempt to a specific device
/// </summary>
public class NotificationDelivery
{
    /// <summary>
    /// Delivery unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Notification ID being delivered
    /// </summary>
    [Required]
    public string NotificationId { get; set; } = string.Empty;

    /// <summary>
    /// Device ID receiving the notification
    /// </summary>
    [Required]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Push service provider used (FCM, APNs, WebPush)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific message ID
    /// </summary>
    [StringLength(200)]
    public string? ProviderMessageId { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// When delivery was attempted
    /// </summary>
    public DateTimeOffset? AttemptedAt { get; set; }

    /// <summary>
    /// When delivery was successful
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; set; }

    /// <summary>
    /// When notification was opened/clicked
    /// </summary>
    public DateTimeOffset? OpenedAt { get; set; }

    /// <summary>
    /// Error code if delivery failed
    /// </summary>
    [StringLength(100)]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// HTTP status code from provider
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// Response body from provider
    /// </summary>
    public string? ProviderResponse { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Next retry time if failed
    /// </summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>
    /// When delivery record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When delivery record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Navigation property to notification
    /// </summary>
    public PushNotification Notification { get; set; } = null!;

    /// <summary>
    /// Navigation property to device
    /// </summary>
    public NotificationDevice Device { get; set; } = null!;
}