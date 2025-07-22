namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Statistics for a specific notification type or platform
/// </summary>
public class NotificationTypeStatistics
{
    /// <summary>
    /// Total sent for this category
    /// </summary>
    public int Sent { get; set; }

    /// <summary>
    /// Total delivered for this category
    /// </summary>
    public int Delivered { get; set; }

    /// <summary>
    /// Total opened for this category
    /// </summary>
    public int Opened { get; set; }

    /// <summary>
    /// Total failed for this category
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Delivery rate for this category
    /// </summary>
    public decimal DeliveryRate { get; set; }

    /// <summary>
    /// Open rate for this category
    /// </summary>
    public decimal OpenRate { get; set; }
}