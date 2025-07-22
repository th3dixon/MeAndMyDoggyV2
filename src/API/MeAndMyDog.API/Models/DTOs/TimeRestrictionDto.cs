namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Time-based access restrictions configuration
/// </summary>
public class TimeRestrictionDto
{
    /// <summary>
    /// Allowed days of week (0=Sunday, 6=Saturday)
    /// </summary>
    public List<int> AllowedDaysOfWeek { get; set; } = new();

    /// <summary>
    /// Allowed time range start (24-hour format)
    /// </summary>
    public TimeSpan? AllowedTimeStart { get; set; }

    /// <summary>
    /// Allowed time range end (24-hour format)
    /// </summary>
    public TimeSpan? AllowedTimeEnd { get; set; }

    /// <summary>
    /// Time zone for time restrictions
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Whether weekend access is blocked
    /// </summary>
    public bool BlockWeekends { get; set; } = false;

    /// <summary>
    /// Whether holiday access is blocked
    /// </summary>
    public bool BlockHolidays { get; set; } = false;
}