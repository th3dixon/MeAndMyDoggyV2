using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for recurring appointment instance
/// </summary>
public class AppointmentInstanceDto
{
    /// <summary>
    /// Instance unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Parent recurring appointment ID
    /// </summary>
    public string ParentAppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Instance number in the recurring series
    /// </summary>
    public int InstanceNumber { get; set; }

    /// <summary>
    /// Original start time for this instance
    /// </summary>
    public DateTimeOffset OriginalStartTime { get; set; }

    /// <summary>
    /// Original end time for this instance
    /// </summary>
    public DateTimeOffset OriginalEndTime { get; set; }

    /// <summary>
    /// Actual start time (may differ from original if modified)
    /// </summary>
    public DateTimeOffset ActualStartTime { get; set; }

    /// <summary>
    /// Actual end time (may differ from original if modified)
    /// </summary>
    public DateTimeOffset ActualEndTime { get; set; }

    /// <summary>
    /// Instance status
    /// </summary>
    public AppointmentStatus Status { get; set; }

    /// <summary>
    /// Whether this instance has been modified from the original
    /// </summary>
    public bool IsModified { get; set; }

    /// <summary>
    /// Whether this instance has been cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Custom title for this instance (if different from parent)
    /// </summary>
    public string? CustomTitle { get; set; }

    /// <summary>
    /// Custom description for this instance
    /// </summary>
    public string? CustomDescription { get; set; }

    /// <summary>
    /// Custom location for this instance
    /// </summary>
    public string? CustomLocation { get; set; }

    /// <summary>
    /// External event ID for this instance
    /// </summary>
    public string? ExternalEventId { get; set; }

    /// <summary>
    /// When this instance was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When this instance was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Who last updated this instance
    /// </summary>
    public string? UpdatedByUserId { get; set; }

    /// <summary>
    /// Cancellation reason (if cancelled)
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// When this instance was cancelled
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }

    /// <summary>
    /// Additional notes for this instance
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Parent appointment details
    /// </summary>
    public CalendarAppointmentDto? ParentAppointment { get; set; }

    /// <summary>
    /// Duration of this instance in minutes
    /// </summary>
    public int DurationMinutes => (int)(ActualEndTime - ActualStartTime).TotalMinutes;

    /// <summary>
    /// Whether times have been modified from the original
    /// </summary>
    public bool HasTimeChanges => ActualStartTime != OriginalStartTime || ActualEndTime != OriginalEndTime;

    /// <summary>
    /// Effective title (custom or parent title)
    /// </summary>
    public string EffectiveTitle => CustomTitle ?? ParentAppointment?.Title ?? string.Empty;

    /// <summary>
    /// Effective description (custom or parent description)
    /// </summary>
    public string? EffectiveDescription => CustomDescription ?? ParentAppointment?.Description;

    /// <summary>
    /// Effective location (custom or parent location)
    /// </summary>
    public string? EffectiveLocation => CustomLocation ?? ParentAppointment?.Location;
}