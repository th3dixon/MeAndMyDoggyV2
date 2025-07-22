using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for message templates
/// </summary>
public class MessageTemplateDto
{
    /// <summary>
    /// Template unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns this template
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Template name/title
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Template category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Template content with placeholders
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Template variables/placeholders
    /// </summary>
    public List<TemplateVariableDto>? Variables { get; set; }

    /// <summary>
    /// Whether template is active/enabled
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether template is shared with other users
    /// </summary>
    public bool IsShared { get; set; }

    /// <summary>
    /// Whether template is a system default
    /// </summary>
    public bool IsSystemTemplate { get; set; }

    /// <summary>
    /// Template language/locale
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Number of times template has been used
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// When template was last used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// When template was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When template was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}