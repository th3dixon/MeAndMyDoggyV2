using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Reminder type statistics
/// </summary>
public class ReminderTypeStatsDto
{
    /// <summary>
    /// Reminder type
    /// </summary>
    public ReminderType ReminderType { get; set; }

    /// <summary>
    /// Type name
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// Number of reminders of this type
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Successful deliveries for this type
    /// </summary>
    public int Delivered { get; set; }

    /// <summary>
    /// Failed deliveries for this type
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Success rate for this type (0-1)
    /// </summary>
    public double SuccessRate { get; set; }
}