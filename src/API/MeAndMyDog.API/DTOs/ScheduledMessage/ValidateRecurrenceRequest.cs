using MeAndMyDog.API.Models.DTOs;

namespace MeAndMyDog.API.DTOs.ScheduledMessage;

/// <summary>
/// Request object for validating recurrence patterns
/// </summary>
public class ValidateRecurrenceRequest
{
    /// <summary>
    /// Recurrence pattern to validate
    /// </summary>
    public RecurrencePatternDto RecurrencePattern { get; set; } = null!;
}