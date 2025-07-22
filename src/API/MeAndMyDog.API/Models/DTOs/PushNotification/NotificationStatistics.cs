namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Notification statistics for admin dashboard
/// </summary>
public class NotificationStatistics
{
    /// <summary>
    /// Total notifications sent
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Total notifications delivered
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Total notifications opened
    /// </summary>
    public int TotalOpened { get; set; }

    /// <summary>
    /// Total notifications failed
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Delivery rate percentage
    /// </summary>
    public decimal DeliveryRate { get; set; }

    /// <summary>
    /// Open rate percentage
    /// </summary>
    public decimal OpenRate { get; set; }

    /// <summary>
    /// Statistics by notification type
    /// </summary>
    public Dictionary<string, NotificationTypeStatistics> ByType { get; set; } = new();

    /// <summary>
    /// Statistics by platform
    /// </summary>
    public Dictionary<string, NotificationTypeStatistics> ByPlatform { get; set; } = new();

    /// <summary>
    /// Daily statistics for the reporting period
    /// </summary>
    public List<DailyNotificationStatistics> DailyStats { get; set; } = new();
}