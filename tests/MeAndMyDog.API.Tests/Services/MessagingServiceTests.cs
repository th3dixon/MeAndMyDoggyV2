using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Implementations;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Tests.TestModels;
using Moq;
using Xunit;

namespace MeAndMyDog.API.Tests.Services;

/// <summary>
/// Unit tests for MessagingService
/// </summary>
public class MessagingServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MessagingService _messagingService;
    private readonly Mock<ILogger<MessagingService>> _mockLogger;
    private readonly Mock<IEncryptionService> _mockEncryptionService;

    public MessagingServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<MessagingService>>();
        _mockEncryptionService = new Mock<IEncryptionService>();

        // Setup mock encryption service
        _mockEncryptionService.Setup(x => x.EncryptMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string conversationId, string content) => new EncryptionResult
            {
                Success = true,
                EncryptedContent = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content)),
                KeyId = "test-key-id"
            });

        _mockEncryptionService.Setup(x => x.DecryptMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string keyId, string encryptedContent) => new DecryptionResult
            {
                Success = true,
                DecryptedContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedContent))
            });

        _messagingService = new MessagingService(_context, _mockLogger.Object, _mockEncryptionService.Object);

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
    public async Task SendMessageAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "test-conversation-1",
            Content = "Hello, World!",
            MessageType = MessageType.Text
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Message);
        Assert.Equal("Hello, World!", result.Message.Content);
        Assert.Equal(MessageType.Text, result.Message.MessageType);
        Assert.Equal("user1", result.Message.SenderId);

        // Verify message was saved to database
        var savedMessage = await _context.Messages.FirstOrDefaultAsync(m => m.Id == result.Message.Id);
        Assert.NotNull(savedMessage);
        Assert.Equal("Hello, World!", savedMessage.Content);
    }

    [Fact]
    public async Task SendMessageAsync_InvalidConversation_ReturnsFailed()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "non-existent-conversation",
            Content = "Hello, World!",
            MessageType = MessageType.Text
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Conversation not found", result.Message);
    }

    [Fact]
    public async Task SendMessageAsync_UserNotInConversation_ReturnsFailed()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "test-conversation-1",
            Content = "Hello, World!",
            MessageType = MessageType.Text
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user3", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not a participant", result.Message);
    }

    [Fact]
    public async Task SendMessageAsync_EmptyContent_ReturnsFailed()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "test-conversation-1",
            Content = "",
            MessageType = MessageType.Text
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Message content cannot be empty", result.Message);
    }

    [Fact]
    public async Task GetConversationMessagesAsync_ValidRequest_ReturnsMessages()
    {
        // Arrange
        await SeedMessagesForConversation();

        // Act
        var result = await _messagingService.GetConversationMessagesAsync("user1", "test-conversation-1", 1, 10);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Messages);
        Assert.True(result.TotalCount > 0);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetConversationMessagesAsync_UserNotInConversation_ReturnsFailed()
    {
        // Arrange
        await SeedMessagesForConversation();

        // Act
        var result = await _messagingService.GetConversationMessagesAsync("user3", "test-conversation-1", 1, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not a participant", result.Message);
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var message = await CreateTestMessage();

        // Act
        var result = await _messagingService.MarkMessageAsReadAsync("user2", message.Id);

        // Assert
        Assert.True(result);

        // Verify read receipt was created
        var readReceipt = await _context.MessageReadReceipts
            .FirstOrDefaultAsync(r => r.MessageId == message.Id && r.UserId == "user2");
        Assert.NotNull(readReceipt);
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_AlreadyRead_ReturnsTrue()
    {
        // Arrange
        var message = await CreateTestMessage();
        await _messagingService.MarkMessageAsReadAsync("user2", message.Id);

        // Act - Mark as read again
        var result = await _messagingService.MarkMessageAsReadAsync("user2", message.Id);

        // Assert
        Assert.True(result);

        // Verify only one read receipt exists
        var readReceipts = await _context.MessageReadReceipts
            .CountAsync(r => r.MessageId == message.Id && r.UserId == "user2");
        Assert.Equal(1, readReceipts);
    }

    [Fact]
    public async Task DeleteMessageAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var message = await CreateTestMessage();

        // Act
        var result = await _messagingService.DeleteMessageAsync("user1", message.Id);

        // Assert
        Assert.True(result);

        // Verify message is marked as deleted
        var deletedMessage = await _context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
        Assert.NotNull(deletedMessage);
        Assert.True(deletedMessage.IsDeleted);
        Assert.NotNull(deletedMessage.DeletedAt);
    }

    [Fact]
    public async Task DeleteMessageAsync_UserNotSender_ReturnsFalse()
    {
        // Arrange
        var message = await CreateTestMessage();

        // Act
        var result = await _messagingService.DeleteMessageAsync("user2", message.Id);

        // Assert
        Assert.False(result);

        // Verify message is not deleted
        var unchangedMessage = await _context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
        Assert.NotNull(unchangedMessage);
        Assert.False(unchangedMessage.IsDeleted);
    }

    [Fact]
    public async Task EditMessageAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var message = await CreateTestMessage();
        var editRequest = new EditMessageRequest
        {
            Content = "Edited message content"
        };

        // Act
        var result = await _messagingService.EditMessageAsync("user1", message.Id, editRequest);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Edited message content", result.Message.Content);
        Assert.True(result.Message.IsEdited);
        Assert.NotNull(result.Message.LastEditedAt);

        // Verify message was updated in database
        var updatedMessage = await _context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
        Assert.NotNull(updatedMessage);
        Assert.Equal("Edited message content", updatedMessage.Content);
        Assert.True(updatedMessage.IsEdited);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task EditMessageAsync_EmptyContent_ReturnsFailed(string content)
    {
        // Arrange
        var message = await CreateTestMessage();
        var editRequest = new EditMessageRequest
        {
            Content = content
        };

        // Act
        var result = await _messagingService.EditMessageAsync("user1", message.Id, editRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("cannot be empty", result.Message);
    }

    [Fact]
    public async Task GetMessageAsync_ValidId_ReturnsMessage()
    {
        // Arrange
        var message = await CreateTestMessage();

        // Act
        var result = await _messagingService.GetMessageAsync("user1", message.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(message.Id, result.Id);
        Assert.Equal(message.Content, result.Content);
    }

    [Fact]
    public async Task GetMessageAsync_UserNotInConversation_ReturnsNull()
    {
        // Arrange
        var message = await CreateTestMessage();

        // Act
        var result = await _messagingService.GetMessageAsync("user3", message.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMessageAsync_NonExistentMessage_ReturnsNull()
    {
        // Act
        var result = await _messagingService.GetMessageAsync("user1", "non-existent-message");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SendMessageAsync_WithAttachments_ReturnsSuccess()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "test-conversation-1",
            Content = "Message with attachments",
            MessageType = MessageType.Text,
            Attachments = new List<MessageAttachmentDto>
            {
                new MessageAttachmentDto
                {
                    FileName = "test.pdf",
                    FileSize = 1024,
                    MimeType = "application/pdf",
                    Url = "https://example.com/test.pdf"
                }
            }
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Message.Attachments);
        Assert.Equal("test.pdf", result.Message.Attachments.First().FileName);
    }

    [Fact]
    public async Task SendMessageAsync_WithReplyTo_ReturnsSuccess()
    {
        // Arrange
        var originalMessage = await CreateTestMessage();
        var request = new SendMessageRequest
        {
            ConversationId = "test-conversation-1",
            Content = "This is a reply",
            MessageType = MessageType.Text,
            ReplyToMessageId = originalMessage.Id
        };

        // Act
        var result = await _messagingService.SendMessageAsync("user2", request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(originalMessage.Id, result.Message.ReplyToMessageId);
    }

    private async Task<Message> CreateTestMessage()
    {
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = "test-conversation-1",
            SenderId = "user1",
            Content = "Test message content",
            MessageType = "Text",
            SentAt = DateTimeOffset.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    private async Task SeedMessagesForConversation()
    {
        var messages = new List<Message>();
        for (int i = 1; i <= 5; i++)
        {
            messages.Add(new Message
            {
                Id = $"message-{i}",
                ConversationId = "test-conversation-1",
                SenderId = i % 2 == 0 ? "user1" : "user2",
                Content = $"Test message {i}",
                MessageType = "Text",
                SentAt = DateTimeOffset.UtcNow.AddMinutes(-i)
            });
        }

        _context.Messages.AddRange(messages);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}