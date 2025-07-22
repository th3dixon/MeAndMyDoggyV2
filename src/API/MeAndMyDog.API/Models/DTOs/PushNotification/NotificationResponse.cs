using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for notification operations
/// </summary>
public class NotificationResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Notification ID if created
    /// </summary>
    public string? NotificationId { get; set; }

    /// <summary>
    /// Number of devices targeted
    /// </summary>
    public int DevicesTargeted { get; set; }

    /// <summary>
    /// Number of successful deliveries
    /// </summary>
    public int DeliveriesSuccessful { get; set; }

    /// <summary>
    /// Number of failed deliveries
    /// </summary>
    public int DeliveriesFailed { get; set; }
}