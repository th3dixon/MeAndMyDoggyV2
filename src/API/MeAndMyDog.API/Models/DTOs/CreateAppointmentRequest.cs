using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a new appointment
/// </summary>
public class CreateAppointmentRequest
{
    /// <summary>
    /// Related conversation ID (optional)
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// Appointment title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Appointment description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Start date and time of the appointment
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End date and time of the appointment
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Time zone identifier for the appointment
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

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
    public AppointmentType AppointmentType { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public AppointmentPriority Priority { get; set; } = AppointmentPriority.Normal;

    /// <summary>
    /// Whether this is an all-day appointment
    /// </summary>
    public bool IsAllDay { get; set; } = false;

    /// <summary>
    /// Whether this is a recurring appointment
    /// </summary>
    public bool IsRecurring { get; set; } = false;

    /// <summary>
    /// Recurrence pattern (if recurring)
    /// </summary>
    public RecurrencePattern? RecurrencePattern { get; set; }

    /// <summary>
    /// Recurrence interval (e.g., every 2 weeks)
    /// </summary>
    public int? RecurrenceInterval { get; set; }

    /// <summary>
    /// End date for recurring appointments
    /// </summary>
    public DateTimeOffset? RecurrenceEndDate { get; set; }

    /// <summary>
    /// Maximum number of occurrences for recurring appointments
    /// </summary>
    public int? MaxOccurrences { get; set; }

    /// <summary>
    /// Whether reminders are enabled
    /// </summary>
    public bool RemindersEnabled { get; set; } = true;

    /// <summary>
    /// Default reminder time in minutes before appointment
    /// </summary>
    public int DefaultReminderMinutes { get; set; } = 15;

    /// <summary>
    /// Color code for calendar display
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Additional notes for the appointment
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Attachment file names
    /// </summary>
    public List<string> AttachmentFiles { get; set; } = new();

    /// <summary>
    /// Initial participants to invite
    /// </summary>
    public List<CreateParticipantRequest> Participants { get; set; } = new();

    /// <summary>
    /// Custom reminder settings
    /// </summary>
    public List<CreateReminderRequest> CustomReminders { get; set; } = new();
}