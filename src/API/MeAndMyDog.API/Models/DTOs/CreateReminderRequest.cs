using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a reminder
/// </summary>
public class CreateReminderRequest
{
    /// <summary>
    /// User ID for this reminder (optional, defaults to current user)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Reminder type
    /// </summary>
    public ReminderType ReminderType { get; set; } = ReminderType.PushNotification;

    /// <summary>
    /// Minutes before appointment to send reminder
    /// </summary>
    public int MinutesBefore { get; set; } = 15;

    /// <summary>
    /// Custom reminder message
    /// </summary>
    public string? CustomMessage { get; set; }

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}