using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Recurring appointment instance entity
/// </summary>
[Table("AppointmentInstances")]
public class AppointmentInstance
{
    /// <summary>
    /// Instance unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Parent recurring appointment ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(ParentAppointment))]
    public string ParentAppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Instance number in the recurring series
    /// </summary>
    [Required]
    public int InstanceNumber { get; set; }

    /// <summary>
    /// Original start time for this instance
    /// </summary>
    [Required]
    public DateTimeOffset OriginalStartTime { get; set; }

    /// <summary>
    /// Original end time for this instance
    /// </summary>
    [Required]
    public DateTimeOffset OriginalEndTime { get; set; }

    /// <summary>
    /// Actual start time (may differ from original if modified)
    /// </summary>
    [Required]
    public DateTimeOffset ActualStartTime { get; set; }

    /// <summary>
    /// Actual end time (may differ from original if modified)
    /// </summary>
    [Required]
    public DateTimeOffset ActualEndTime { get; set; }

    /// <summary>
    /// Instance status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether this instance has been modified from the original
    /// </summary>
    public bool IsModified { get; set; } = false;

    /// <summary>
    /// Whether this instance has been cancelled
    /// </summary>
    public bool IsCancelled { get; set; } = false;

    /// <summary>
    /// Custom title for this instance (if different from parent)
    /// </summary>
    [MaxLength(200)]
    public string? CustomTitle { get; set; }

    /// <summary>
    /// Custom description for this instance
    /// </summary>
    [MaxLength(2000)]
    public string? CustomDescription { get; set; }

    /// <summary>
    /// Custom location for this instance
    /// </summary>
    [MaxLength(500)]
    public string? CustomLocation { get; set; }

    /// <summary>
    /// External event ID for this instance
    /// </summary>
    [MaxLength(500)]
    public string? ExternalEventId { get; set; }

    /// <summary>
    /// When this instance was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When this instance was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Who last updated this instance
    /// </summary>
    [MaxLength(450)]
    public string? UpdatedByUserId { get; set; }

    /// <summary>
    /// Cancellation reason (if cancelled)
    /// </summary>
    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    /// <summary>
    /// When this instance was cancelled
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }

    /// <summary>
    /// Additional notes for this instance
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to parent appointment
    /// </summary>
    public virtual CalendarAppointment ParentAppointment { get; set; } = null!;
}