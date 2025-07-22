using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for voice message operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class VoiceMessageController : ControllerBase
{
    private readonly IVoiceMessageService _voiceMessageService;
    private readonly ILogger<VoiceMessageController> _logger;

    /// <summary>
    /// Initialize the voice message controller
    /// </summary>
    public VoiceMessageController(IVoiceMessageService voiceMessageService, ILogger<VoiceMessageController> logger)
    {
        _voiceMessageService = voiceMessageService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a voice message via multipart form data
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="audioFile">Audio file</param>
    /// <param name="audioFormat">Audio format</param>
    /// <param name="durationSeconds">Duration in seconds</param>
    /// <param name="sampleRate">Sample rate</param>
    /// <param name="enableTranscription">Enable automatic transcription</param>
    /// <param name="transcriptionLanguage">Language for transcription</param>
    /// <param name="autoDeleteAfterPlay">Auto-delete after play</param>
    /// <param name="parentMessageId">Parent message ID for replies</param>
    /// <returns>Voice message response</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<VoiceMessageResponse>> UploadVoiceMessage(
        IFormFile audioFile,
        [FromForm] string conversationId,
        [FromForm] string audioFormat = "webm",
        [FromForm] double durationSeconds = 0,
        [FromForm] int sampleRate = 48000,
        [FromForm] bool enableTranscription = true,
        [FromForm] string? transcriptionLanguage = null,
        [FromForm] bool autoDeleteAfterPlay = false,
        [FromForm] string? parentMessageId = null)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VoiceMessageResponse { Success = false, Message = "User not authenticated" });
            }

            if (audioFile == null || audioFile.Length == 0)
            {
                return BadRequest(new VoiceMessageResponse { Success = false, Message = "Audio file is required" });
            }

            var request = new UploadVoiceMessageRequest
            {
                ConversationId = conversationId,
                AudioFormat = audioFormat,
                DurationSeconds = durationSeconds > 0 ? durationSeconds : EstimateDuration(audioFile.Length),
                SampleRate = sampleRate,
                EnableTranscription = enableTranscription,
                TranscriptionLanguage = transcriptionLanguage,
                AutoDeleteAfterPlay = autoDeleteAfterPlay,
                ParentMessageId = parentMessageId
            };

            using var audioStream = audioFile.OpenReadStream();
            var response = await _voiceMessageService.UploadVoiceMessageAsync(userId, request, audioStream);

            if (response.Success)
            {
                _logger.LogInformation("Voice message uploaded successfully by user {UserId} to conversation {ConversationId}", 
                    userId, conversationId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Voice message upload failed for user {UserId}: {Message}", userId, response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading voice message for user {UserId}", GetUserId());
            return StatusCode(500, new VoiceMessageResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Upload voice message from base64 audio data
    /// </summary>
    /// <param name="request">Upload request with base64 audio data</param>
    /// <returns>Voice message response</returns>
    [HttpPost("upload-base64")]
    public async Task<ActionResult<VoiceMessageResponse>> UploadVoiceMessageBase64([FromBody] UploadVoiceMessageRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VoiceMessageResponse { Success = false, Message = "User not authenticated" });
            }

            var response = await _voiceMessageService.UploadVoiceMessageAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Voice message uploaded from base64 by user {UserId} to conversation {ConversationId}", 
                    userId, request.ConversationId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Voice message upload from base64 failed for user {UserId}: {Message}", userId, response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading voice message from base64 for user {UserId}", GetUserId());
            return StatusCode(500, new VoiceMessageResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get voice message details
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Voice message details</returns>
    [HttpGet("{voiceMessageId}")]
    public async Task<ActionResult<VoiceMessageDto>> GetVoiceMessage(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var voiceMessage = await _voiceMessageService.GetVoiceMessageAsync(userId, voiceMessageId);

            if (voiceMessage == null)
            {
                return NotFound("Voice message not found or access denied");
            }

            return Ok(voiceMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice message {VoiceMessageId} for user {UserId}", voiceMessageId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get audio stream for voice message playback
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Audio file stream</returns>
    [HttpGet("{voiceMessageId}/audio")]
    public async Task<ActionResult> GetAudioStream(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var audioStream = await _voiceMessageService.GetAudioStreamAsync(userId, voiceMessageId);

            if (audioStream == null)
            {
                return NotFound("Audio file not found or access denied");
            }

            // Get voice message details for content type
            var voiceMessage = await _voiceMessageService.GetVoiceMessageAsync(userId, voiceMessageId);
            var contentType = GetContentType(voiceMessage?.AudioFormat ?? "webm");

            return File(audioStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audio stream for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track voice message playback
    /// </summary>
    /// <param name="request">Playback tracking request</param>
    /// <returns>Success status</returns>
    [HttpPost("playback")]
    public async Task<ActionResult> TrackPlayback([FromBody] VoiceMessagePlaybackRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _voiceMessageService.TrackPlaybackAsync(userId, request);

            if (success)
            {
                _logger.LogDebug("Playback tracked for voice message {VoiceMessageId} by user {UserId}", 
                    request.VoiceMessageId, userId);
                return Ok(new { Success = true, Message = "Playback tracked successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to track playback" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking playback for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a voice message
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{voiceMessageId}")]
    public async Task<ActionResult> DeleteVoiceMessage(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _voiceMessageService.DeleteVoiceMessageAsync(userId, voiceMessageId);

            if (success)
            {
                _logger.LogInformation("Voice message {VoiceMessageId} deleted by user {UserId}", voiceMessageId, userId);
                return Ok(new { Success = true, Message = "Voice message deleted successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to delete voice message or access denied" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting voice message {VoiceMessageId} for user {UserId}", voiceMessageId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get transcription for a voice message
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Transcription text</returns>
    [HttpGet("{voiceMessageId}/transcription")]
    public async Task<ActionResult<string>> GetTranscription(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var transcription = await _voiceMessageService.GetTranscriptionAsync(userId, voiceMessageId);

            if (transcription == null)
            {
                return NotFound("Transcription not available");
            }

            return Ok(new { Transcription = transcription });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transcription for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Request transcription for a voice message
    /// </summary>
    /// <param name="request">Transcription request</param>
    /// <returns>Success status</returns>
    [HttpPost("transcribe")]
    public async Task<ActionResult> RequestTranscription([FromBody] TranscriptionRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _voiceMessageService.RequestTranscriptionAsync(userId, request);

            if (success)
            {
                _logger.LogInformation("Transcription requested for voice message {VoiceMessageId} by user {UserId}", 
                    request.VoiceMessageId, userId);
                return Ok(new { Success = true, Message = "Transcription request submitted successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to request transcription" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting transcription for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get processing status of a voice message
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Processing status</returns>
    [HttpGet("{voiceMessageId}/status")]
    public async Task<ActionResult<VoiceMessageResponse>> GetProcessingStatus(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _voiceMessageService.GetProcessingStatusAsync(userId, voiceMessageId);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing status for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, GetUserId());
            return StatusCode(500, new VoiceMessageResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get waveform data for visualization
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <returns>Waveform data array</returns>
    [HttpGet("{voiceMessageId}/waveform")]
    public async Task<ActionResult<double[]>> GetWaveform(string voiceMessageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var waveformData = await _voiceMessageService.GenerateWaveformAsync(userId, voiceMessageId);

            if (waveformData == null)
            {
                return NotFound("Waveform data not available");
            }

            return Ok(waveformData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waveform for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get voice messages for a specific message
    /// </summary>
    /// <param name="messageId">Parent message ID</param>
    /// <returns>List of voice messages</returns>
    [HttpGet("message/{messageId}")]
    public async Task<ActionResult<List<VoiceMessageDto>>> GetVoiceMessagesForMessage(string messageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var voiceMessages = await _voiceMessageService.GetVoiceMessagesForMessageAsync(userId, messageId);

            return Ok(voiceMessages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice messages for message {MessageId} and user {UserId}", messageId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Convert voice message to different audio format
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <param name="targetFormat">Target audio format</param>
    /// <returns>Converted audio stream</returns>
    [HttpGet("{voiceMessageId}/convert/{targetFormat}")]
    public async Task<ActionResult> ConvertAudioFormat(string voiceMessageId, string targetFormat)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var convertedStream = await _voiceMessageService.ConvertAudioFormatAsync(userId, voiceMessageId, targetFormat);

            if (convertedStream == null)
            {
                return NotFound("Failed to convert audio format");
            }

            var contentType = GetContentType(targetFormat);
            return File(convertedStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting audio format for voice message {VoiceMessageId} to {TargetFormat} for user {UserId}", 
                voiceMessageId, targetFormat, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get voice message statistics for the current user
    /// </summary>
    /// <param name="fromDate">Start date (ISO 8601 format)</param>
    /// <param name="toDate">End date (ISO 8601 format)</param>
    /// <returns>Voice message statistics</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetStatistics([FromQuery] string fromDate, [FromQuery] string toDate)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!DateTimeOffset.TryParse(fromDate, out var from) || !DateTimeOffset.TryParse(toDate, out var to))
            {
                return BadRequest("Invalid date format. Use ISO 8601 format (e.g., 2023-01-01T00:00:00Z)");
            }

            var statistics = await _voiceMessageService.GetVoiceMessageStatisticsAsync(userId, from, to);

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice message statistics for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get the current user's ID from the JWT token
    /// </summary>
    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Get appropriate content type for audio format
    /// </summary>
    private static string GetContentType(string audioFormat)
    {
        return audioFormat.ToLower() switch
        {
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            "ogg" => "audio/ogg",
            "webm" => "audio/webm",
            "m4a" => "audio/mp4",
            _ => "audio/webm"
        };
    }

    /// <summary>
    /// Estimate duration based on file size (rough approximation)
    /// </summary>
    private static double EstimateDuration(long fileSizeBytes)
    {
        // Rough estimate: assume ~64 kbps average bitrate
        var estimatedSeconds = (fileSizeBytes * 8.0) / (64 * 1000);
        return Math.Max(estimatedSeconds, 1.0);
    }
}