using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for appointment reminder
/// </summary>
public class AppointmentReminderDto
{
    /// <summary>
    /// Reminder unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Appointment ID
    /// </summary>
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// User ID for this reminder
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Reminder type
    /// </summary>
    public ReminderType ReminderType { get; set; }

    /// <summary>
    /// Minutes before appointment to send reminder
    /// </summary>
    public int MinutesBefore { get; set; }

    /// <summary>
    /// When the reminder should be sent
    /// </summary>
    public DateTimeOffset ReminderTime { get; set; }

    /// <summary>
    /// Whether the reminder has been sent
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// When the reminder was sent
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// Whether the reminder was delivered successfully
    /// </summary>
    public bool IsDelivered { get; set; }

    /// <summary>
    /// Delivery method used
    /// </summary>
    public string? DeliveryMethod { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? DeliveryError { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// Next retry time (if applicable)
    /// </summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>
    /// Custom reminder message
    /// </summary>
    public string? CustomMessage { get; set; }

    /// <summary>
    /// Whether this reminder is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the reminder was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}

