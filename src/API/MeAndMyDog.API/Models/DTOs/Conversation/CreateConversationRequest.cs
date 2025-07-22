using System.ComponentModel.DataAnnotations;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for creating a conversation
/// </summary>
public class CreateConversationRequest
{
    /// <summary>
    /// Type of conversation to create
    /// </summary>
    [Required]
    public ConversationType ConversationType { get; set; }

    /// <summary>
    /// List of user IDs to add as participants (excluding creator)
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one participant is required")]
    public List<string> ParticipantIds { get; set; } = new();

    /// <summary>
    /// Optional conversation title
    /// </summary>
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Optional conversation description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional conversation image URL
    /// </summary>
    [Url]
    public string? ImageUrl { get; set; }
}