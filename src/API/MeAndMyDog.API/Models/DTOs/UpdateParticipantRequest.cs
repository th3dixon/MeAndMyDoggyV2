using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update participant details
/// </summary>
public class UpdateParticipantRequest
{
    /// <summary>
    /// Participant display name
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Participant role in the appointment
    /// </summary>
    public ParticipantRole? Role { get; set; }

    /// <summary>
    /// Whether this participant is required
    /// </summary>
    public bool? IsRequired { get; set; }

    /// <summary>
    /// Participant's time zone
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Additional participant notes
    /// </summary>
    public string? Notes { get; set; }
}