using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for recurrence patterns
/// </summary>
public class RecurrencePatternDto
{
    /// <summary>
    /// Type of recurrence
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Interval between occurrences
    /// </summary>
    public int Interval { get; set; } = 1;

    /// <summary>
    /// Days of week for weekly recurrence
    /// </summary>
    public List<string>? DaysOfWeek { get; set; }

    /// <summary>
    /// Day of month for monthly recurrence
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// Month for yearly recurrence
    /// </summary>
    public int? Month { get; set; }

    /// <summary>
    /// End date for recurrence
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Maximum number of occurrences
    /// </summary>
    public int? MaxOccurrences { get; set; }
}