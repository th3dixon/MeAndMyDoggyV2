namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for uploading voice messages
/// </summary>
public class UploadVoiceMessageRequest
{
    /// <summary>
    /// ID of the conversation to send the voice message to
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Audio file data (base64 encoded or multipart form data)
    /// </summary>
    public string? AudioData { get; set; }

    /// <summary>
    /// Audio format/codec
    /// </summary>
    public string AudioFormat { get; set; } = "webm";

    /// <summary>
    /// Duration of the recording in seconds
    /// </summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// Sample rate of the audio
    /// </summary>
    public int SampleRate { get; set; } = 48000;

    /// <summary>
    /// Whether to enable automatic transcription
    /// </summary>
    public bool EnableTranscription { get; set; } = true;

    /// <summary>
    /// Language for transcription (auto-detect if null)
    /// </summary>
    public string? TranscriptionLanguage { get; set; }

    /// <summary>
    /// Whether to auto-delete after first play
    /// </summary>
    public bool AutoDeleteAfterPlay { get; set; }

    /// <summary>
    /// Optional reply to message ID
    /// </summary>
    public string? ParentMessageId { get; set; }
}