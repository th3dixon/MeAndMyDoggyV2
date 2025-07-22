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
/// Unit tests for ConversationService
/// </summary>
public class ConversationServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ConversationService _conversationService;
    private readonly Mock<ILogger<ConversationService>> _mockLogger;

    public ConversationServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<ConversationService>>();
        _conversationService = new ConversationService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test users would normally come from Identity system
        // For testing, we'll assume users exist
    }

    [Fact]
    public async Task CreateConversationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Test Conversation",
            ConversationType = ConversationType.Group,
            ParticipantIds = new List<string> { "user1", "user2", "user3" }
        };

        // Act
        var result = await _conversationService.CreateConversationAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Conversation);
        Assert.Equal("Test Conversation", result.Conversation.Name);
        Assert.Equal(ConversationType.Group, result.Conversation.ConversationType);
        Assert.Equal("user1", result.Conversation.CreatedBy);
        Assert.Equal(3, result.Conversation.Participants.Count);

        // Verify conversation was saved to database
        var savedConversation = await _context.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == result.Conversation.Id);
        Assert.NotNull(savedConversation);
        Assert.Equal(3, savedConversation.Participants.Count);
    }

    [Fact]
    public async Task CreateConversationAsync_DirectConversation_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Direct Chat",
            ConversationType = ConversationType.Direct,
            ParticipantIds = new List<string> { "user1", "user2" }
        };

        // Act
        var result = await _conversationService.CreateConversationAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ConversationType.Direct, result.Conversation.ConversationType);
        Assert.Equal(2, result.Conversation.Participants.Count);
    }

    [Fact]
    public async Task CreateConversationAsync_EmptyParticipants_ReturnsFailed()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Test Conversation",
            ConversationType = ConversationType.Group,
            ParticipantIds = new List<string>()
        };

        // Act
        var result = await _conversationService.CreateConversationAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("at least one participant", result.Message);
    }

    [Fact]
    public async Task CreateConversationAsync_DirectConversationTooManyParticipants_ReturnsFailed()
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = "Direct Chat",
            ConversationType = ConversationType.Direct,
            ParticipantIds = new List<string> { "user1", "user2", "user3" }
        };

        // Act
        var result = await _conversationService.CreateConversationAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("exactly 2 participants", result.Message);
    }

    [Fact]
    public async Task GetConversationAsync_ValidId_ReturnsConversation()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.GetConversationAsync("user1", conversation.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(conversation.Id, result.Id);
        Assert.Equal(conversation.Name, result.Name);
        Assert.NotEmpty(result.Participants);
    }

    [Fact]
    public async Task GetConversationAsync_UserNotParticipant_ReturnsNull()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.GetConversationAsync("user3", conversation.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserConversationsAsync_ValidUser_ReturnsConversations()
    {
        // Arrange
        await CreateTestConversation();
        await CreateTestConversation("user1", "user3");

        // Act
        var result = await _conversationService.GetUserConversationsAsync("user1");

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count >= 2);
        Assert.All(result, c => Assert.Contains(c.Participants, p => p.UserId == "user1"));
    }

    [Fact]
    public async Task AddParticipantAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();
        var request = new AddParticipantRequest
        {
            UserId = "user3",
            Role = ParticipantRole.Member
        };

        // Act
        var result = await _conversationService.AddParticipantAsync("user1", conversation.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Participant);
        Assert.Equal("user3", result.Participant.UserId);
        Assert.Equal(ParticipantRole.Member, result.Participant.Role);

        // Verify participant was added to database
        var participant = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversation.Id && p.UserId == "user3");
        Assert.NotNull(participant);
    }

    [Fact]
    public async Task AddParticipantAsync_AlreadyParticipant_ReturnsFailed()
    {
        // Arrange
        var conversation = await CreateTestConversation();
        var request = new AddParticipantRequest
        {
            UserId = "user2", // Already a participant
            Role = ParticipantRole.Member
        };

        // Act
        var result = await _conversationService.AddParticipantAsync("user1", conversation.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already a participant", result.Message);
    }

    [Fact]
    public async Task AddParticipantAsync_UserNotAdmin_ReturnsFailed()
    {
        // Arrange
        var conversation = await CreateTestConversation();
        var request = new AddParticipantRequest
        {
            UserId = "user3",
            Role = ParticipantRole.Member
        };

        // Act - user2 is not admin
        var result = await _conversationService.AddParticipantAsync("user2", conversation.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("permission", result.Message);
    }

    [Fact]
    public async Task RemoveParticipantAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.RemoveParticipantAsync("user1", conversation.Id, "user2");

        // Assert
        Assert.True(result);

        // Verify participant was removed from database
        var participant = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversation.Id && p.UserId == "user2");
        Assert.Null(participant);
    }

    [Fact]
    public async Task RemoveParticipantAsync_RemoveSelf_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act - user removes themselves
        var result = await _conversationService.RemoveParticipantAsync("user2", conversation.Id, "user2");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveParticipantAsync_UserNotAdmin_ReturnsFalse()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act - user2 (not admin) tries to remove user1
        var result = await _conversationService.RemoveParticipantAsync("user2", conversation.Id, "user1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateConversationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();
        var request = new UpdateConversationRequest
        {
            Name = "Updated Conversation Name",
            Description = "Updated description"
        };

        // Act
        var result = await _conversationService.UpdateConversationAsync("user1", conversation.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Updated Conversation Name", result.Conversation.Name);
        Assert.Equal("Updated description", result.Conversation.Description);

        // Verify changes were saved to database
        var updatedConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversation.Id);
        Assert.NotNull(updatedConversation);
        Assert.Equal("Updated Conversation Name", updatedConversation.Name);
        Assert.Equal("Updated description", updatedConversation.Description);
    }

    [Fact]
    public async Task UpdateConversationAsync_UserNotAdmin_ReturnsFailed()
    {
        // Arrange
        var conversation = await CreateTestConversation();
        var request = new UpdateConversationRequest
        {
            Name = "Updated Name"
        };

        // Act
        var result = await _conversationService.UpdateConversationAsync("user2", conversation.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("permission", result.Message);
    }

    [Fact]
    public async Task ArchiveConversationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.ArchiveConversationAsync("user1", conversation.Id);

        // Assert
        Assert.True(result);

        // Verify conversation was archived in database
        var archivedConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversation.Id);
        Assert.NotNull(archivedConversation);
        Assert.True(archivedConversation.IsArchived);
        Assert.NotNull(archivedConversation.ArchivedAt);
    }

    [Fact]
    public async Task ArchiveConversationAsync_UserNotAdmin_ReturnsFalse()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.ArchiveConversationAsync("user2", conversation.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateParticipantRoleAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.UpdateParticipantRoleAsync("user1", conversation.Id, "user2", ParticipantRole.Admin);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ParticipantRole.Admin, result.Participant.Role);

        // Verify role was updated in database
        var participant = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversation.Id && p.UserId == "user2");
        Assert.NotNull(participant);
        Assert.Equal("Admin", participant.Role);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateConversationAsync_EmptyName_ReturnsFailed(string name)
    {
        // Arrange
        var request = new CreateConversationRequest
        {
            Name = name,
            ConversationType = ConversationType.Group,
            ParticipantIds = new List<string> { "user1", "user2" }
        };

        // Act
        var result = await _conversationService.CreateConversationAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("name is required", result.Message);
    }

    [Fact]
    public async Task GetConversationParticipantsAsync_ValidConversation_ReturnsParticipants()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.GetConversationParticipantsAsync("user1", conversation.Id);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.UserId == "user1");
        Assert.Contains(result, p => p.UserId == "user2");
    }

    [Fact]
    public async Task GetConversationParticipantsAsync_UserNotParticipant_ReturnsEmpty()
    {
        // Arrange
        var conversation = await CreateTestConversation();

        // Act
        var result = await _conversationService.GetConversationParticipantsAsync("user3", conversation.Id);

        // Assert
        Assert.Empty(result);
    }

    private async Task<Conversation> CreateTestConversation(string user1 = "user1", string user2 = "user2")
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Conversation",
            ConversationType = "Group",
            CreatedBy = user1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var participant1 = new ConversationParticipant
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversation.Id,
            UserId = user1,
            Role = "Admin",
            JoinedAt = DateTimeOffset.UtcNow
        };

        var participant2 = new ConversationParticipant
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversation.Id,
            UserId = user2,
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        _context.Conversations.Add(conversation);
        _context.ConversationParticipants.AddRange(participant1, participant2);
        await _context.SaveChangesAsync();

        return conversation;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}