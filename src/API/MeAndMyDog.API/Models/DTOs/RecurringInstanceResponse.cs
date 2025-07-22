namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after recurring instance action
/// </summary>
public class RecurringInstanceResponse
{
    /// <summary>
    /// Instance details
    /// </summary>
    public AppointmentInstanceDto Instance { get; set; } = null!;

    /// <summary>
    /// Success indicator
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Success or error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Any validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();
}