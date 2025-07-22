using MeAndMyDog.API.Models.DTOs;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for message encryption and key management
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypt a message with end-to-end encryption
    /// </summary>
    /// <param name="userId">ID of the user sending the message</param>
    /// <param name="request">Encryption request details</param>
    /// <returns>Encrypted message response</returns>
    Task<EncryptedMessageResponse> EncryptMessageAsync(string userId, EncryptMessageRequest request);

    /// <summary>
    /// Decrypt a message for the requesting user
    /// </summary>
    /// <param name="userId">ID of the user requesting decryption</param>
    /// <param name="request">Decryption request details</param>
    /// <returns>Decrypted message response</returns>
    Task<DecryptedMessageResponse> DecryptMessageAsync(string userId, DecryptMessageRequest request);

    /// <summary>
    /// Generate a new key pair for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="request">Key generation request</param>
    /// <returns>Key generation response</returns>
    Task<GenerateKeyPairResponse> GenerateKeyPairAsync(string userId, GenerateKeyPairRequest request);

    /// <summary>
    /// Get user's encryption keys
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="includePrivate">Whether to include private key information</param>
    /// <returns>List of user's encryption keys</returns>
    Task<List<UserEncryptionKeyDto>> GetUserKeysAsync(string userId, bool includePrivate = false);

    /// <summary>
    /// Get a specific encryption key by ID
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="keyId">ID of the key</param>
    /// <returns>Encryption key or null if not found</returns>
    Task<UserEncryptionKeyDto?> GetKeyAsync(string userId, string keyId);

    /// <summary>
    /// Revoke an encryption key
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="request">Key revocation request</param>
    /// <returns>True if key was successfully revoked</returns>
    Task<bool> RevokeKeyAsync(string userId, RevokeKeyRequest request);

    /// <summary>
    /// Set a key as the primary key for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="keyId">ID of the key to set as primary</param>
    /// <returns>True if key was successfully set as primary</returns>
    Task<bool> SetPrimaryKeyAsync(string userId, string keyId);

    /// <summary>
    /// Initialize encryption for a conversation
    /// </summary>
    /// <param name="userId">ID of the user initializing encryption</param>
    /// <param name="conversationId">ID of the conversation</param>
    /// <returns>True if encryption was successfully initialized</returns>
    Task<bool> InitializeConversationEncryptionAsync(string userId, string conversationId);

    /// <summary>
    /// Rotate encryption keys for a conversation
    /// </summary>
    /// <param name="userId">ID of the user requesting rotation</param>
    /// <param name="request">Key rotation request</param>
    /// <returns>True if keys were successfully rotated</returns>
    Task<bool> RotateConversationKeysAsync(string userId, RotateConversationKeyRequest request);

    /// <summary>
    /// Get conversation encryption status
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="conversationId">ID of the conversation</param>
    /// <returns>Encryption status information</returns>
    Task<object> GetConversationEncryptionStatusAsync(string userId, string conversationId);

    /// <summary>
    /// Verify message integrity and authenticity
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="messageId">ID of the message to verify</param>
    /// <returns>Verification result</returns>
    Task<bool> VerifyMessageIntegrityAsync(string userId, string messageId);

    /// <summary>
    /// Export public keys for sharing
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="keyId">ID of the key to export (null for all active keys)</param>
    /// <returns>Exported public keys in standard format</returns>
    Task<string> ExportPublicKeysAsync(string userId, string? keyId = null);

    /// <summary>
    /// Import public keys from other users
    /// </summary>
    /// <param name="userId">ID of the user importing keys</param>
    /// <param name="publicKeys">Public keys in standard format</param>
    /// <returns>True if keys were successfully imported</returns>
    Task<bool> ImportPublicKeysAsync(string userId, string publicKeys);

    /// <summary>
    /// Get encryption statistics for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Encryption usage statistics</returns>
    Task<object> GetEncryptionStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate);

    /// <summary>
    /// Backup user's encryption keys
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="passphrase">Backup encryption passphrase</param>
    /// <returns>Encrypted backup data</returns>
    Task<string> BackupKeysAsync(string userId, string passphrase);

    /// <summary>
    /// Restore user's encryption keys from backup
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="backupData">Encrypted backup data</param>
    /// <param name="passphrase">Backup decryption passphrase</param>
    /// <returns>True if keys were successfully restored</returns>
    Task<bool> RestoreKeysAsync(string userId, string backupData, string passphrase);

    /// <summary>
    /// Perform key maintenance (cleanup expired keys, rotate old keys, etc.)
    /// </summary>
    /// <param name="userId">ID of the user (null for system-wide maintenance)</param>
    /// <returns>Maintenance results</returns>
    Task<object> PerformKeyMaintenanceAsync(string? userId = null);

    /// <summary>
    /// Encrypt data with a specific algorithm
    /// </summary>
    /// <param name="plainText">Plain text to encrypt</param>
    /// <param name="key">Encryption key</param>
    /// <param name="algorithm">Encryption algorithm</param>
    /// <returns>Encrypted data with metadata</returns>
    Task<(string EncryptedData, string IV, string AuthTag)> EncryptDataAsync(string plainText, byte[] key, string algorithm);

    /// <summary>
    /// Decrypt data with a specific algorithm
    /// </summary>
    /// <param name="encryptedData">Encrypted data</param>
    /// <param name="key">Decryption key</param>
    /// <param name="iv">Initialization vector</param>
    /// <param name="authTag">Authentication tag</param>
    /// <param name="algorithm">Encryption algorithm</param>
    /// <returns>Decrypted plain text</returns>
    Task<string> DecryptDataAsync(string encryptedData, byte[] key, string iv, string authTag, string algorithm);

    /// <summary>
    /// Generate a cryptographically secure random key
    /// </summary>
    /// <param name="keySizeBytes">Key size in bytes</param>
    /// <returns>Random key bytes</returns>
    byte[] GenerateRandomKey(int keySizeBytes);

    /// <summary>
    /// Derive a key from a password using PBKDF2 or Argon2
    /// </summary>
    /// <param name="password">Password to derive from</param>
    /// <param name="salt">Salt for derivation</param>
    /// <param name="iterations">Number of iterations</param>
    /// <param name="keySize">Desired key size in bytes</param>
    /// <param name="algorithm">KDF algorithm (PBKDF2, Argon2)</param>
    /// <returns>Derived key</returns>
    Task<byte[]> DeriveKeyAsync(string password, byte[] salt, int iterations, int keySize, string algorithm);
}