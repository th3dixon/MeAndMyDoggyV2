namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for voice message playback tracking
/// </summary>
public class VoiceMessagePlaybackRequest
{
    /// <summary>
    /// Voice message ID
    /// </summary>
    public string VoiceMessageId { get; set; } = string.Empty;

    /// <summary>
    /// Device information for tracking
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Playback position when stopped (for resuming)
    /// </summary>
    public double? PlaybackPosition { get; set; }

    /// <summary>
    /// Whether playback was completed
    /// </summary>
    public bool PlaybackCompleted { get; set; }
}