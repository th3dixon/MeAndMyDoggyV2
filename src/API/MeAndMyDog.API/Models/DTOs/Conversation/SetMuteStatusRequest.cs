namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for setting mute status
/// </summary>
public class SetMuteStatusRequest
{
    /// <summary>
    /// Whether to mute the conversation
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    /// Optional time when mute should be automatically lifted
    /// </summary>
    public DateTimeOffset? MutedUntil { get; set; }
}