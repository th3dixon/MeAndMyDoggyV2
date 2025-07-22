using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for editing a message
/// </summary>
public class EditMessageRequest
{
    /// <summary>
    /// New message content
    /// </summary>
    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Reason for editing (optional)
    /// </summary>
    [StringLength(200)]
    public string? EditReason { get; set; }
}