using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for notification delivery
/// </summary>
public class NotificationDeliveryDto
{
    /// <summary>
    /// Delivery unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Device ID receiving the notification
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Device information
    /// </summary>
    public NotificationDeviceDto? Device { get; set; }

    /// <summary>
    /// Push service provider used
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific message ID
    /// </summary>
    public string? ProviderMessageId { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public string Status { get; set; } = string.Empty;

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
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }
}