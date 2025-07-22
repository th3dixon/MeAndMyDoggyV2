using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Appointment reminder entity
/// </summary>
[Table("AppointmentReminders")]
public class AppointmentReminder
{
    /// <summary>
    /// Reminder unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Appointment ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(Appointment))]
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// User ID for this reminder
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Reminder type
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ReminderType { get; set; } = string.Empty;

    /// <summary>
    /// Minutes before appointment to send reminder
    /// </summary>
    [Required]
    public int MinutesBefore { get; set; }

    /// <summary>
    /// When the reminder should be sent
    /// </summary>
    [Required]
    public DateTimeOffset ReminderTime { get; set; }

    /// <summary>
    /// Whether the reminder has been sent
    /// </summary>
    public bool IsSent { get; set; } = false;

    /// <summary>
    /// When the reminder was sent
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// Whether the reminder was delivered successfully
    /// </summary>
    public bool IsDelivered { get; set; } = false;

    /// <summary>
    /// Delivery method used
    /// </summary>
    [MaxLength(50)]
    public string? DeliveryMethod { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [MaxLength(1000)]
    public string? DeliveryError { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Next retry time (if applicable)
    /// </summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>
    /// Custom reminder message
    /// </summary>
    [MaxLength(1000)]
    public string? CustomMessage { get; set; }

    /// <summary>
    /// Whether this reminder is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the reminder was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property to appointment
    /// </summary>
    public virtual CalendarAppointment Appointment { get; set; } = null!;
}