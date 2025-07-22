namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Available time slot information
/// </summary>
public class AvailableTimeSlotDto
{
    /// <summary>
    /// Start time of available slot
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of available slot
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Duration of the slot in minutes
    /// </summary>
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

    /// <summary>
    /// Time zone for the slot
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}