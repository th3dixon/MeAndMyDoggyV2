using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for adding a participant
/// </summary>
public class AddParticipantRequest
{
    /// <summary>
    /// User ID to add to the conversation
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;
}