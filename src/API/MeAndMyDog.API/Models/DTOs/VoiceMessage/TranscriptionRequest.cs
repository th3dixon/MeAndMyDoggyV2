namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for voice message transcription
/// </summary>
public class TranscriptionRequest
{
    /// <summary>
    /// Voice message ID to transcribe
    /// </summary>
    public string VoiceMessageId { get; set; } = string.Empty;

    /// <summary>
    /// Target language for transcription
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Whether to force re-transcription if already exists
    /// </summary>
    public bool ForceReTranscribe { get; set; }
}