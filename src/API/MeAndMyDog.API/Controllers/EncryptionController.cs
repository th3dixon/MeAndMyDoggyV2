using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for message encryption and key management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EncryptionController : ControllerBase
{
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<EncryptionController> _logger;

    /// <summary>
    /// Initialize the encryption controller
    /// </summary>
    public EncryptionController(IEncryptionService encryptionService, ILogger<EncryptionController> logger)
    {
        _encryptionService = encryptionService;
        _logger = logger;
    }

    /// <summary>
    /// Encrypt and send a message
    /// </summary>
    /// <param name="request">Message encryption request</param>
    /// <returns>Encrypted message response</returns>
    [HttpPost("encrypt-message")]
    public async Task<ActionResult<EncryptedMessageResponse>> EncryptMessage([FromBody] EncryptMessageRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new EncryptedMessageResponse { Success = false, Message = "User not authenticated" });
            }

            var response = await _encryptionService.EncryptMessageAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Message encrypted successfully by user {UserId} in conversation {ConversationId}", 
                    userId, request.ConversationId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Message encryption failed for user {UserId}: {Message}", userId, response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting message for user {UserId}", GetUserId());
            return StatusCode(500, new EncryptedMessageResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Decrypt a message
    /// </summary>
    /// <param name="request">Message decryption request</param>
    /// <returns>Decrypted message response</returns>
    [HttpPost("decrypt-message")]
    public async Task<ActionResult<DecryptedMessageResponse>> DecryptMessage([FromBody] DecryptMessageRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new DecryptedMessageResponse { Success = false, Message = "User not authenticated" });
            }

            var response = await _encryptionService.DecryptMessageAsync(userId, request);

            if (response.Success)
            {
                _logger.LogDebug("Message decrypted successfully for user {UserId}", userId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Message decryption failed for user {UserId}: {Message}", userId, response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting message for user {UserId}", GetUserId());
            return StatusCode(500, new DecryptedMessageResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Generate a new encryption key pair
    /// </summary>
    /// <param name="request">Key generation request</param>
    /// <returns>Generated key pair response</returns>
    [HttpPost("generate-keypair")]
    public async Task<ActionResult<GenerateKeyPairResponse>> GenerateKeyPair([FromBody] GenerateKeyPairRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GenerateKeyPairResponse { Success = false, Message = "User not authenticated" });
            }

            var response = await _encryptionService.GenerateKeyPairAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Key pair generated successfully for user {UserId}: {KeyType} {KeySize}-bit", 
                    userId, request.KeyType, request.KeySizeBits);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Key pair generation failed for user {UserId}: {Message}", userId, response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating key pair for user {UserId}", GetUserId());
            return StatusCode(500, new GenerateKeyPairResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get user's encryption keys
    /// </summary>
    /// <param name="includePrivate">Whether to include private key information</param>
    /// <returns>List of user's encryption keys</returns>
    [HttpGet("keys")]
    public async Task<ActionResult<List<UserEncryptionKeyDto>>> GetKeys([FromQuery] bool includePrivate = false)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var keys = await _encryptionService.GetUserKeysAsync(userId, includePrivate);

            return Ok(keys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encryption keys for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get a specific encryption key
    /// </summary>
    /// <param name="keyId">Encryption key ID</param>
    /// <returns>Encryption key details</returns>
    [HttpGet("keys/{keyId}")]
    public async Task<ActionResult<UserEncryptionKeyDto>> GetKey(string keyId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var key = await _encryptionService.GetKeyAsync(userId, keyId);

            if (key == null)
            {
                return NotFound("Encryption key not found");
            }

            return Ok(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encryption key {KeyId} for user {UserId}", keyId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Revoke an encryption key
    /// </summary>
    /// <param name="request">Key revocation request</param>
    /// <returns>Success status</returns>
    [HttpPost("revoke-key")]
    public async Task<ActionResult> RevokeKey([FromBody] RevokeKeyRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _encryptionService.RevokeKeyAsync(userId, request);

            if (success)
            {
                _logger.LogInformation("Key {KeyId} revoked by user {UserId}: {Reason}", request.KeyId, userId, request.Reason);
                return Ok(new { Success = true, Message = "Key revoked successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to revoke key or key not found" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking key {KeyId} for user {UserId}", request.KeyId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Set a key as the primary key
    /// </summary>
    /// <param name="keyId">Key ID to set as primary</param>
    /// <returns>Success status</returns>
    [HttpPost("keys/{keyId}/set-primary")]
    public async Task<ActionResult> SetPrimaryKey(string keyId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _encryptionService.SetPrimaryKeyAsync(userId, keyId);

            if (success)
            {
                _logger.LogInformation("Primary key set to {KeyId} for user {UserId}", keyId, userId);
                return Ok(new { Success = true, Message = "Primary key set successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to set primary key or key not found" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary key {KeyId} for user {UserId}", keyId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Initialize encryption for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <returns>Success status</returns>
    [HttpPost("conversations/{conversationId}/initialize")]
    public async Task<ActionResult> InitializeConversationEncryption(string conversationId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _encryptionService.InitializeConversationEncryptionAsync(userId, conversationId);

            if (success)
            {
                _logger.LogInformation("Encryption initialized for conversation {ConversationId} by user {UserId}", 
                    conversationId, userId);
                return Ok(new { Success = true, Message = "Conversation encryption initialized successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to initialize conversation encryption" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing encryption for conversation {ConversationId} by user {UserId}", 
                conversationId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Rotate encryption keys for a conversation
    /// </summary>
    /// <param name="request">Key rotation request</param>
    /// <returns>Success status</returns>
    [HttpPost("rotate-conversation-keys")]
    public async Task<ActionResult> RotateConversationKeys([FromBody] RotateConversationKeyRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _encryptionService.RotateConversationKeysAsync(userId, request);

            if (success)
            {
                _logger.LogInformation("Keys rotated for conversation {ConversationId} by user {UserId}: {Reason}", 
                    request.ConversationId, userId, request.Reason ?? "No reason provided");
                return Ok(new { Success = true, Message = "Conversation keys rotated successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to rotate conversation keys" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rotating keys for conversation {ConversationId} by user {UserId}", 
                request.ConversationId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get encryption status for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <returns>Encryption status information</returns>
    [HttpGet("conversations/{conversationId}/status")]
    public async Task<ActionResult<object>> GetConversationEncryptionStatus(string conversationId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var status = await _encryptionService.GetConversationEncryptionStatusAsync(userId, conversationId);

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encryption status for conversation {ConversationId} and user {UserId}", 
                conversationId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Verify message integrity
    /// </summary>
    /// <param name="messageId">Message ID to verify</param>
    /// <returns>Verification result</returns>
    [HttpPost("messages/{messageId}/verify")]
    public async Task<ActionResult> VerifyMessageIntegrity(string messageId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var verified = await _encryptionService.VerifyMessageIntegrityAsync(userId, messageId);

            return Ok(new { 
                Success = true, 
                Verified = verified, 
                Message = verified ? "Message integrity verified" : "Message integrity check failed" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying message integrity for message {MessageId} and user {UserId}", 
                messageId, GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Export public keys
    /// </summary>
    /// <param name="keyId">Specific key ID to export (optional)</param>
    /// <returns>Exported public keys</returns>
    [HttpGet("export-public-keys")]
    public async Task<ActionResult<string>> ExportPublicKeys([FromQuery] string? keyId = null)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var exportedKeys = await _encryptionService.ExportPublicKeysAsync(userId, keyId);

            return Ok(new { PublicKeys = exportedKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting public keys for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Import public keys from other users
    /// </summary>
    /// <param name="publicKeys">Public keys to import</param>
    /// <returns>Success status</returns>
    [HttpPost("import-public-keys")]
    public async Task<ActionResult> ImportPublicKeys([FromBody] string publicKeys)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _encryptionService.ImportPublicKeysAsync(userId, publicKeys);

            if (success)
            {
                _logger.LogInformation("Public keys imported successfully for user {UserId}", userId);
                return Ok(new { Success = true, Message = "Public keys imported successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to import public keys" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing public keys for user {UserId}", GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get encryption statistics
    /// </summary>
    /// <param name="fromDate">Start date (ISO 8601 format)</param>
    /// <param name="toDate">End date (ISO 8601 format)</param>
    /// <returns>Encryption usage statistics</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetEncryptionStatistics([FromQuery] string fromDate, [FromQuery] string toDate)
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

            var statistics = await _encryptionService.GetEncryptionStatisticsAsync(userId, from, to);

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting encryption statistics for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Backup encryption keys
    /// </summary>
    /// <param name="passphrase">Backup encryption passphrase</param>
    /// <returns>Encrypted backup data</returns>
    [HttpPost("backup-keys")]
    public async Task<ActionResult> BackupKeys([FromBody] string passphrase)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var backupData = await _encryptionService.BackupKeysAsync(userId, passphrase);

            _logger.LogInformation("Keys backed up for user {UserId}", userId);

            return Ok(new { BackupData = backupData, Message = "Keys backed up successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up keys for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Restore encryption keys from backup
    /// </summary>
    /// <param name="request">Restore request with backup data and passphrase</param>
    /// <returns>Success status</returns>
    [HttpPost("restore-keys")]
    public async Task<ActionResult> RestoreKeys([FromBody] object request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Extract backup data and passphrase from request
            var requestData = System.Text.Json.JsonSerializer.Serialize(request);
            var restoreData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(requestData);
            
            if (!restoreData!.TryGetValue("backupData", out var backupData) || 
                !restoreData.TryGetValue("passphrase", out var passphrase))
            {
                return BadRequest("Backup data and passphrase are required");
            }

            var success = await _encryptionService.RestoreKeysAsync(userId, backupData, passphrase);

            if (success)
            {
                _logger.LogInformation("Keys restored successfully for user {UserId}", userId);
                return Ok(new { Success = true, Message = "Keys restored successfully" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "Failed to restore keys" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring keys for user {UserId}", GetUserId());
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Perform key maintenance
    /// </summary>
    /// <returns>Maintenance results</returns>
    [HttpPost("maintenance")]
    public async Task<ActionResult<object>> PerformKeyMaintenance()
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var results = await _encryptionService.PerformKeyMaintenanceAsync(userId);

            _logger.LogInformation("Key maintenance completed for user {UserId}", userId);

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing key maintenance for user {UserId}", GetUserId());
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
}