namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for voice messages
/// </summary>
public class VoiceMessageDto
{
    /// <summary>
    /// Unique identifier for the voice message
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parent message
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// URL to access the audio file
    /// </summary>
    public string AudioUrl { get; set; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Audio format (mp3, wav, ogg, webm)
    /// </summary>
    public string AudioFormat { get; set; } = string.Empty;

    /// <summary>
    /// Duration in seconds
    /// </summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Audio sample rate
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Audio bit rate in kbps
    /// </summary>
    public int BitRate { get; set; }

    /// <summary>
    /// Number of audio channels
    /// </summary>
    public int Channels { get; set; }

    /// <summary>
    /// Whether transcription is available
    /// </summary>
    public bool IsTranscribed { get; set; }

    /// <summary>
    /// Transcription text
    /// </summary>
    public string? TranscriptionText { get; set; }

    /// <summary>
    /// Transcription confidence score
    /// </summary>
    public double? TranscriptionConfidence { get; set; }

    /// <summary>
    /// Transcription language
    /// </summary>
    public string? TranscriptionLanguage { get; set; }

    /// <summary>
    /// Whether the voice message is being processed
    /// </summary>
    public bool IsProcessing { get; set; }

    /// <summary>
    /// Processing status
    /// </summary>
    public string? ProcessingStatus { get; set; }

    /// <summary>
    /// Waveform visualization data
    /// </summary>
    public double[]? WaveformData { get; set; }

    /// <summary>
    /// Whether the message has been played
    /// </summary>
    public bool IsPlayed { get; set; }

    /// <summary>
    /// Number of times played
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// When first played
    /// </summary>
    public DateTimeOffset? FirstPlayedAt { get; set; }

    /// <summary>
    /// When last played
    /// </summary>
    public DateTimeOffset? LastPlayedAt { get; set; }

    /// <summary>
    /// When the voice message was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Conversation ID where this voice message was sent
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the user who sent the voice message
    /// </summary>
    public string SenderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name of the sender
    /// </summary>
    public string SenderName { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in seconds (alias for DurationSeconds)
    /// </summary>
    public double? Duration => DurationSeconds;
}