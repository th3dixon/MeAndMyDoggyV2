using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for updating a conversation
/// </summary>
public class UpdateConversationRequest
{
    /// <summary>
    /// New conversation title
    /// </summary>
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// New conversation description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// New conversation image URL
    /// </summary>
    [Url]
    public string? ImageUrl { get; set; }
}