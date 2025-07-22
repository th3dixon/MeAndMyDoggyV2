using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a reusable message template
/// </summary>
public class MessageTemplate
{
    /// <summary>
    /// Template unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns this template
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

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
    /// Template category (personal, business, appointment, etc.)
    /// </summary>
    [StringLength(50)]
    public string Category { get; set; } = "personal";

    /// <summary>
    /// Template content with placeholders
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Template variables/placeholders (JSON array)
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// Whether template is active/enabled
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether template is shared with other users
    /// </summary>
    public bool IsShared { get; set; } = false;

    /// <summary>
    /// Whether template is a system default
    /// </summary>
    public bool IsSystemTemplate { get; set; } = false;

    /// <summary>
    /// Template language/locale
    /// </summary>
    [StringLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Number of times template has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

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

    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to scheduled messages using this template
    /// </summary>
    public ICollection<ScheduledMessage> ScheduledMessages { get; set; } = new List<ScheduledMessage>();
}