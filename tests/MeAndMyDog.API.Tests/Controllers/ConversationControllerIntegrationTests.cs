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
/// Integration tests for ConversationController
/// </summary>
public class ConversationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public ConversationControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("ConversationIntegrationTestDb");
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
        var users = new[]
        {
            new ApplicationUser { Id = "user1", UserName = "user1@example.com", Email = "user1@example.com", EmailConfirmed = true },
            new ApplicationUser { Id = "user2", UserName = "user2@example.com", Email = "user2@example.com", EmailConfirmed = true },
            new ApplicationUser { Id = "user3", UserName = "user3@example.com", Email = "user3@example.com", EmailConfirmed = true }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    private void SetupAuthentication()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "mock-jwt-token");
        _client.DefaultRequestHeaders.Add("X-User-Id", "user1");
    }

    [Fact]
    public async Task CreateConversation_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Test Group Chat",
            ConversationType = ConversationType.Group,
            ParticipantIds = new List<string> { "user2", "user3" }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/conversations", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateConversationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Conversation);
        Assert.Equal("Test Group Chat", result.Conversation.Name);
        Assert.Equal(ConversationType.Group, result.Conversation.ConversationType);
        Assert.Equal(3, result.Conversation.Participants.Count); // user1 + user2 + user3
    }

    [Fact]
    public async Task CreateConversation_DirectConversation_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Direct Chat",
            ConversationType = ConversationType.Direct,
            ParticipantIds = new List<string> { "user2" }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/conversations", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateConversationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(ConversationType.Direct, result.Conversation.ConversationType);
        Assert.Equal(2, result.Conversation.Participants.Count);
    }

    [Fact]
    public async Task CreateConversation_EmptyParticipants_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Invalid Conversation",
            ConversationType = ConversationType.Group,
            ParticipantIds = new List<string>()
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/conversations", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateConversation_DirectWithMultipleParticipants_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Invalid Direct Chat",
            ConversationType = ConversationType.Direct,
            ParticipantIds = new List<string> { "user2", "user3" } // Too many for direct chat
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/conversations", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversation_ValidId_ReturnsConversation()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        // Act
        var response = await _client.GetAsync($"/api/v1/conversations/{conversationId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ConversationDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(conversationId, result.Id);
        Assert.Equal("Test Conversation", result.Name);
    }

    [Fact]
    public async Task GetConversation_InvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/conversations/invalid-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetUserConversations_ReturnsUserConversations()
    {
        // Arrange
        await CreateTestConversation("Conversation 1", ConversationType.Direct, new[] { "user2" });
        await CreateTestConversation("Conversation 2", ConversationType.Group, new[] { "user2", "user3" });

        // Act
        var response = await _client.GetAsync("/api/v1/conversations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ConversationDto>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.All(result, c => Assert.Contains(c.Participants, p => p.UserId == "user1"));
    }

    [Fact]
    public async Task UpdateConversation_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Original Name", ConversationType.Group, new[] { "user2" });

        var request = new UpdateConversationRequest
        {
            Name = "Updated Name",
            Description = "Updated description"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/conversations/{conversationId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UpdateConversationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Updated Name", result.Conversation.Name);
        Assert.Equal("Updated description", result.Conversation.Description);
    }

    [Fact]
    public async Task UpdateConversation_NonAdmin_ReturnsForbidden()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        // Change to user2 (non-admin)
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", "user2");

        var request = new UpdateConversationRequest
        {
            Name = "Unauthorized Update"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/conversations/{conversationId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AddParticipant_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        var request = new AddParticipantRequest
        {
            UserId = "user3",
            Role = ParticipantRole.Member
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1/conversations/{conversationId}/participants", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AddParticipantResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("user3", result.Participant.UserId);
        Assert.Equal(ParticipantRole.Member, result.Participant.Role);
    }

    [Fact]
    public async Task AddParticipant_AlreadyMember_ReturnsBadRequest()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        var request = new AddParticipantRequest
        {
            UserId = "user2", // Already a participant
            Role = ParticipantRole.Member
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1/conversations/{conversationId}/participants", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveParticipant_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2", "user3" });

        // Act
        var response = await _client.DeleteAsync($"/api/v1/conversations/{conversationId}/participants/user2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify participant was removed
        var conversationResponse = await _client.GetAsync($"/api/v1/conversations/{conversationId}");
        var conversationContent = await conversationResponse.Content.ReadAsStringAsync();
        var conversation = JsonSerializer.Deserialize<ConversationDto>(conversationContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.DoesNotContain(conversation.Participants, p => p.UserId == "user2");
    }

    [Fact]
    public async Task RemoveParticipant_NonAdmin_ReturnsForbidden()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2", "user3" });

        // Change to user2 (non-admin)
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", "user2");

        // Act
        var response = await _client.DeleteAsync($"/api/v1/conversations/{conversationId}/participants/user3");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ArchiveConversation_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        // Act
        var response = await _client.PostAsync($"/api/v1/conversations/{conversationId}/archive", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ArchiveConversation_NonAdmin_ReturnsForbidden()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        // Change to user2 (non-admin)
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", "user2");

        // Act
        var response = await _client.PostAsync($"/api/v1/conversations/{conversationId}/archive", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetConversationParticipants_ValidConversation_ReturnsParticipants()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2", "user3" });

        // Act
        var response = await _client.GetAsync($"/api/v1/conversations/{conversationId}/participants");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ConversationParticipantDto>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(3, result.Count); // user1 (admin) + user2 + user3
        Assert.Contains(result, p => p.UserId == "user1" && p.Role == ParticipantRole.Admin);
        Assert.Contains(result, p => p.UserId == "user2" && p.Role == ParticipantRole.Member);
        Assert.Contains(result, p => p.UserId == "user3" && p.Role == ParticipantRole.Member);
    }

    [Fact]
    public async Task UpdateParticipantRole_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversationId = await CreateTestConversation("Test Conversation", ConversationType.Group, new[] { "user2" });

        // Act
        var response = await _client.PutAsync($"/api/v1/conversations/{conversationId}/participants/user2/role?role=Admin", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UpdateParticipantRoleResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(ParticipantRole.Admin, result.Participant.Role);
    }

    private async Task<string> CreateTestConversation(string name, ConversationType type, string[] participantIds)
    {
        var request = new CreateConversationRequest
        {
            Name = name,
            ConversationType = type,
            ParticipantIds = participantIds.ToList()
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/conversations", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateConversationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Conversation?.Id ?? throw new InvalidOperationException("Failed to create test conversation");
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}