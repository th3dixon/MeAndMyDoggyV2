using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Appointment conflict information
/// </summary>
public class AppointmentConflictDto
{
    /// <summary>
    /// Conflicting appointment ID
    /// </summary>
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Appointment title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Start time of conflicting appointment
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End time of conflicting appointment
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Appointment status
    /// </summary>
    public AppointmentStatus Status { get; set; }

    /// <summary>
    /// Whether the conflict is tentative
    /// </summary>
    public bool IsTentative => Status == AppointmentStatus.Pending;

    /// <summary>
    /// User ID for this conflict
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}