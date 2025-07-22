using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for voice message management
/// </summary>
public class VoiceMessageService : IVoiceMessageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VoiceMessageService> _logger;
    private readonly IFileUploadService _fileUploadService;
    private readonly AudioProcessingSettings _audioSettings;
    private readonly string _voiceMessagesPath;

    /// <summary>
    /// Initialize the voice message service
    /// </summary>
    public VoiceMessageService(
        ApplicationDbContext context, 
        ILogger<VoiceMessageService> logger, 
        IFileUploadService fileUploadService,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _fileUploadService = fileUploadService;
        
        // Initialize audio processing settings from configuration
        _audioSettings = configuration.GetSection("AudioProcessing").Get<AudioProcessingSettings>() 
            ?? new AudioProcessingSettings();
        
        _voiceMessagesPath = configuration.GetValue<string>("FileStorage:VoiceMessagesPath") 
            ?? Path.Combine("uploads", "voice-messages");

        // Ensure voice messages directory exists
        Directory.CreateDirectory(_voiceMessagesPath);
    }

    /// <inheritdoc />
    public async Task<VoiceMessageResponse> UploadVoiceMessageAsync(string userId, UploadVoiceMessageRequest request, Stream audioFile)
    {
        try
        {
            // Validate request
            var validationResult = ValidateUploadRequest(request, audioFile);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            // Validate conversation access
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == request.ConversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return new VoiceMessageResponse
                {
                    Success = false,
                    Message = "Access denied to this conversation"
                };
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.{request.AudioFormat}";
            var filePath = Path.Combine(_voiceMessagesPath, fileName);

            // Process audio file
            using var processedAudio = await ProcessAudioAsync(audioFile, _audioSettings);
            
            // Save processed audio to file
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await processedAudio.CopyToAsync(fileStream);
            var fileSizeBytes = fileStream.Length;

            // Create message entity
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = request.ConversationId,
                SenderId = userId,
                MessageType = EnumConverter.ToString(MessageType.Voice),
                Content = "🎤 Voice Message", // Default content for voice messages
                ParentMessageId = request.ParentMessageId,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(MessageStatus.Sent)
            };

            // Create voice message entity
            var voiceMessage = new VoiceMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = message.Id,
                FilePath = filePath,
                OriginalFileName = $"voice_message.{request.AudioFormat}",
                AudioFormat = request.AudioFormat,
                DurationSeconds = request.DurationSeconds,
                FileSizeBytes = fileSizeBytes,
                SampleRate = request.SampleRate,
                BitRate = CalculateBitRate(fileSizeBytes, request.DurationSeconds),
                Channels = 1, // Most voice messages are mono
                IsTranscribed = false,
                IsProcessing = request.EnableTranscription,
                ProcessingStatus = request.EnableTranscription ? "Queued for transcription" : "Processing complete",
                AutoDeleteAfterPlay = request.AutoDeleteAfterPlay,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.Messages.Add(message);
            _context.VoiceMessages.Add(voiceMessage);

            // Update conversation
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);

            if (conversation != null)
            {
                conversation.LastMessageId = message.Id;
                conversation.LastMessageAt = message.CreatedAt;
                conversation.LastMessagePreview = "🎤 Voice Message";
                conversation.MessageCount++;
                conversation.UpdatedAt = DateTimeOffset.UtcNow;

                // Update unread counts
                var otherParticipants = conversation.Participants.Where(p => p.UserId != userId);
                foreach (var participant in otherParticipants)
                {
                    participant.UnreadCount++;
                }
            }

            await _context.SaveChangesAsync();

            // Generate waveform asynchronously if enabled
            if (_audioSettings.GenerateWaveform)
            {
                _ = Task.Run(async () => await GenerateWaveformDataAsync(voiceMessage.Id));
            }

            // Start transcription process if requested
            if (request.EnableTranscription)
            {
                _ = Task.Run(async () => await ProcessTranscriptionAsync(voiceMessage.Id, request.TranscriptionLanguage));
            }

            var voiceMessageDto = await MapToDto(voiceMessage);

            _logger.LogInformation("Voice message {VoiceMessageId} uploaded successfully by user {UserId} to conversation {ConversationId}",
                voiceMessage.Id, userId, request.ConversationId);

            return new VoiceMessageResponse
            {
                Success = true,
                Message = "Voice message uploaded successfully",
                VoiceMessage = voiceMessageDto,
                ProcessingProgress = request.EnableTranscription ? 10 : 100
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading voice message for user {UserId}", userId);
            return new VoiceMessageResponse
            {
                Success = false,
                Message = "Failed to upload voice message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<VoiceMessageResponse> UploadVoiceMessageAsync(string userId, UploadVoiceMessageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.AudioData))
            {
                return new VoiceMessageResponse
                {
                    Success = false,
                    Message = "Audio data is required"
                };
            }

            // Convert base64 to stream
            byte[] audioBytes;
            try
            {
                // Remove data URL prefix if present (e.g., "data:audio/webm;base64,")
                var base64Data = Regex.Replace(request.AudioData, @"^data:audio/[^;]+;base64,", "");
                audioBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                return new VoiceMessageResponse
                {
                    Success = false,
                    Message = "Invalid audio data format"
                };
            }

            using var audioStream = new MemoryStream(audioBytes);
            return await UploadVoiceMessageAsync(userId, request, audioStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading voice message from base64 for user {UserId}", userId);
            return new VoiceMessageResponse
            {
                Success = false,
                Message = "Failed to process audio data"
            };
        }
    }

    /// <inheritdoc />
    public async Task<VoiceMessageDto?> GetVoiceMessageAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == voiceMessageId);

            if (voiceMessage == null)
            {
                return null;
            }

            // Check if user has access to this voice message
            if (!voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return null;
            }

            return await MapToDto(voiceMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice message {VoiceMessageId} for user {UserId}", voiceMessageId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<VoiceMessageDto>> GetVoiceMessagesForMessageAsync(string userId, string messageId)
    {
        try
        {
            var voiceMessages = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .Where(vm => vm.MessageId == messageId &&
                           vm.Message.Conversation.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            var result = new List<VoiceMessageDto>();
            foreach (var voiceMessage in voiceMessages)
            {
                result.Add(await MapToDto(voiceMessage));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice messages for message {MessageId} and user {UserId}", messageId, userId);
            return new List<VoiceMessageDto>();
        }
    }

    /// <inheritdoc />
    public async Task<bool> TrackPlaybackAsync(string userId, VoiceMessagePlaybackRequest request)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == request.VoiceMessageId);

            if (voiceMessage == null || !voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return false;
            }

            // Don't track playback for the sender
            if (voiceMessage.Message.SenderId == userId)
            {
                return true;
            }

            var now = DateTimeOffset.UtcNow;

            if (!voiceMessage.IsPlayed)
            {
                voiceMessage.IsPlayed = true;
                voiceMessage.FirstPlayedAt = now;
            }

            voiceMessage.PlayCount++;
            voiceMessage.LastPlayedAt = now;
            voiceMessage.UpdatedAt = now;

            // Auto-delete if configured and playback completed
            if (voiceMessage.AutoDeleteAfterPlay && request.PlaybackCompleted)
            {
                // Delete the physical file
                if (File.Exists(voiceMessage.FilePath))
                {
                    File.Delete(voiceMessage.FilePath);
                }

                // Mark as deleted in database
                voiceMessage.ProcessingStatus = "Auto-deleted after playback";
            }

            await _context.SaveChangesAsync();

            _logger.LogDebug("Playback tracked for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking playback for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteVoiceMessageAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                .FirstOrDefaultAsync(vm => vm.Id == voiceMessageId);

            if (voiceMessage == null || voiceMessage.Message.SenderId != userId)
            {
                return false;
            }

            // Delete physical file
            if (File.Exists(voiceMessage.FilePath))
            {
                File.Delete(voiceMessage.FilePath);
            }

            // Remove from database
            _context.VoiceMessages.Remove(voiceMessage);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Voice message {VoiceMessageId} deleted by user {UserId}", voiceMessageId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting voice message {VoiceMessageId} for user {UserId}", voiceMessageId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetTranscriptionAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == voiceMessageId);

            if (voiceMessage == null || !voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return null;
            }

            return voiceMessage.TranscriptionText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transcription for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RequestTranscriptionAsync(string userId, TranscriptionRequest request)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == request.VoiceMessageId);

            if (voiceMessage == null || !voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return false;
            }

            if (voiceMessage.IsTranscribed && !request.ForceReTranscribe)
            {
                return true; // Already transcribed
            }

            voiceMessage.IsProcessing = true;
            voiceMessage.ProcessingStatus = "Queued for transcription";
            voiceMessage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            // Start transcription process
            _ = Task.Run(async () => await ProcessTranscriptionAsync(request.VoiceMessageId, request.Language));

            _logger.LogInformation("Transcription requested for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting transcription for voice message {VoiceMessageId} by user {UserId}", 
                request.VoiceMessageId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<Stream?> GetAudioStreamAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == voiceMessageId);

            if (voiceMessage == null || !voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return null;
            }

            if (!File.Exists(voiceMessage.FilePath))
            {
                _logger.LogWarning("Audio file not found for voice message {VoiceMessageId}: {FilePath}", 
                    voiceMessageId, voiceMessage.FilePath);
                return null;
            }

            return new FileStream(voiceMessage.FilePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audio stream for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<VoiceMessageResponse> GetProcessingStatusAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await GetVoiceMessageAsync(userId, voiceMessageId);
            
            if (voiceMessage == null)
            {
                return new VoiceMessageResponse
                {
                    Success = false,
                    Message = "Voice message not found"
                };
            }

            var processingProgress = 100;
            if (voiceMessage.IsProcessing)
            {
                processingProgress = voiceMessage.IsTranscribed ? 90 : 50;
            }

            return new VoiceMessageResponse
            {
                Success = true,
                Message = voiceMessage.ProcessingStatus ?? "Processing complete",
                VoiceMessage = voiceMessage,
                ProcessingProgress = processingProgress
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing status for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, userId);
            return new VoiceMessageResponse
            {
                Success = false,
                Message = "Failed to get processing status"
            };
        }
    }

    /// <inheritdoc />
    public async Task<double[]?> GenerateWaveformAsync(string userId, string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(vm => vm.Id == voiceMessageId);

            if (voiceMessage == null || !voiceMessage.Message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return null;
            }

            // Return existing waveform if available
            if (!string.IsNullOrEmpty(voiceMessage.WaveformData))
            {
                try
                {
                    return JsonSerializer.Deserialize<double[]>(voiceMessage.WaveformData);
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Invalid waveform data format for voice message {VoiceMessageId}", voiceMessageId);
                }
            }

            // Generate new waveform data
            var waveformData = await GenerateWaveformDataAsync(voiceMessageId);
            return waveformData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating waveform for voice message {VoiceMessageId} and user {UserId}", 
                voiceMessageId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<object> GetVoiceMessageStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        try
        {
            var voiceMessages = await _context.VoiceMessages
                .Include(vm => vm.Message)
                    .ThenInclude(m => m.Conversation)
                        .ThenInclude(c => c.Participants)
                .Where(vm => vm.Message.Conversation.Participants.Any(p => p.UserId == userId) &&
                           vm.CreatedAt >= fromDate && vm.CreatedAt <= toDate)
                .ToListAsync();

            var statistics = new
            {
                TotalVoiceMessages = voiceMessages.Count,
                VoiceMessagesSent = voiceMessages.Count(vm => vm.Message.SenderId == userId),
                VoiceMessagesReceived = voiceMessages.Count(vm => vm.Message.SenderId != userId),
                TotalDuration = voiceMessages.Sum(vm => vm.DurationSeconds),
                AverageDuration = voiceMessages.Any() ? voiceMessages.Average(vm => vm.DurationSeconds) : 0,
                TotalFileSize = voiceMessages.Sum(vm => vm.FileSizeBytes),
                TranscribedMessages = voiceMessages.Count(vm => vm.IsTranscribed),
                PlayedMessages = voiceMessages.Count(vm => vm.IsPlayed),
                TotalPlayCount = voiceMessages.Sum(vm => vm.PlayCount),
                AudioFormats = voiceMessages.GroupBy(vm => vm.AudioFormat)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AutoDeletedMessages = voiceMessages.Count(vm => vm.AutoDeleteAfterPlay),
                ProcessingErrors = voiceMessages.Count(vm => vm.ProcessingStatus?.Contains("error", StringComparison.OrdinalIgnoreCase) == true)
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voice message statistics for user {UserId}", userId);
            return new { Error = "Failed to retrieve statistics" };
        }
    }

    /// <inheritdoc />
    public async Task<Stream?> ConvertAudioFormatAsync(string userId, string voiceMessageId, string targetFormat)
    {
        try
        {
            var audioStream = await GetAudioStreamAsync(userId, voiceMessageId);
            if (audioStream == null)
            {
                return null;
            }

            // For now, return the original stream
            // In a production system, you would use FFmpeg or similar to convert the audio format
            _logger.LogInformation("Audio format conversion requested from voice message {VoiceMessageId} to {TargetFormat} by user {UserId}", 
                voiceMessageId, targetFormat, userId);

            return audioStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting audio format for voice message {VoiceMessageId} to {TargetFormat} for user {UserId}", 
                voiceMessageId, targetFormat, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<Stream> ProcessAudioAsync(Stream audioStream, AudioProcessingSettings settings)
    {
        try
        {
            // Basic audio processing implementation
            // In a production system, you would use FFmpeg, NAudio, or similar libraries for advanced processing

            var processedStream = new MemoryStream();
            await audioStream.CopyToAsync(processedStream);
            processedStream.Position = 0;

            _logger.LogDebug("Audio processing completed with settings: Quality={Quality}, GenerateWaveform={GenerateWaveform}", 
                settings.AudioQuality, settings.GenerateWaveform);

            return processedStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audio stream");
            throw;
        }
    }

    /// <summary>
    /// Validate voice message upload request
    /// </summary>
    private VoiceMessageResponse ValidateUploadRequest(UploadVoiceMessageRequest request, Stream audioFile)
    {
        if (string.IsNullOrEmpty(request.ConversationId))
        {
            return new VoiceMessageResponse
            {
                Success = false,
                Message = "Conversation ID is required"
            };
        }

        if (audioFile.Length > _audioSettings.MaxFileSizeBytes)
        {
            return new VoiceMessageResponse
            {
                Success = false,
                Message = $"File size exceeds maximum limit of {_audioSettings.MaxFileSizeBytes / (1024 * 1024)} MB"
            };
        }

        if (request.DurationSeconds > _audioSettings.MaxDurationSeconds)
        {
            return new VoiceMessageResponse
            {
                Success = false,
                Message = $"Duration exceeds maximum limit of {_audioSettings.MaxDurationSeconds / 60:F1} minutes"
            };
        }

        if (!_audioSettings.SupportedFormats.Contains(request.AudioFormat.ToLower()))
        {
            return new VoiceMessageResponse
            {
                Success = false,
                Message = $"Audio format '{request.AudioFormat}' is not supported"
            };
        }

        return new VoiceMessageResponse { Success = true };
    }

    /// <summary>
    /// Calculate bit rate from file size and duration
    /// </summary>
    private static int CalculateBitRate(long fileSizeBytes, double durationSeconds)
    {
        if (durationSeconds <= 0) return 0;
        
        var bitsTotal = fileSizeBytes * 8;
        var bitRate = (int)(bitsTotal / durationSeconds / 1000); // kbps
        return Math.Max(bitRate, 1);
    }

    /// <summary>
    /// Generate waveform visualization data
    /// </summary>
    private async Task<double[]?> GenerateWaveformDataAsync(string voiceMessageId)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages.FindAsync(voiceMessageId);
            if (voiceMessage == null || !File.Exists(voiceMessage.FilePath))
            {
                return null;
            }

            // Simple waveform generation (in production, use proper audio processing library)
            var waveformPoints = new List<double>();
            var random = new Random();
            
            // Generate mock waveform data based on duration
            var pointsCount = Math.Min((int)(voiceMessage.DurationSeconds * 10), 1000);
            for (int i = 0; i < pointsCount; i++)
            {
                waveformPoints.Add(random.NextDouble());
            }

            var waveformData = waveformPoints.ToArray();
            
            // Save waveform data to database
            voiceMessage.WaveformData = JsonSerializer.Serialize(waveformData);
            voiceMessage.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogDebug("Waveform generated for voice message {VoiceMessageId} with {PointCount} points", 
                voiceMessageId, pointsCount);

            return waveformData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating waveform for voice message {VoiceMessageId}", voiceMessageId);
            return null;
        }
    }

    /// <summary>
    /// Process transcription for voice message
    /// </summary>
    private async Task ProcessTranscriptionAsync(string voiceMessageId, string? language = null)
    {
        try
        {
            var voiceMessage = await _context.VoiceMessages.FindAsync(voiceMessageId);
            if (voiceMessage == null)
            {
                return;
            }

            voiceMessage.ProcessingStatus = "Processing transcription...";
            await _context.SaveChangesAsync();

            // Simulate transcription processing
            await Task.Delay(2000);

            // Mock transcription result (in production, use speech-to-text service)
            voiceMessage.IsTranscribed = true;
            voiceMessage.TranscriptionText = "This is a mock transcription of the voice message.";
            voiceMessage.TranscriptionConfidence = 0.95;
            voiceMessage.TranscriptionLanguage = language ?? "en-US";
            voiceMessage.IsProcessing = false;
            voiceMessage.ProcessingStatus = "Transcription complete";
            voiceMessage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Transcription completed for voice message {VoiceMessageId}", voiceMessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transcription for voice message {VoiceMessageId}", voiceMessageId);
            
            var voiceMessage = await _context.VoiceMessages.FindAsync(voiceMessageId);
            if (voiceMessage != null)
            {
                voiceMessage.IsProcessing = false;
                voiceMessage.ProcessingStatus = $"Transcription failed: {ex.Message}";
                voiceMessage.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Map VoiceMessage entity to DTO
    /// </summary>
    private async Task<VoiceMessageDto> MapToDto(VoiceMessage voiceMessage)
    {
        double[]? waveformData = null;
        if (!string.IsNullOrEmpty(voiceMessage.WaveformData))
        {
            try
            {
                waveformData = JsonSerializer.Deserialize<double[]>(voiceMessage.WaveformData);
            }
            catch (JsonException)
            {
                // Ignore invalid waveform data
            }
        }

        return new VoiceMessageDto
        {
            Id = voiceMessage.Id,
            MessageId = voiceMessage.MessageId,
            AudioUrl = $"/api/v1/VoiceMessage/{voiceMessage.Id}/audio", // URL endpoint for audio playback
            OriginalFileName = voiceMessage.OriginalFileName,
            AudioFormat = voiceMessage.AudioFormat,
            DurationSeconds = voiceMessage.DurationSeconds,
            FileSizeBytes = voiceMessage.FileSizeBytes,
            SampleRate = voiceMessage.SampleRate,
            BitRate = voiceMessage.BitRate,
            Channels = voiceMessage.Channels,
            IsTranscribed = voiceMessage.IsTranscribed,
            TranscriptionText = voiceMessage.TranscriptionText,
            TranscriptionConfidence = voiceMessage.TranscriptionConfidence,
            TranscriptionLanguage = voiceMessage.TranscriptionLanguage,
            IsProcessing = voiceMessage.IsProcessing,
            ProcessingStatus = voiceMessage.ProcessingStatus,
            WaveformData = waveformData,
            IsPlayed = voiceMessage.IsPlayed,
            PlayCount = voiceMessage.PlayCount,
            FirstPlayedAt = voiceMessage.FirstPlayedAt,
            LastPlayedAt = voiceMessage.LastPlayedAt,
            CreatedAt = voiceMessage.CreatedAt
        };
    }
}