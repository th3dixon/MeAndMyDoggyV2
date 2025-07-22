using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to configure self-destruct for a message
/// </summary>
public class ConfigureSelfDestructRequest
{
    /// <summary>
    /// Message ID to configure
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Self-destruct mode type
    /// </summary>
    public DestructMode DestructMode { get; set; } = DestructMode.Timer;

    /// <summary>
    /// Timer duration in seconds
    /// </summary>
    public int TimerSeconds { get; set; } = 3600; // Default 1 hour

    /// <summary>
    /// When the timer starts
    /// </summary>
    public TriggerEvent TriggerEvent { get; set; } = TriggerEvent.FirstRead;

    /// <summary>
    /// Whether to notify participants when message is destroyed
    /// </summary>
    public bool NotifyOnDestruction { get; set; } = true;

    /// <summary>
    /// Whether to show countdown timer to participants
    /// </summary>
    public bool ShowCountdown { get; set; } = true;

    /// <summary>
    /// Maximum number of views before destruction
    /// </summary>
    public int? MaxViews { get; set; }

    /// <summary>
    /// Whether screenshots are blocked
    /// </summary>
    public bool BlockScreenshots { get; set; } = false;

    /// <summary>
    /// Scheduled destruction time (for ScheduledTime mode)
    /// </summary>
    public DateTimeOffset? ScheduledDestructAt { get; set; }
}