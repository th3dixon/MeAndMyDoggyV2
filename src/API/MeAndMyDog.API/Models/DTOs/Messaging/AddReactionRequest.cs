using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for adding a reaction to a message
/// </summary>
public class AddReactionRequest
{
    /// <summary>
    /// Reaction emoji or identifier
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Reaction { get; set; } = string.Empty;
}