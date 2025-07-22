using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Message view tracking for self-destruct messages
/// </summary>
[Table("MessageViewTrackings")]
public class MessageViewTracking
{
    /// <summary>
    /// View tracking unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Self-destruct message ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(SelfDestructMessage))]
    public string SelfDestructMessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who viewed the message
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string ViewedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// When the message was viewed
    /// </summary>
    [Required]
    public DateTimeOffset ViewedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// How long the message was viewed (milliseconds)
    /// </summary>
    public long ViewDurationMs { get; set; }

    /// <summary>
    /// Client IP address
    /// </summary>
    [MaxLength(45)]
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Client user agent
    /// </summary>
    [MaxLength(500)]
    public string? ClientUserAgent { get; set; }

    /// <summary>
    /// Client device fingerprint
    /// </summary>
    [MaxLength(200)]
    public string? DeviceFingerprint { get; set; }

    /// <summary>
    /// Whether this view triggered timer start
    /// </summary>
    public bool TriggeredTimer { get; set; } = false;

    /// <summary>
    /// Additional view metadata (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? ViewMetadata { get; set; }

    /// <summary>
    /// Navigation property to self-destruct message
    /// </summary>
    public virtual SelfDestructMessage SelfDestructMessage { get; set; } = null!;
}