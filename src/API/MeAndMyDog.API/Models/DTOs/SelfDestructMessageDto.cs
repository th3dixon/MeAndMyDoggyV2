using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for self-destructing message
/// </summary>
public class SelfDestructMessageDto
{
    /// <summary>
    /// Self-destruct record unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Related message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who set the self-destruct timer
    /// </summary>
    public string SetByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Self-destruct mode type
    /// </summary>
    public DestructMode DestructMode { get; set; }

    /// <summary>
    /// Timer duration in seconds
    /// </summary>
    public int TimerSeconds { get; set; }

    /// <summary>
    /// When the timer starts (message read, sent, etc.)
    /// </summary>
    public TriggerEvent TriggerEvent { get; set; }

    /// <summary>
    /// When the timer was started
    /// </summary>
    public DateTimeOffset? TimerStartedAt { get; set; }

    /// <summary>
    /// When the message should be destroyed
    /// </summary>
    public DateTimeOffset? DestructAt { get; set; }

    /// <summary>
    /// Whether the message has been destroyed
    /// </summary>
    public bool IsDestroyed { get; set; }

    /// <summary>
    /// When the message was actually destroyed
    /// </summary>
    public DateTimeOffset? DestroyedAt { get; set; }

    /// <summary>
    /// Destruction method used
    /// </summary>
    public string? DestructionMethod { get; set; }

    /// <summary>
    /// Whether to notify participants when message is destroyed
    /// </summary>
    public bool NotifyOnDestruction { get; set; }

    /// <summary>
    /// Whether to show countdown timer to participants
    /// </summary>
    public bool ShowCountdown { get; set; }

    /// <summary>
    /// Maximum number of views before destruction
    /// </summary>
    public int? MaxViews { get; set; }

    /// <summary>
    /// Current view count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Whether screenshots are blocked (hint for client)
    /// </summary>
    public bool BlockScreenshots { get; set; }

    /// <summary>
    /// Time remaining until destruction (in seconds)
    /// </summary>
    public int? TimeRemainingSeconds
    {
        get
        {
            if (!DestructAt.HasValue || IsDestroyed)
                return null;
            
            var remaining = (DestructAt.Value - DateTimeOffset.UtcNow).TotalSeconds;
            return remaining > 0 ? (int)remaining : 0;
        }
    }

    /// <summary>
    /// Whether the countdown is currently active
    /// </summary>
    public bool IsCountdownActive => TimerStartedAt.HasValue && !IsDestroyed && DestructAt.HasValue && DestructAt > DateTimeOffset.UtcNow;

    /// <summary>
    /// Views remaining before destruction
    /// </summary>
    public int? ViewsRemaining => MaxViews.HasValue ? Math.Max(0, MaxViews.Value - ViewCount) : null;

    /// <summary>
    /// When the self-destruct was configured
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the configuration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
