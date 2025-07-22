using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a participant
/// </summary>
public class CreateParticipantRequest
{
    /// <summary>
    /// Participant user ID (if internal user)
    /// </summary>
    public string? UserId { get; set; }

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
    public ParticipantRole Role { get; set; } = ParticipantRole.RequiredAttendee;

    /// <summary>
    /// Whether this participant is required
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Participant's time zone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Additional participant notes
    /// </summary>
    public string? Notes { get; set; }
}