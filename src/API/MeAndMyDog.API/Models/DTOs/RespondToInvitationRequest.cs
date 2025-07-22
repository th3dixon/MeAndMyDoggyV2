using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to respond to an appointment invitation
/// </summary>
public class RespondToInvitationRequest
{
    /// <summary>
    /// Participant response status
    /// </summary>
    public ResponseStatus ResponseStatus { get; set; }

    /// <summary>
    /// Participant's response message
    /// </summary>
    public string? ResponseMessage { get; set; }
}