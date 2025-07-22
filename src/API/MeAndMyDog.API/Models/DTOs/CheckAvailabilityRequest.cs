namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Calendar availability request
/// </summary>
public class CheckAvailabilityRequest
{
    /// <summary>
    /// User IDs to check availability for
    /// </summary>
    public List<string> UserIds { get; set; } = new();

    /// <summary>
    /// Start time for availability check
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time for availability check
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Minimum duration required in minutes
    /// </summary>
    public int MinimumDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Time zone for the check
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Include tentative appointments as conflicts
    /// </summary>
    public bool IncludeTentative { get; set; } = true;
}