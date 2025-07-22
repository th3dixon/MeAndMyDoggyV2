using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a voice message attachment
/// </summary>
public class VoiceMessage
{
    /// <summary>
    /// Unique identifier for the voice message
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the message this voice message belongs to
    /// </summary>
    [Required]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the parent message
    /// </summary>
    public virtual Message Message { get; set; } = null!;

    /// <summary>
    /// File path or URL to the audio file
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Original filename uploaded by the user
    /// </summary>
    [MaxLength(255)]
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Audio format/codec (mp3, wav, ogg, webm, etc.)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string AudioFormat { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the voice message in seconds
    /// </summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Sample rate of the audio (e.g., 44100, 48000)
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Bit rate of the audio in kbps
    /// </summary>
    public int BitRate { get; set; }

    /// <summary>
    /// Number of audio channels (1 for mono, 2 for stereo)
    /// </summary>
    public int Channels { get; set; } = 1;

    /// <summary>
    /// Whether the voice message has been transcribed to text
    /// </summary>
    public bool IsTranscribed { get; set; }

    /// <summary>
    /// Transcription text of the voice message (if available)
    /// </summary>
    public string? TranscriptionText { get; set; }

    /// <summary>
    /// Confidence score of the transcription (0.0 to 1.0)
    /// </summary>
    public double? TranscriptionConfidence { get; set; }

    /// <summary>
    /// Language detected/used for transcription
    /// </summary>
    [MaxLength(10)]
    public string? TranscriptionLanguage { get; set; }

    /// <summary>
    /// Whether the voice message is currently being processed
    /// </summary>
    public bool IsProcessing { get; set; }

    /// <summary>
    /// Processing status or error message
    /// </summary>
    [MaxLength(500)]
    public string? ProcessingStatus { get; set; }

    /// <summary>
    /// Waveform data for visualization (JSON array of amplitude values)
    /// </summary>
    public string? WaveformData { get; set; }

    /// <summary>
    /// Whether the voice message has been played by the recipient
    /// </summary>
    public bool IsPlayed { get; set; }

    /// <summary>
    /// Number of times the voice message has been played
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// When the voice message was first played
    /// </summary>
    public DateTimeOffset? FirstPlayedAt { get; set; }

    /// <summary>
    /// When the voice message was last played
    /// </summary>
    public DateTimeOffset? LastPlayedAt { get; set; }

    /// <summary>
    /// Whether the voice message should auto-delete after being played
    /// </summary>
    public bool AutoDeleteAfterPlay { get; set; }

    /// <summary>
    /// When the voice message was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the voice message was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}