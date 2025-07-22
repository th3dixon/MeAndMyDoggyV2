namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update self-destruct configuration
/// </summary>
public class UpdateSelfDestructRequest
{
    /// <summary>
    /// Timer duration in seconds
    /// </summary>
    public int? TimerSeconds { get; set; }

    /// <summary>
    /// Whether to notify participants when message is destroyed
    /// </summary>
    public bool? NotifyOnDestruction { get; set; }

    /// <summary>
    /// Whether to show countdown timer to participants
    /// </summary>
    public bool? ShowCountdown { get; set; }

    /// <summary>
    /// Maximum number of views before destruction
    /// </summary>
    public int? MaxViews { get; set; }

    /// <summary>
    /// Whether screenshots are blocked
    /// </summary>
    public bool? BlockScreenshots { get; set; }
}