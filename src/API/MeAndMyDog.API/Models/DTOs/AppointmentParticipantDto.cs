using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for appointment participant
/// </summary>
public class AppointmentParticipantDto
{
    /// <summary>
    /// Participant unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Appointment ID
    /// </summary>
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Participant user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Participant email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Participant display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Participant role in the appointment
    /// </summary>
    public ParticipantRole Role { get; set; }

    /// <summary>
    /// Participant response status
    /// </summary>
    public ResponseStatus ResponseStatus { get; set; }

    /// <summary>
    /// Whether this participant is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Whether this participant is the organizer
    /// </summary>
    public bool IsOrganizer { get; set; }

    /// <summary>
    /// When the participant was invited
    /// </summary>
    public DateTimeOffset InvitedAt { get; set; }

    /// <summary>
    /// When the participant responded
    /// </summary>
    public DateTimeOffset? RespondedAt { get; set; }

    /// <summary>
    /// Participant's response message
    /// </summary>
    public string? ResponseMessage { get; set; }

    /// <summary>
    /// External participant ID (for calendar sync)
    /// </summary>
    public string? ExternalParticipantId { get; set; }

    /// <summary>
    /// Participant's time zone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Additional participant notes
    /// </summary>
    public string? Notes { get; set; }
}