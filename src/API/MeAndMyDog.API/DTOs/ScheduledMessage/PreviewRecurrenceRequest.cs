using MeAndMyDog.API.Models.DTOs;

namespace MeAndMyDog.API.DTOs.ScheduledMessage;

/// <summary>
/// Request object for previewing recurrence
/// </summary>
public class PreviewRecurrenceRequest
{
    /// <summary>
    /// Start date for recurrence
    /// </summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Recurrence pattern
    /// </summary>
    public RecurrencePatternDto RecurrencePattern { get; set; } = null!;

    /// <summary>
    /// Timezone for calculation
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Number of occurrences to preview
    /// </summary>
    public int Count { get; set; } = 10;
}