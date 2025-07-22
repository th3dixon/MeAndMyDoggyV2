namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Daily notification statistics
/// </summary>
public class DailyNotificationStatistics
{
    /// <summary>
    /// Date for these statistics
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Total sent on this date
    /// </summary>
    public int Sent { get; set; }

    /// <summary>
    /// Total delivered on this date
    /// </summary>
    public int Delivered { get; set; }

    /// <summary>
    /// Total opened on this date
    /// </summary>
    public int Opened { get; set; }

    /// <summary>
    /// Total failed on this date
    /// </summary>
    public int Failed { get; set; }
}