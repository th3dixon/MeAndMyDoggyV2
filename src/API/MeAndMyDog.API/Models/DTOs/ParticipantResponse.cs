namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after participant action
/// </summary>
public class ParticipantResponse
{
    /// <summary>
    /// Participant details
    /// </summary>
    public AppointmentParticipantDto Participant { get; set; } = null!;

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