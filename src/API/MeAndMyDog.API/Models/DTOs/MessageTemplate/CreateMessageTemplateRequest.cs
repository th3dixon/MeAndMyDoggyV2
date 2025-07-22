using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for creating a message template
/// </summary>
public class CreateMessageTemplateRequest
{
    /// <summary>
    /// Template name/title
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Template category
    /// </summary>
    [Required]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Template content with placeholders
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Template variables/placeholders
    /// </summary>
    public List<TemplateVariableDto>? Variables { get; set; }

    /// <summary>
    /// Whether template is shared with other users
    /// </summary>
    public bool IsShared { get; set; } = false;

    /// <summary>
    /// Template language/locale
    /// </summary>
    public string Language { get; set; } = "en";
}