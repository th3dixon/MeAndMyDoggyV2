using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Reminder delivery statistics
/// </summary>
public class ReminderStatsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total reminders created
    /// </summary>
    public int TotalReminders { get; set; }

    /// <summary>
    /// Reminders sent successfully
    /// </summary>
    public int RemindersDelivered { get; set; }

    /// <summary>
    /// Failed reminder deliveries
    /// </summary>
    public int ReminderFailures { get; set; }

    /// <summary>
    /// Pending reminders
    /// </summary>
    public int PendingReminders { get; set; }

    /// <summary>
    /// Delivery success rate (0-1)
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Most used reminder type
    /// </summary>
    public ReminderType MostUsedReminderType { get; set; }

    /// <summary>
    /// Average reminder time in minutes before appointment
    /// </summary>
    public double AverageReminderTime { get; set; }

    /// <summary>
    /// Statistics date range start
    /// </summary>
    public DateTimeOffset FromDate { get; set; }

    /// <summary>
    /// Statistics date range end
    /// </summary>
    public DateTimeOffset ToDate { get; set; }

    /// <summary>
    /// Reminder type breakdown
    /// </summary>
    public List<ReminderTypeStatsDto> ReminderTypeStats { get; set; } = new();
}