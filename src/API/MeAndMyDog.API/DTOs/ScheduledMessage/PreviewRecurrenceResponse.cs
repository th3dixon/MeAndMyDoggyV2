namespace MeAndMyDog.API.DTOs.ScheduledMessage;

/// <summary>
/// Response object for recurrence preview
/// </summary>
public class PreviewRecurrenceResponse
{
    /// <summary>
    /// Whether preview was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// List of occurrence dates
    /// </summary>
    public List<DateTimeOffset> Occurrences { get; set; } = new();

    /// <summary>
    /// Number of occurrences returned
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Any preview messages
    /// </summary>
    public string? Message { get; set; }
}