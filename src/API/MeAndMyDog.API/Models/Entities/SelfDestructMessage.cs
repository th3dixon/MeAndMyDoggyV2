using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Self-destructing message entity with automatic deletion capabilities
/// </summary>
[Table("SelfDestructMessages")]
public class SelfDestructMessage
{
    /// <summary>
    /// Self-destruct record unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Related message ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(Message))]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who set the self-destruct timer
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string SetByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Self-destruct mode type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DestructMode { get; set; } = string.Empty;

    /// <summary>
    /// Timer duration in seconds
    /// </summary>
    public int TimerSeconds { get; set; }

    /// <summary>
    /// When the timer starts (message read, sent, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TriggerEvent { get; set; } = string.Empty;

    /// <summary>
    /// When the timer was started
    /// </summary>
    public DateTimeOffset? TimerStartedAt { get; set; }

    /// <summary>
    /// When the message should be destroyed
    /// </summary>
    public DateTimeOffset? DestructAt { get; set; }

    /// <summary>
    /// Whether the message has been destroyed
    /// </summary>
    public bool IsDestroyed { get; set; } = false;

    /// <summary>
    /// When the message was actually destroyed
    /// </summary>
    public DateTimeOffset? DestroyedAt { get; set; }

    /// <summary>
    /// Destruction method used
    /// </summary>
    [MaxLength(50)]
    public string? DestructionMethod { get; set; }

    /// <summary>
    /// Whether to notify participants when message is destroyed
    /// </summary>
    public bool NotifyOnDestruction { get; set; } = true;

    /// <summary>
    /// Whether to show countdown timer to participants
    /// </summary>
    public bool ShowCountdown { get; set; } = true;

    /// <summary>
    /// Maximum number of views before destruction
    /// </summary>
    public int? MaxViews { get; set; }

    /// <summary>
    /// Current view count
    /// </summary>
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// Whether screenshots are blocked (hint for client)
    /// </summary>
    public bool BlockScreenshots { get; set; } = false;

    /// <summary>
    /// Additional security options (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? SecurityOptions { get; set; }

    /// <summary>
    /// When the self-destruct was configured
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the configuration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property to message
    /// </summary>
    public virtual Message Message { get; set; } = null!;

    /// <summary>
    /// Navigation property to view tracking records
    /// </summary>
    public virtual ICollection<MessageViewTracking> ViewTrackings { get; set; } = new List<MessageViewTracking>();
}