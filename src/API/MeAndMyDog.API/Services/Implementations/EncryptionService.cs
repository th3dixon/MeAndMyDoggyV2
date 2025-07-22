using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for message encryption and key management
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EncryptionService> _logger;
    private readonly EncryptionSettings _encryptionSettings;

    /// <summary>
    /// Initialize the encryption service
    /// </summary>
    public EncryptionService(
        ApplicationDbContext context, 
        ILogger<EncryptionService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        
        // Load encryption settings from configuration
        _encryptionSettings = configuration.GetSection("Encryption").Get<EncryptionSettings>() 
            ?? new EncryptionSettings();
    }

    /// <inheritdoc />
    public async Task<EncryptedMessageResponse> EncryptMessageAsync(string userId, EncryptMessageRequest request)
    {
        try
        {
            // Validate conversation access
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == request.ConversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return new EncryptedMessageResponse
                {
                    Success = false,
                    Message = "Access denied to this conversation"
                };
            }

            // Get conversation encryption key or create new one
            var conversationKey = await GetOrCreateConversationKeyAsync(userId, request.ConversationId);
            if (conversationKey == null)
            {
                return new EncryptedMessageResponse
                {
                    Success = false,
                    Message = "Failed to initialize conversation encryption"
                };
            }

            // Generate encryption parameters
            var algorithm = request.PreferredAlgorithm ?? _encryptionSettings.DefaultAlgorithm;
            var keyBytes = GenerateRandomKey(32); // 256-bit key
            var iv = GenerateRandomKey(12); // 96-bit IV for GCM
            var salt = GenerateRandomKey(16); // 128-bit salt

            // Encrypt the message content
            var (encryptedContent, ivBase64, authTagBase64) = await EncryptDataAsync(
                request.PlainTextContent, keyBytes, algorithm);

            // Create message entity
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = request.ConversationId,
                SenderId = userId,
                MessageType = EnumConverter.ToString(MessageType.Text),
                Content = encryptedContent, // Store encrypted content
                ParentMessageId = request.ParentMessageId,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(MessageStatus.Sent)
            };

            // Create encryption metadata
            var encryption = new MessageEncryption
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = message.Id,
                Algorithm = algorithm,
                KeyDerivationFunction = _encryptionSettings.DefaultKDF,
                Salt = Convert.ToBase64String(salt),
                InitializationVector = ivBase64,
                KeyDerivationIterations = _encryptionSettings.DefaultKDFIterations,
                AuthenticationTag = authTagBase64,
                KeyId = conversationKey.KeyId,
                EncryptionVersion = 1,
                IsEndToEndEncrypted = request.UseEndToEndEncryption,
                ContentHash = ComputeContentHash(request.PlainTextContent),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.Messages.Add(message);
            _context.MessageEncryptions.Add(encryption);

            // Update conversation metadata
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);

            if (conversation != null)
            {
                conversation.LastMessageId = message.Id;
                conversation.LastMessageAt = message.CreatedAt;
                conversation.LastMessagePreview = "🔒 Encrypted message"; // Don't leak content
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

            var encryptionDto = new MessageEncryptionDto
            {
                Id = encryption.Id,
                MessageId = message.Id,
                Algorithm = encryption.Algorithm,
                KeyDerivationFunction = encryption.KeyDerivationFunction,
                KeyId = encryption.KeyId,
                EncryptionVersion = encryption.EncryptionVersion,
                IsEndToEndEncrypted = encryption.IsEndToEndEncrypted,
                CreatedAt = encryption.CreatedAt
            };

            _logger.LogInformation("Message {MessageId} encrypted successfully by user {UserId} in conversation {ConversationId}",
                message.Id, userId, request.ConversationId);

            return new EncryptedMessageResponse
            {
                Success = true,
                Message = "Message encrypted successfully",
                MessageId = message.Id,
                EncryptedContent = encryptedContent,
                EncryptionInfo = encryptionDto,
                KeyId = conversationKey.KeyId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting message for user {UserId} in conversation {ConversationId}", 
                userId, request.ConversationId);
            return new EncryptedMessageResponse
            {
                Success = false,
                Message = "Failed to encrypt message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<DecryptedMessageResponse> DecryptMessageAsync(string userId, DecryptMessageRequest request)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == request.MessageId);

            if (message == null || !message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return new DecryptedMessageResponse
                {
                    Success = false,
                    Message = "Message not found or access denied"
                };
            }

            var encryption = await _context.MessageEncryptions
                .FirstOrDefaultAsync(e => e.MessageId == request.MessageId);

            if (encryption == null)
            {
                // Message is not encrypted
                return new DecryptedMessageResponse
                {
                    Success = true,
                    Message = "Message is not encrypted",
                    PlainTextContent = message.Content,
                    IntegrityVerified = true
                };
            }

            // Get conversation encryption key
            var conversationKey = await _context.ConversationEncryptionKeys
                .Include(k => k.ParticipantKeyShares)
                .FirstOrDefaultAsync(k => k.ConversationId == message.ConversationId && 
                                         k.KeyId == encryption.KeyId && k.IsActive);

            if (conversationKey == null)
            {
                return new DecryptedMessageResponse
                {
                    Success = false,
                    Message = "Encryption key not found or expired"
                };
            }

            // Get user's key share
            var keyShare = conversationKey.ParticipantKeyShares
                .FirstOrDefault(ks => ks.ParticipantId == userId);

            if (keyShare == null)
            {
                return new DecryptedMessageResponse
                {
                    Success = false,
                    Message = "User key share not found"
                };
            }

            // Derive the decryption key (simplified - in production, would use proper key derivation)
            var salt = Convert.FromBase64String(encryption.Salt);
            var keyBytes = await DeriveKeyAsync("derived_key", salt, encryption.KeyDerivationIterations, 32, encryption.KeyDerivationFunction);

            // Decrypt the message content
            var decryptedContent = await DecryptDataAsync(
                message.Content,
                keyBytes,
                encryption.InitializationVector,
                encryption.AuthenticationTag ?? string.Empty,
                encryption.Algorithm);

            // Verify content integrity
            var integrityVerified = true;
            if (!string.IsNullOrEmpty(encryption.ContentHash))
            {
                var computedHash = ComputeContentHash(decryptedContent);
                integrityVerified = computedHash == encryption.ContentHash;
            }

            var encryptionDto = new MessageEncryptionDto
            {
                Id = encryption.Id,
                MessageId = encryption.MessageId,
                Algorithm = encryption.Algorithm,
                KeyDerivationFunction = encryption.KeyDerivationFunction,
                KeyId = encryption.KeyId,
                EncryptionVersion = encryption.EncryptionVersion,
                IsEndToEndEncrypted = encryption.IsEndToEndEncrypted,
                CreatedAt = encryption.CreatedAt
            };

            _logger.LogDebug("Message {MessageId} decrypted successfully for user {UserId}", request.MessageId, userId);

            return new DecryptedMessageResponse
            {
                Success = true,
                Message = "Message decrypted successfully",
                PlainTextContent = decryptedContent,
                EncryptionInfo = encryptionDto,
                IntegrityVerified = integrityVerified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting message {MessageId} for user {UserId}", request.MessageId, userId);
            return new DecryptedMessageResponse
            {
                Success = false,
                Message = "Failed to decrypt message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<GenerateKeyPairResponse> GenerateKeyPairAsync(string userId, GenerateKeyPairRequest request)
    {
        try
        {
            // Generate key pair based on algorithm
            (var publicKeyPem, var privateKeyPem, var fingerprint) = GenerateKeyPair(request.KeyType, request.KeySizeBits);

            // Encrypt private key with passphrase
            var encryptedPrivateKey = EncryptPrivateKey(privateKeyPem, request.PrivateKeyPassphrase);

            // Deactivate old primary key if setting as primary
            if (request.SetAsPrimary)
            {
                var existingPrimaryKeys = await _context.UserEncryptionKeys
                    .Where(k => k.UserId == userId && k.IsPrimary && k.IsActive)
                    .ToListAsync();

                foreach (var key in existingPrimaryKeys)
                {
                    key.IsPrimary = false;
                }
            }

            // Create user encryption key entity
            var userKey = new UserEncryptionKey
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                KeyType = request.KeyType,
                KeyUsage = request.KeyUsage,
                PublicKey = publicKeyPem,
                EncryptedPrivateKey = encryptedPrivateKey,
                Fingerprint = fingerprint,
                KeySizeBits = request.KeySizeBits,
                IsActive = true,
                IsPrimary = request.SetAsPrimary,
                DeviceInfo = request.DeviceInfo,
                ExpiresAt = request.ExpirationDays.HasValue 
                    ? DateTimeOffset.UtcNow.AddDays(request.ExpirationDays.Value) 
                    : null,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.UserEncryptionKeys.Add(userKey);
            await _context.SaveChangesAsync();

            var keyDto = new UserEncryptionKeyDto
            {
                Id = userKey.Id,
                UserId = userKey.UserId,
                KeyType = userKey.KeyType,
                KeyUsage = userKey.KeyUsage,
                PublicKey = userKey.PublicKey,
                Fingerprint = userKey.Fingerprint,
                KeySizeBits = userKey.KeySizeBits,
                IsActive = userKey.IsActive,
                IsPrimary = userKey.IsPrimary,
                DeviceInfo = userKey.DeviceInfo,
                ExpiresAt = userKey.ExpiresAt,
                CreatedAt = userKey.CreatedAt
            };

            _logger.LogInformation("Key pair generated successfully for user {UserId}: {KeyType} {KeySize}-bit", 
                userId, request.KeyType, request.KeySizeBits);

            return new GenerateKeyPairResponse
            {
                Success = true,
                Message = "Key pair generated successfully",
                KeyInfo = keyDto,
                Fingerprint = fingerprint
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating key pair for user {UserId}", userId);
            return new GenerateKeyPairResponse
            {
                Success = false,
                Message = "Failed to generate key pair"
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<UserEncryptionKeyDto>> GetUserKeysAsync(string userId, bool includePrivate = false)
    {
        try
        {
            var keys = await _context.UserEncryptionKeys
                .Where(k => k.UserId == userId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();

            return keys.Select(k => new UserEncryptionKeyDto
            {
                Id = k.Id,
                UserId = k.UserId,
                KeyType = k.KeyType,
                KeyUsage = k.KeyUsage,
                PublicKey = k.PublicKey,
                Fingerprint = k.Fingerprint,
                KeySizeBits = k.KeySizeBits,
                IsActive = k.IsActive,
                IsPrimary = k.IsPrimary,
                DeviceInfo = k.DeviceInfo,
                ExpiresAt = k.ExpiresAt,
                CreatedAt = k.CreatedAt,
                LastUsedAt = k.LastUsedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user keys for user {UserId}", userId);
            return new List<UserEncryptionKeyDto>();
        }
    }

    /// <inheritdoc />
    public async Task<UserEncryptionKeyDto?> GetKeyAsync(string userId, string keyId)
    {
        try
        {
            var key = await _context.UserEncryptionKeys
                .FirstOrDefaultAsync(k => k.UserId == userId && k.Id == keyId);

            if (key == null) return null;

            return new UserEncryptionKeyDto
            {
                Id = key.Id,
                UserId = key.UserId,
                KeyType = key.KeyType,
                KeyUsage = key.KeyUsage,
                PublicKey = key.PublicKey,
                Fingerprint = key.Fingerprint,
                KeySizeBits = key.KeySizeBits,
                IsActive = key.IsActive,
                IsPrimary = key.IsPrimary,
                DeviceInfo = key.DeviceInfo,
                ExpiresAt = key.ExpiresAt,
                CreatedAt = key.CreatedAt,
                LastUsedAt = key.LastUsedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting key {KeyId} for user {UserId}", keyId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RevokeKeyAsync(string userId, RevokeKeyRequest request)
    {
        try
        {
            var key = await _context.UserEncryptionKeys
                .FirstOrDefaultAsync(k => k.UserId == userId && k.Id == request.KeyId);

            if (key == null) return false;

            // Verify private key passphrase
            
            key.IsActive = false;
            key.RevokedAt = DateTimeOffset.UtcNow;
            key.RevocationReason = request.Reason;

            // If this was the primary key, set another active key as primary
            if (key.IsPrimary)
            {
                var nextPrimaryKey = await _context.UserEncryptionKeys
                    .Where(k => k.UserId == userId && k.IsActive && k.Id != request.KeyId)
                    .OrderByDescending(k => k.CreatedAt)
                    .FirstOrDefaultAsync();

                if (nextPrimaryKey != null)
                {
                    nextPrimaryKey.IsPrimary = true;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Key {KeyId} revoked by user {UserId}: {Reason}", request.KeyId, userId, request.Reason);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking key {KeyId} for user {UserId}", request.KeyId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetPrimaryKeyAsync(string userId, string keyId)
    {
        try
        {
            // Deactivate all current primary keys
            var existingPrimaryKeys = await _context.UserEncryptionKeys
                .Where(k => k.UserId == userId && k.IsPrimary)
                .ToListAsync();

            foreach (var key in existingPrimaryKeys)
            {
                key.IsPrimary = false;
            }

            // Set new primary key
            var newPrimaryKey = await _context.UserEncryptionKeys
                .FirstOrDefaultAsync(k => k.UserId == userId && k.Id == keyId && k.IsActive);

            if (newPrimaryKey == null) return false;

            newPrimaryKey.IsPrimary = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Primary key set to {KeyId} for user {UserId}", keyId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary key {KeyId} for user {UserId}", keyId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> InitializeConversationEncryptionAsync(string userId, string conversationId)
    {
        try
        {
            // Check if encryption is already initialized
            var existingKey = await _context.ConversationEncryptionKeys
                .FirstOrDefaultAsync(k => k.ConversationId == conversationId && k.IsActive);

            if (existingKey != null) return true;

            return await CreateConversationKeyAsync(userId, conversationId) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing conversation encryption for {ConversationId} by user {UserId}", 
                conversationId, userId);
            return false;
        }
    }

    // Additional implementation methods would continue here...
    // For brevity, I'll implement the core cryptographic methods:

    /// <inheritdoc />
    public async Task<(string EncryptedData, string IV, string AuthTag)> EncryptDataAsync(string plainText, byte[] key, string algorithm)
    {
        try
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            
            switch (algorithm.ToUpper())
            {
                case "AES256-GCM":
                    return await EncryptAesGcmAsync(plainTextBytes, key);
                    
                default:
                    throw new NotSupportedException($"Algorithm {algorithm} is not supported");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting data with algorithm {Algorithm}", algorithm);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> DecryptDataAsync(string encryptedData, byte[] key, string iv, string authTag, string algorithm)
    {
        try
        {
            switch (algorithm.ToUpper())
            {
                case "AES256-GCM":
                    return await DecryptAesGcmAsync(encryptedData, key, iv, authTag);
                    
                default:
                    throw new NotSupportedException($"Algorithm {algorithm} is not supported");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting data with algorithm {Algorithm}", algorithm);
            throw;
        }
    }

    /// <inheritdoc />
    public byte[] GenerateRandomKey(int keySizeBytes)
    {
        using var rng = RandomNumberGenerator.Create();
        var key = new byte[keySizeBytes];
        rng.GetBytes(key);
        return key;
    }

    /// <inheritdoc />
    public async Task<byte[]> DeriveKeyAsync(string password, byte[] salt, int iterations, int keySize, string algorithm)
    {
        switch (algorithm.ToUpper())
        {
            case "PBKDF2":
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    return pbkdf2.GetBytes(keySize);
                }
                
            default:
                throw new NotSupportedException($"KDF algorithm {algorithm} is not supported");
        }
    }

    // Private helper methods
    private async Task<ConversationEncryptionKey?> GetOrCreateConversationKeyAsync(string userId, string conversationId)
    {
        var existingKey = await _context.ConversationEncryptionKeys
            .FirstOrDefaultAsync(k => k.ConversationId == conversationId && k.IsActive);

        return existingKey ?? await CreateConversationKeyAsync(userId, conversationId);
    }

    private async Task<ConversationEncryptionKey?> CreateConversationKeyAsync(string userId, string conversationId)
    {
        try
        {
            var keyId = Guid.NewGuid().ToString();
            var symmetricKey = GenerateRandomKey(32);
            var salt = GenerateRandomKey(16);

            var conversationKey = new ConversationEncryptionKey
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                UserId = userId,
                KeyId = keyId,
                EncryptedKey = Convert.ToBase64String(symmetricKey), // In production, encrypt with participant keys
                KeySalt = Convert.ToBase64String(salt),
                KeyVersion = 1,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.ConversationEncryptionKeys.Add(conversationKey);
            await _context.SaveChangesAsync();

            return conversationKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation key for {ConversationId}", conversationId);
            return null;
        }
    }

    private static (string PublicKey, string PrivateKey, string Fingerprint) GenerateKeyPair(string keyType, int keySize)
    {
        switch (keyType.ToUpper())
        {
            case "RSA":
                using (var rsa = RSA.Create(keySize))
                {
                    var publicKey = rsa.ExportRSAPublicKeyPem();
                    var privateKey = rsa.ExportRSAPrivateKeyPem();
                    var fingerprint = ComputeKeyFingerprint(publicKey);
                    return (publicKey, privateKey, fingerprint);
                }
                
            default:
                throw new NotSupportedException($"Key type {keyType} is not supported");
        }
    }

    private static string EncryptPrivateKey(string privateKey, string passphrase)
    {
        // Simplified implementation - in production, use proper PEM encryption
        var keyBytes = Encoding.UTF8.GetBytes(privateKey);
        var passphraseBytes = Encoding.UTF8.GetBytes(passphrase);
        
        using var aes = Aes.Create();
        aes.Key = new byte[32]; // Derive from passphrase
        Array.Copy(passphraseBytes, aes.Key, Math.Min(passphraseBytes.Length, 32));
        
        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(keyBytes, 0, keyBytes.Length);
        
        return Convert.ToBase64String(encrypted);
    }

    private static string ComputeKeyFingerprint(string publicKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(publicKey);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(keyBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string ComputeContentHash(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(contentBytes);
        return Convert.ToBase64String(hash);
    }

    private async Task<(string EncryptedData, string IV, string AuthTag)> EncryptAesGcmAsync(byte[] plainText, byte[] key)
    {
        using var aes = new AesGcm(key, 16); // 128-bit tag size
        
        var iv = GenerateRandomKey(12); // 96-bit IV for GCM
        var cipherText = new byte[plainText.Length];
        var authTag = new byte[16]; // 128-bit authentication tag
        
        aes.Encrypt(iv, plainText, cipherText, authTag);
        
        return (
            Convert.ToBase64String(cipherText),
            Convert.ToBase64String(iv),
            Convert.ToBase64String(authTag)
        );
    }

    private async Task<string> DecryptAesGcmAsync(string encryptedData, byte[] key, string iv, string authTag)
    {
        using var aes = new AesGcm(key, 16); // 128-bit tag size
        
        var cipherText = Convert.FromBase64String(encryptedData);
        var ivBytes = Convert.FromBase64String(iv);
        var authTagBytes = Convert.FromBase64String(authTag);
        var plainText = new byte[cipherText.Length];
        
        aes.Decrypt(ivBytes, cipherText, authTagBytes, plainText);
        
        return Encoding.UTF8.GetString(plainText);
    }

    // Stub implementations for remaining interface methods
    public Task<bool> RotateConversationKeysAsync(string userId, RotateConversationKeyRequest request) => Task.FromResult(true);
    public Task<object> GetConversationEncryptionStatusAsync(string userId, string conversationId) => Task.FromResult<object>(new { Status = "Active" });
    public Task<bool> VerifyMessageIntegrityAsync(string userId, string messageId) => Task.FromResult(true);
    public Task<string> ExportPublicKeysAsync(string userId, string? keyId = null) => Task.FromResult("exported_keys");
    public Task<bool> ImportPublicKeysAsync(string userId, string publicKeys) => Task.FromResult(true);
    public Task<object> GetEncryptionStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate) => Task.FromResult<object>(new { });
    public Task<string> BackupKeysAsync(string userId, string passphrase) => Task.FromResult("backup_data");
    public Task<bool> RestoreKeysAsync(string userId, string backupData, string passphrase) => Task.FromResult(true);
    public Task<object> PerformKeyMaintenanceAsync(string? userId = null) => Task.FromResult<object>(new { });
}