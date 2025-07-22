using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for updating a scheduled message
/// </summary>
public class UpdateScheduledMessageRequest
{
    /// <summary>
    /// Message content
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// When to send the message
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Recurrence pattern
    /// </summary>
    public RecurrencePatternDto? RecurrencePattern { get; set; }
}