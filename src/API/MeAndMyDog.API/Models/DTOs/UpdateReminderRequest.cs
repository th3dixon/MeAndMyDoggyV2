using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update a reminder
/// </summary>
public class UpdateReminderRequest
{
    /// <summary>
    /// Reminder type
    /// </summary>
    public ReminderType? ReminderType { get; set; }

    /// <summary>
    /// Minutes before appointment to send reminder
    /// </summary>
    public int? MinutesBefore { get; set; }

    /// <summary>
    /// Custom reminder message
    /// </summary>
    public string? CustomMessage { get; set; }

    /// <summary>
    /// Whether this reminder is active
    /// </summary>
    public bool? IsActive { get; set; }
}