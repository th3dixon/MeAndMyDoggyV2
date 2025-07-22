using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for updating a message template
/// </summary>
public class UpdateMessageTemplateRequest : CreateMessageTemplateRequest
{
    /// <summary>
    /// Whether template is active/enabled
    /// </summary>
    public bool IsActive { get; set; } = true;
}