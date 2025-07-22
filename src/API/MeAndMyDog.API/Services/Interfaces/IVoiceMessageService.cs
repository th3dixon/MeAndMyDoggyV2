using MeAndMyDog.API.Models.DTOs;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for voice message management
/// </summary>
public interface IVoiceMessageService
{
    /// <summary>
    /// Upload and process a voice message
    /// </summary>
    /// <param name="userId">ID of the user uploading the voice message</param>
    /// <param name="request">Voice message upload request</param>
    /// <param name="audioFile">Audio file stream</param>
    /// <returns>Voice message response with processing status</returns>
    Task<VoiceMessageResponse> UploadVoiceMessageAsync(string userId, UploadVoiceMessageRequest request, Stream audioFile);

    /// <summary>
    /// Upload voice message from base64 encoded audio data
    /// </summary>
    /// <param name="userId">ID of the user uploading the voice message</param>
    /// <param name="request">Voice message upload request with base64 audio data</param>
    /// <returns>Voice message response with processing status</returns>
    Task<VoiceMessageResponse> UploadVoiceMessageAsync(string userId, UploadVoiceMessageRequest request);

    /// <summary>
    /// Get voice message details by ID
    /// </summary>
    /// <param name="userId">ID of the user requesting the voice message</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Voice message data or null if not found/unauthorized</returns>
    Task<VoiceMessageDto?> GetVoiceMessageAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Get voice messages for a specific message
    /// </summary>
    /// <param name="userId">ID of the user requesting voice messages</param>
    /// <param name="messageId">Parent message ID</param>
    /// <returns>List of voice messages</returns>
    Task<List<VoiceMessageDto>> GetVoiceMessagesForMessageAsync(string userId, string messageId);

    /// <summary>
    /// Track voice message playback
    /// </summary>
    /// <param name="userId">ID of the user playing the voice message</param>
    /// <param name="request">Playback tracking request</param>
    /// <returns>True if tracking was successful</returns>
    Task<bool> TrackPlaybackAsync(string userId, VoiceMessagePlaybackRequest request);

    /// <summary>
    /// Delete a voice message
    /// </summary>
    /// <param name="userId">ID of the user deleting the voice message</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteVoiceMessageAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Get transcription for a voice message
    /// </summary>
    /// <param name="userId">ID of the user requesting transcription</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Transcription text or null if not available/unauthorized</returns>
    Task<string?> GetTranscriptionAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Request transcription for a voice message
    /// </summary>
    /// <param name="userId">ID of the user requesting transcription</param>
    /// <param name="request">Transcription request</param>
    /// <returns>True if transcription was initiated successfully</returns>
    Task<bool> RequestTranscriptionAsync(string userId, TranscriptionRequest request);

    /// <summary>
    /// Get audio stream for playback
    /// </summary>
    /// <param name="userId">ID of the user requesting audio</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Audio file stream or null if not found/unauthorized</returns>
    Task<Stream?> GetAudioStreamAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Get processing status of a voice message
    /// </summary>
    /// <param name="userId">ID of the user checking status</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Processing status information</returns>
    Task<VoiceMessageResponse> GetProcessingStatusAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Generate waveform visualization data for a voice message
    /// </summary>
    /// <param name="userId">ID of the user requesting waveform</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Waveform data array</returns>
    Task<double[]?> GenerateWaveformAsync(string userId, string voiceMessageId);

    /// <summary>
    /// Get voice message statistics for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Voice message statistics</returns>
    Task<object> GetVoiceMessageStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate);

    /// <summary>
    /// Convert voice message to different audio format
    /// </summary>
    /// <param name="userId">ID of the user requesting conversion</param>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <param name="targetFormat">Target audio format</param>
    /// <returns>Converted audio stream or null</returns>
    Task<Stream?> ConvertAudioFormatAsync(string userId, string voiceMessageId, string targetFormat);

    /// <summary>
    /// Process audio file for optimization and enhancement
    /// </summary>
    /// <param name="audioStream">Input audio stream</param>
    /// <param name="settings">Processing settings</param>
    /// <returns>Processed audio stream</returns>
    Task<Stream> ProcessAudioAsync(Stream audioStream, AudioProcessingSettings settings);
}