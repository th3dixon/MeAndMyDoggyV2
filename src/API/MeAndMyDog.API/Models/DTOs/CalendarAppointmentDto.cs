using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for calendar appointment
/// </summary>
public class CalendarAppointmentDto
{
    /// <summary>
    /// Appointment unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User who created the appointment
    /// </summary>
    public string CreatedByUserId { get; set; } = string.Empty;

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
    /// Appointment status
    /// </summary>
    public AppointmentStatus Status { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public AppointmentPriority Priority { get; set; }

    /// <summary>
    /// Whether this is an all-day appointment
    /// </summary>
    public bool IsAllDay { get; set; }

    /// <summary>
    /// Whether this is a recurring appointment
    /// </summary>
    public bool IsRecurring { get; set; }

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
    /// External calendar event ID (for sync with external calendars)
    /// </summary>
    public string? ExternalEventId { get; set; }

    /// <summary>
    /// External calendar provider (Google, Outlook, etc.)
    /// </summary>
    public CalendarProvider? ExternalProvider { get; set; }

    /// <summary>
    /// Whether reminders are enabled
    /// </summary>
    public bool RemindersEnabled { get; set; }

    /// <summary>
    /// Default reminder time in minutes before appointment
    /// </summary>
    public int DefaultReminderMinutes { get; set; }

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
    /// When the appointment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the appointment was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Who last updated the appointment
    /// </summary>
    public string? UpdatedByUserId { get; set; }

    /// <summary>
    /// When the appointment was cancelled (if applicable)
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Appointment participants
    /// </summary>
    public List<AppointmentParticipantDto> Participants { get; set; } = new();

    /// <summary>
    /// Appointment reminders
    /// </summary>
    public List<AppointmentReminderDto> Reminders { get; set; } = new();

    /// <summary>
    /// Duration of the appointment in minutes
    /// </summary>
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

    /// <summary>
    /// Whether the appointment is in the past
    /// </summary>
    public bool IsPast => EndTime < DateTimeOffset.UtcNow;

    /// <summary>
    /// Whether the appointment is currently happening
    /// </summary>
    public bool IsCurrentlyHappening => StartTime <= DateTimeOffset.UtcNow && EndTime >= DateTimeOffset.UtcNow;

    /// <summary>
    /// Number of confirmed participants
    /// </summary>
    public int ConfirmedParticipantsCount => Participants.Count(p => p.ResponseStatus == ResponseStatus.Accepted);
}