namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after creating an appointment
/// </summary>
public class AppointmentResponse
{
    /// <summary>
    /// Created or updated appointment
    /// </summary>
    public CalendarAppointmentDto Appointment { get; set; } = null!;

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

    /// <summary>
    /// Number of participants invited successfully
    /// </summary>
    public int ParticipantsInvited { get; set; }

    /// <summary>
    /// Number of reminders created
    /// </summary>
    public int RemindersCreated { get; set; }

    /// <summary>
    /// External calendar sync status
    /// </summary>
    public string? ExternalSyncStatus { get; set; }
}