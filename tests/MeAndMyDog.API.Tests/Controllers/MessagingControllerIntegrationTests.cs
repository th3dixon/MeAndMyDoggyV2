using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace MeAndMyDog.API.Tests.Controllers;

/// <summary>
/// Integration tests for MessagingController
/// </summary>
public class MessagingControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public MessagingControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("MessagingIntegrationTestDb");
                });
            });
        });

        _client = _factory.CreateClient();

        // Get the database context
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed test data
        SeedTestData().Wait();

        // Set up authentication
        SetupAuthentication();
    }

    private async Task SeedTestData()
    {
        // Create test users
        var user1 = new ApplicationUser
        {
            Id = "user1",
            UserName = "testuser1@example.com",
            Email = "testuser1@example.com",
            EmailConfirmed = true
        };

        var user2 = new ApplicationUser
        {
            Id = "user2",
            UserName = "testuser2@example.com",
            Email = "testuser2@example.com",
            EmailConfirmed = true
        };

        _context.Users.AddRange(user1, user2);

        // Create test conversation
        var conversation = new Conversation
        {
            Id = "conv-test-1",
            Name = "Test Conversation",
            ConversationType = "Direct",
            CreatedBy = "user1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var participant1 = new ConversationParticipant
        {
            Id = "part-1",
            ConversationId = conversation.Id,
            UserId = "user1",
            Role = "Admin",
            JoinedAt = DateTimeOffset.UtcNow
        };

        var participant2 = new ConversationParticipant
        {
            Id = "part-2",
            ConversationId = conversation.Id,
            UserId = "user2",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        _context.Conversations.Add(conversation);
        _context.ConversationParticipants.AddRange(participant1, participant2);

        await _context.SaveChangesAsync();
    }

    private void SetupAuthentication()
    {
        // In a real scenario, you would generate a proper JWT token
        // For testing, we'll use a mock authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "mock-jwt-token");
        _client.DefaultRequestHeaders.Add("X-User-Id", "user1"); // Mock user ID for testing
    }

    [Fact]
    public async Task SendMessage_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "conv-test-1",
            Content = "Hello, this is a test message!",
            MessageType = MessageType.Text
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/messaging/send", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SendMessageResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Message);
        Assert.Equal("Hello, this is a test message!", result.Message.Content);
    }

    [Fact]
    public async Task SendMessage_InvalidConversation_ReturnsBadRequest()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "invalid-conv-id",
            Content = "Test message",
            MessageType = MessageType.Text
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/messaging/send", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendMessage_EmptyContent_ReturnsBadRequest()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "conv-test-1",
            Content = "",
            MessageType = MessageType.Text
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/messaging/send", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversationMessages_ValidRequest_ReturnsMessages()
    {
        // Arrange
        // First send a message
        await SendTestMessage("conv-test-1", "Test message for retrieval");

        // Act
        var response = await _client.GetAsync("/api/v1/messaging/conversations/conv-test-1/messages?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetMessagesResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task GetConversationMessages_InvalidConversation_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/messaging/conversations/invalid-conv/messages?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MarkMessageAsRead_ValidMessage_ReturnsSuccess()
    {
        // Arrange
        var messageId = await SendTestMessage("conv-test-1", "Message to mark as read");

        // Act
        var response = await _client.PostAsync($"/api/v1/messaging/messages/{messageId}/read", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MarkMessageAsRead_InvalidMessage_ReturnsNotFound()
    {
        // Act
        var response = await _client.PostAsync("/api/v1/messaging/messages/invalid-message-id/read", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EditMessage_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var messageId = await SendTestMessage("conv-test-1", "Original message content");

        var request = new EditMessageRequest
        {
            Content = "Edited message content"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/messaging/messages/{messageId}/edit", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EditMessageResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Edited message content", result.Message.Content);
        Assert.True(result.Message.IsEdited);
    }

    [Fact]
    public async Task DeleteMessage_ValidMessage_ReturnsSuccess()
    {
        // Arrange
        var messageId = await SendTestMessage("conv-test-1", "Message to delete");

        // Act
        var response = await _client.DeleteAsync($"/api/v1/messaging/messages/{messageId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMessage_InvalidMessage_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/messaging/messages/invalid-message-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMessage_ValidMessage_ReturnsMessage()
    {
        // Arrange
        var messageId = await SendTestMessage("conv-test-1", "Message to retrieve");

        // Act
        var response = await _client.GetAsync($"/api/v1/messaging/messages/{messageId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MessageDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(messageId, result.Id);
        Assert.Equal("Message to retrieve", result.Content);
    }

    [Fact]
    public async Task GetMessage_InvalidMessage_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/messaging/messages/invalid-message-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendMessage_WithAttachments_ReturnsSuccess()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            ConversationId = "conv-test-1",
            Content = "Message with attachment",
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

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/messaging/send", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SendMessageResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Message.Attachments);
        Assert.Equal("test.pdf", result.Message.Attachments.First().FileName);
    }

    [Fact]
    public async Task SendMessage_WithReplyTo_ReturnsSuccess()
    {
        // Arrange
        var originalMessageId = await SendTestMessage("conv-test-1", "Original message");

        var request = new SendMessageRequest
        {
            ConversationId = "conv-test-1",
            Content = "This is a reply",
            MessageType = MessageType.Text,
            ReplyToMessageId = originalMessageId
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/messaging/send", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SendMessageResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(originalMessageId, result.Message.ReplyToMessageId);
    }

    [Fact]
    public async Task GetConversationMessages_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        // Send multiple messages
        for (int i = 1; i <= 15; i++)
        {
            await SendTestMessage("conv-test-1", $"Test message {i}");
        }

        // Act
        var response = await _client.GetAsync("/api/v1/messaging/conversations/conv-test-1/messages?page=2&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetMessagesResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(5, result.Messages.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.True(result.TotalCount >= 15);
    }

    [Fact]
    public async Task GetConversationMessages_EmptyConversation_ReturnsEmptyList()
    {
        // Create a new empty conversation
        var emptyConversation = new Conversation
        {
            Id = "empty-conv",
            Name = "Empty Conversation",
            ConversationType = "Direct",
            CreatedBy = "user1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var participant = new ConversationParticipant
        {
            Id = "empty-part",
            ConversationId = emptyConversation.Id,
            UserId = "user1",
            Role = "Admin",
            JoinedAt = DateTimeOffset.UtcNow
        };

        _context.Conversations.Add(emptyConversation);
        _context.ConversationParticipants.Add(participant);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/messaging/conversations/empty-conv/messages?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetMessagesResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Messages);
        Assert.Equal(0, result.TotalCount);
    }

    private async Task<string> SendTestMessage(string conversationId, string content)
    {
        var request = new SendMessageRequest
        {
            ConversationId = conversationId,
            Content = content,
            MessageType = MessageType.Text
        };

        var json = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/messaging/send", stringContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SendMessageResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Message?.Id ?? throw new InvalidOperationException("Failed to send test message");
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}