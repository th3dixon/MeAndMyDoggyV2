using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Calendar appointment entity
/// </summary>
[Table("CalendarAppointments")]
public class CalendarAppointment
{
    /// <summary>
    /// Appointment unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// User who created the appointment
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Related conversation ID (optional)
    /// </summary>
    [MaxLength(450)]
    public string? ConversationId { get; set; }

    /// <summary>
    /// Appointment title
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Appointment description
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Start date and time of the appointment
    /// </summary>
    [Required]
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// End date and time of the appointment
    /// </summary>
    [Required]
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Time zone identifier for the appointment
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Appointment location (physical or virtual)
    /// </summary>
    [MaxLength(500)]
    public string? Location { get; set; }

    /// <summary>
    /// Virtual meeting URL (for online appointments)
    /// </summary>
    [MaxLength(1000)]
    public string? MeetingUrl { get; set; }

    /// <summary>
    /// Appointment type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AppointmentType { get; set; } = string.Empty;

    /// <summary>
    /// Appointment status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Priority level
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Priority { get; set; } = string.Empty;

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
    [MaxLength(50)]
    public string? RecurrencePattern { get; set; }

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
    [MaxLength(500)]
    public string? ExternalEventId { get; set; }

    /// <summary>
    /// External calendar provider (Google, Outlook, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? ExternalProvider { get; set; }

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
    [MaxLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// Additional notes for the appointment
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Attachment file names (JSON array)
    /// </summary>
    [MaxLength(2000)]
    public string? AttachmentFiles { get; set; }

    /// <summary>
    /// When the appointment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the appointment was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Who last updated the appointment
    /// </summary>
    [MaxLength(450)]
    public string? UpdatedByUserId { get; set; }

    /// <summary>
    /// When the appointment was cancelled (if applicable)
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Navigation property to appointment participants
    /// </summary>
    public virtual ICollection<AppointmentParticipant> Participants { get; set; } = new List<AppointmentParticipant>();

    /// <summary>
    /// Navigation property to appointment reminders
    /// </summary>
    public virtual ICollection<AppointmentReminder> Reminders { get; set; } = new List<AppointmentReminder>();

    /// <summary>
    /// Navigation property to recurring appointment instances
    /// </summary>
    public virtual ICollection<AppointmentInstance> RecurringInstances { get; set; } = new List<AppointmentInstance>();
}