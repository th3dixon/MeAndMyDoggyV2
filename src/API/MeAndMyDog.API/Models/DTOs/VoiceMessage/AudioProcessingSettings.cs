namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Audio processing settings for voice messages
/// </summary>
public class AudioProcessingSettings
{
    /// <summary>
    /// Maximum file size in bytes (default 10MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// Maximum duration in seconds (default 5 minutes)
    /// </summary>
    public double MaxDurationSeconds { get; set; } = 300;

    /// <summary>
    /// Supported audio formats
    /// </summary>
    public string[] SupportedFormats { get; set; } = { "webm", "ogg", "mp3", "wav", "m4a" };

    /// <summary>
    /// Target audio quality for compression
    /// </summary>
    public string AudioQuality { get; set; } = "medium"; // low, medium, high

    /// <summary>
    /// Whether to generate waveform visualization
    /// </summary>
    public bool GenerateWaveform { get; set; } = true;

    /// <summary>
    /// Whether to enable noise reduction
    /// </summary>
    public bool EnableNoiseReduction { get; set; } = true;

    /// <summary>
    /// Whether to normalize audio levels
    /// </summary>
    public bool EnableNormalization { get; set; } = true;
}