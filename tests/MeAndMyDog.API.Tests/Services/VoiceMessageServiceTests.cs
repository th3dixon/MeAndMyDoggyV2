using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Implementations;
using Moq;
using Xunit;

namespace MeAndMyDog.API.Tests.Services;

/// <summary>
/// Unit tests for VoiceMessageService
/// </summary>
public class VoiceMessageServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly VoiceMessageService _voiceMessageService;
    private readonly Mock<ILogger<VoiceMessageService>> _mockLogger;

    public VoiceMessageServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<VoiceMessageService>>();
        _voiceMessageService = new VoiceMessageService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var conversation = new Conversation
        {
            Id = "test-conversation-1",
            Name = "Test Conversation",
            ConversationType = "Direct",
            CreatedBy = "user1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var participant1 = new ConversationParticipant
        {
            Id = "participant-1",
            ConversationId = conversation.Id,
            UserId = "user1",
            Role = "Admin",
            JoinedAt = DateTimeOffset.UtcNow
        };

        var participant2 = new ConversationParticipant
        {
            Id = "participant-2",
            ConversationId = conversation.Id,
            UserId = "user2",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        _context.Conversations.Add(conversation);
        _context.ConversationParticipants.AddRange(participant1, participant2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task StartRecordingAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new StartRecordingRequest
        {
            ConversationId = "test-conversation-1",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = 300
        };

        // Act
        var result = await _voiceMessageService.StartRecordingAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.VoiceMessage);
        Assert.Equal("user1", result.VoiceMessage.SenderId);
        Assert.Equal("test-conversation-1", result.VoiceMessage.ConversationId);
        Assert.Equal(VoiceRecordingStatus.Recording, result.VoiceMessage.Status);

        // Verify recording was saved to database
        var savedRecording = await _context.VoiceMessages
            .FirstOrDefaultAsync(v => v.Id == result.VoiceMessage.Id);
        Assert.NotNull(savedRecording);
        Assert.Equal("Recording", savedRecording.Status);
    }

    [Fact]
    public async Task StartRecordingAsync_UserNotInConversation_ReturnsFailed()
    {
        // Arrange
        var request = new StartRecordingRequest
        {
            ConversationId = "test-conversation-1",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = 300
        };

        // Act
        var result = await _voiceMessageService.StartRecordingAsync("user3", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not a participant", result.Message);
    }

    [Fact]
    public async Task StartRecordingAsync_InvalidConversation_ReturnsFailed()
    {
        // Arrange
        var request = new StartRecordingRequest
        {
            ConversationId = "non-existent-conversation",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = 300
        };

        // Act
        var result = await _voiceMessageService.StartRecordingAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Conversation not found", result.Message);
    }

    [Fact]
    public async Task StopRecordingAsync_ValidRecording_ReturnsSuccess()
    {
        // Arrange
        var startRequest = new StartRecordingRequest
        {
            ConversationId = "test-conversation-1",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = 300
        };
        var startResult = await _voiceMessageService.StartRecordingAsync("user1", startRequest);
        Assert.True(startResult.Success);

        var stopRequest = new StopRecordingRequest
        {
            AudioData = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 }), // Mock audio data
            DurationSeconds = 15.5,
            FileSize = 12345
        };

        // Act
        var result = await _voiceMessageService.StopRecordingAsync("user1", startResult.VoiceMessage.Id, stopRequest);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.VoiceMessage);
        Assert.Equal(VoiceRecordingStatus.Completed, result.VoiceMessage.Status);
        Assert.Equal(15.5, result.VoiceMessage.DurationSeconds);
        Assert.NotEmpty(result.VoiceMessage.AudioUrl);

        // Verify recording was updated in database
        var updatedRecording = await _context.VoiceMessages
            .FirstOrDefaultAsync(v => v.Id == startResult.VoiceMessage.Id);
        Assert.NotNull(updatedRecording);
        Assert.Equal("Completed", updatedRecording.Status);
        Assert.NotNull(updatedRecording.CompletedAt);
    }

    [Fact]
    public async Task StopRecordingAsync_NotRecording_ReturnsFailed()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed);
        var stopRequest = new StopRecordingRequest
        {
            AudioData = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 }),
            DurationSeconds = 15.5,
            FileSize = 12345
        };

        // Act
        var result = await _voiceMessageService.StopRecordingAsync("user1", voiceMessage.Id, stopRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not currently recording", result.Message);
    }

    [Fact]
    public async Task CancelRecordingAsync_ValidRecording_ReturnsSuccess()
    {
        // Arrange
        var startRequest = new StartRecordingRequest
        {
            ConversationId = "test-conversation-1",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = 300
        };
        var startResult = await _voiceMessageService.StartRecordingAsync("user1", startRequest);
        Assert.True(startResult.Success);

        // Act
        var result = await _voiceMessageService.CancelRecordingAsync("user1", startResult.VoiceMessage.Id);

        // Assert
        Assert.True(result);

        // Verify recording was cancelled in database
        var cancelledRecording = await _context.VoiceMessages
            .FirstOrDefaultAsync(v => v.Id == startResult.VoiceMessage.Id);
        Assert.NotNull(cancelledRecording);
        Assert.Equal("Cancelled", cancelledRecording.Status);
        Assert.NotNull(cancelledRecording.CancelledAt);
    }

    [Fact]
    public async Task GetVoiceMessageAsync_ValidMessage_ReturnsMessage()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed);

        // Act
        var result = await _voiceMessageService.GetVoiceMessageAsync("user1", voiceMessage.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(voiceMessage.Id, result.Id);
        Assert.Equal(voiceMessage.SenderId, result.SenderId);
        Assert.Equal(voiceMessage.ConversationId, result.ConversationId);
    }

    [Fact]
    public async Task GetVoiceMessageAsync_UserNotInConversation_ReturnsNull()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed);

        // Act
        var result = await _voiceMessageService.GetVoiceMessageAsync("user3", voiceMessage.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetConversationVoiceMessagesAsync_ValidConversation_ReturnsMessages()
    {
        // Arrange
        await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user1");
        await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user2");
        await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user1");

        // Act
        var result = await _voiceMessageService.GetConversationVoiceMessagesAsync("user1", "test-conversation-1", 1, 10);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.VoiceMessages);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task DeleteVoiceMessageAsync_ValidMessage_ReturnsSuccess()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed);

        // Act
        var result = await _voiceMessageService.DeleteVoiceMessageAsync("user1", voiceMessage.Id);

        // Assert
        Assert.True(result);

        // Verify message was marked as deleted
        var deletedMessage = await _context.VoiceMessages
            .FirstOrDefaultAsync(v => v.Id == voiceMessage.Id);
        Assert.NotNull(deletedMessage);
        Assert.True(deletedMessage.IsDeleted);
        Assert.NotNull(deletedMessage.DeletedAt);
    }

    [Fact]
    public async Task DeleteVoiceMessageAsync_UserNotSender_ReturnsFalse()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user1");

        // Act
        var result = await _voiceMessageService.DeleteVoiceMessageAsync("user2", voiceMessage.Id);

        // Assert
        Assert.False(result);

        // Verify message was not deleted
        var unchangedMessage = await _context.VoiceMessages
            .FirstOrDefaultAsync(v => v.Id == voiceMessage.Id);
        Assert.NotNull(unchangedMessage);
        Assert.False(unchangedMessage.IsDeleted);
    }

    [Fact]
    public async Task GenerateTranscriptionAsync_ValidVoiceMessage_ReturnsTranscription()
    {
        // Arrange
        var voiceMessage = await CreateTestVoiceMessage(VoiceRecordingStatus.Completed);

        // Act
        var result = await _voiceMessageService.GenerateTranscriptionAsync("user1", voiceMessage.Id);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Transcription);
        Assert.NotEmpty(result.Transcription.TranscribedText);
        Assert.True(result.Transcription.Confidence > 0);

        // Verify transcription was saved to database
        var savedTranscription = await _context.VoiceTranscriptions
            .FirstOrDefaultAsync(t => t.VoiceMessageId == voiceMessage.Id);
        Assert.NotNull(savedTranscription);
    }

    [Fact]
    public async Task ProcessAudioAsync_ValidAudioData_ReturnsProcessedAudio()
    {
        // Arrange
        var audioData = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        var request = new ProcessAudioRequest
        {
            AudioData = audioData,
            TargetFormat = AudioFormat.MP3,
            Quality = VoiceQuality.High,
            NoiseReduction = true,
            VolumeNormalization = true
        };

        // Act
        var result = await _voiceMessageService.ProcessAudioAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ProcessedAudio);
        Assert.NotEmpty(result.ProcessedAudio.AudioData);
        Assert.Equal(AudioFormat.MP3, result.ProcessedAudio.Format);
        Assert.True(result.ProcessedAudio.FileSize > 0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(3601)] // Over 1 hour
    public async Task StartRecordingAsync_InvalidDuration_ReturnsFailed(int maxDurationSeconds)
    {
        // Arrange
        var request = new StartRecordingRequest
        {
            ConversationId = "test-conversation-1",
            Quality = VoiceQuality.High,
            MaxDurationSeconds = maxDurationSeconds
        };

        // Act
        var result = await _voiceMessageService.StartRecordingAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("duration", result.Message.ToLower());
    }

    [Fact]
    public async Task GetVoiceMessageStatisticsAsync_WithData_ReturnsStats()
    {
        // Arrange
        await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user1");
        await CreateTestVoiceMessage(VoiceRecordingStatus.Completed, "user2");
        await CreateTestVoiceMessage(VoiceRecordingStatus.Cancelled, "user1");

        // Act
        var result = await _voiceMessageService.GetVoiceMessageStatisticsAsync("user1");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalRecordings >= 2);
        Assert.True(result.CompletedRecordings >= 2);
        Assert.True(result.CancelledRecordings >= 1);
        Assert.True(result.TotalDurationSeconds >= 0);
    }

    private async Task<VoiceMessage> CreateTestVoiceMessage(VoiceRecordingStatus status, string senderId = "user1")
    {
        var voiceMessage = new VoiceMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = "test-conversation-1",
            SenderId = senderId,
            Status = status.ToString(),
            Quality = "High",
            DurationSeconds = 15.5,
            FileSize = 12345,
            AudioUrl = "https://example.com/audio.mp3",
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            CompletedAt = status == VoiceRecordingStatus.Completed ? DateTimeOffset.UtcNow : null,
            CancelledAt = status == VoiceRecordingStatus.Cancelled ? DateTimeOffset.UtcNow : null
        };

        _context.VoiceMessages.Add(voiceMessage);
        await _context.SaveChangesAsync();
        return voiceMessage;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}