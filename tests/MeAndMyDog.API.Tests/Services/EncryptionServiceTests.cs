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
/// Unit tests for EncryptionService
/// </summary>
public class EncryptionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EncryptionService _encryptionService;
    private readonly Mock<ILogger<EncryptionService>> _mockLogger;

    public EncryptionServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<EncryptionService>>();
        _encryptionService = new EncryptionService(_context, _mockLogger.Object);

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

        _context.Conversations.Add(conversation);
        _context.SaveChanges();
    }

    [Fact]
    public async Task EncryptMessageAsync_ValidContent_ReturnsSuccess()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var content = "Hello, this is a secret message!";

        // Act
        var result = await _encryptionService.EncryptMessageAsync(conversationId, content);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(content, result.EncryptedContent);
        Assert.NotNull(result.KeyId);
        Assert.NotEmpty(result.EncryptedContent);

        // Verify encryption key was saved
        var key = await _context.EncryptionKeys.FirstOrDefaultAsync(k => k.Id == result.KeyId);
        Assert.NotNull(key);
        Assert.Equal(conversationId, key.ConversationId);
    }

    [Fact]
    public async Task EncryptMessageAsync_EmptyContent_ReturnsFailed()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var content = "";

        // Act
        var result = await _encryptionService.EncryptMessageAsync(conversationId, content);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Content cannot be empty", result.ErrorMessage);
    }

    [Fact]
    public async Task EncryptMessageAsync_InvalidConversation_ReturnsFailed()
    {
        // Arrange
        var conversationId = "non-existent-conversation";
        var content = "Test message";

        // Act
        var result = await _encryptionService.EncryptMessageAsync(conversationId, content);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Conversation not found", result.ErrorMessage);
    }

    [Fact]
    public async Task DecryptMessageAsync_ValidEncryption_ReturnsOriginalContent()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var originalContent = "Hello, this is a secret message!";

        // First encrypt the message
        var encryptResult = await _encryptionService.EncryptMessageAsync(conversationId, originalContent);
        Assert.True(encryptResult.Success);

        // Act
        var decryptResult = await _encryptionService.DecryptMessageAsync(encryptResult.KeyId, encryptResult.EncryptedContent);

        // Assert
        Assert.True(decryptResult.Success);
        Assert.Equal(originalContent, decryptResult.DecryptedContent);
    }

    [Fact]
    public async Task DecryptMessageAsync_InvalidKeyId_ReturnsFailed()
    {
        // Arrange
        var keyId = "non-existent-key";
        var encryptedContent = "some-encrypted-content";

        // Act
        var result = await _encryptionService.DecryptMessageAsync(keyId, encryptedContent);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Encryption key not found", result.ErrorMessage);
    }

    [Fact]
    public async Task GenerateKeyPairAsync_ValidConversation_ReturnsSuccess()
    {
        // Arrange
        var conversationId = "test-conversation-1";

        // Act
        var result = await _encryptionService.GenerateKeyPairAsync(conversationId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.KeyPair);
        Assert.NotEmpty(result.KeyPair.PublicKey);
        Assert.NotEmpty(result.KeyPair.PrivateKey);

        // Verify key pair was saved
        var keyPair = await _context.EncryptionKeyPairs.FirstOrDefaultAsync(k => k.Id == result.KeyPair.Id);
        Assert.NotNull(keyPair);
        Assert.Equal(conversationId, keyPair.ConversationId);
    }

    [Fact]
    public async Task GetConversationKeysAsync_ValidConversation_ReturnsKeys()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        
        // Generate some keys first
        await _encryptionService.GenerateKeyPairAsync(conversationId);
        await _encryptionService.EncryptMessageAsync(conversationId, "Test message 1");
        await _encryptionService.EncryptMessageAsync(conversationId, "Test message 2");

        // Act
        var result = await _encryptionService.GetConversationKeysAsync(conversationId);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, key => Assert.Equal(conversationId, key.ConversationId));
    }

    [Fact]
    public async Task RevokeKeyAsync_ValidKey_ReturnsSuccess()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var encryptResult = await _encryptionService.EncryptMessageAsync(conversationId, "Test message");
        Assert.True(encryptResult.Success);

        // Act
        var result = await _encryptionService.RevokeKeyAsync(encryptResult.KeyId, "user1", "Key compromised");

        // Assert
        Assert.True(result);

        // Verify key was revoked
        var key = await _context.EncryptionKeys.FirstOrDefaultAsync(k => k.Id == encryptResult.KeyId);
        Assert.NotNull(key);
        Assert.True(key.IsRevoked);
        Assert.NotNull(key.RevokedAt);
        Assert.Equal("user1", key.RevokedBy);
    }

    [Fact]
    public async Task ValidateKeyAsync_ValidActiveKey_ReturnsValid()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var encryptResult = await _encryptionService.EncryptMessageAsync(conversationId, "Test message");
        Assert.True(encryptResult.Success);

        // Act
        var result = await _encryptionService.ValidateKeyAsync(encryptResult.KeyId);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("Active", result.Status);
        Assert.Null(result.ValidationError);
    }

    [Fact]
    public async Task ValidateKeyAsync_RevokedKey_ReturnsInvalid()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var encryptResult = await _encryptionService.EncryptMessageAsync(conversationId, "Test message");
        Assert.True(encryptResult.Success);

        // Revoke the key
        await _encryptionService.RevokeKeyAsync(encryptResult.KeyId, "user1", "Test revocation");

        // Act
        var result = await _encryptionService.ValidateKeyAsync(encryptResult.KeyId);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("Revoked", result.Status);
        Assert.NotNull(result.ValidationError);
    }

    [Fact]
    public async Task ValidateKeyAsync_ExpiredKey_ReturnsInvalid()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        var encryptResult = await _encryptionService.EncryptMessageAsync(conversationId, "Test message");
        Assert.True(encryptResult.Success);

        // Manually expire the key by updating the database
        var key = await _context.EncryptionKeys.FirstOrDefaultAsync(k => k.Id == encryptResult.KeyId);
        Assert.NotNull(key);
        key.ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _encryptionService.ValidateKeyAsync(encryptResult.KeyId);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("Expired", result.Status);
        Assert.Contains("expired", result.ValidationError);
    }

    [Fact]
    public async Task RotateConversationKeysAsync_ValidConversation_ReturnsSuccess()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        
        // Create some existing keys
        await _encryptionService.EncryptMessageAsync(conversationId, "Message 1");
        await _encryptionService.EncryptMessageAsync(conversationId, "Message 2");

        // Act
        var result = await _encryptionService.RotateConversationKeysAsync(conversationId, "user1");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.NewKeyPair);

        // Verify old keys were revoked
        var oldKeys = await _context.EncryptionKeys
            .Where(k => k.ConversationId == conversationId && k.Id != result.NewKeyPair.Id)
            .ToListAsync();
        Assert.All(oldKeys, key => Assert.True(key.IsRevoked));

        // Verify new key pair was created
        var newKeyPair = await _context.EncryptionKeyPairs
            .FirstOrDefaultAsync(k => k.Id == result.NewKeyPair.Id);
        Assert.NotNull(newKeyPair);
        Assert.Equal(conversationId, newKeyPair.ConversationId);
    }

    [Fact]
    public async Task GetEncryptionStatisticsAsync_WithData_ReturnsStats()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        
        // Create some encrypted messages
        await _encryptionService.EncryptMessageAsync(conversationId, "Message 1");
        await _encryptionService.EncryptMessageAsync(conversationId, "Message 2");
        await _encryptionService.EncryptMessageAsync(conversationId, "Message 3");

        // Act
        var result = await _encryptionService.GetEncryptionStatisticsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalEncryptedMessages >= 3);
        Assert.True(result.ActiveKeys >= 3);
        Assert.NotEmpty(result.EncryptionAlgorithms);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task EncryptMessageAsync_InvalidContent_ReturnsFailed(string content)
    {
        // Arrange
        var conversationId = "test-conversation-1";

        // Act
        var result = await _encryptionService.EncryptMessageAsync(conversationId, content);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("cannot be empty", result.ErrorMessage);
    }

    [Fact]
    public async Task ExportConversationKeysAsync_ValidConversation_ReturnsKeys()
    {
        // Arrange
        var conversationId = "test-conversation-1";
        
        // Create some keys
        await _encryptionService.GenerateKeyPairAsync(conversationId);
        await _encryptionService.EncryptMessageAsync(conversationId, "Test message");

        // Act
        var result = await _encryptionService.ExportConversationKeysAsync(conversationId, "user1");

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.ExportedKeys);
        Assert.NotNull(result.ExportTimestamp);
        Assert.Equal("user1", result.ExportedBy);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}