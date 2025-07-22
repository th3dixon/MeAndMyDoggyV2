using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update an existing appointment
/// </summary>
public class UpdateAppointmentRequest
{
    /// <summary>
    /// Appointment title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Appointment description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Start date and time of the appointment
    /// </summary>
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    /// End date and time of the appointment
    /// </summary>
    public DateTimeOffset? EndTime { get; set; }

    /// <summary>
    /// Time zone identifier for the appointment
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Appointment location (physical or virtual)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Virtual meeting URL (for online appointments)
    /// </summary>
    public string? MeetingUrl { get; set; }

    /// <summary>
    /// Appointment type
    /// </summary>
    public AppointmentType? AppointmentType { get; set; }

    /// <summary>
    /// Appointment status
    /// </summary>
    public AppointmentStatus? Status { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public AppointmentPriority? Priority { get; set; }

    /// <summary>
    /// Color code for calendar display
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Additional notes for the appointment
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether reminders are enabled
    /// </summary>
    public bool? RemindersEnabled { get; set; }

    /// <summary>
    /// Default reminder time in minutes before appointment
    /// </summary>
    public int? DefaultReminderMinutes { get; set; }
}