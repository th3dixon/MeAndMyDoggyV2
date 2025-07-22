namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Daily scheduled message statistics
/// </summary>
public class DailyScheduledMessageStats
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Messages scheduled on this date
    /// </summary>
    public int Scheduled { get; set; }

    /// <summary>
    /// Messages sent on this date
    /// </summary>
    public int Sent { get; set; }

    /// <summary>
    /// Messages failed on this date
    /// </summary>
    public int Failed { get; set; }
}