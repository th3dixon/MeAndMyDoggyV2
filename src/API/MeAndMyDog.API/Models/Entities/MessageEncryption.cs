using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing message encryption metadata
/// </summary>
public class MessageEncryption
{
    /// <summary>
    /// Unique identifier for the encryption record
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the message this encryption applies to
    /// </summary>
    [Required]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the parent message
    /// </summary>
    public virtual Message Message { get; set; } = null!;

    /// <summary>
    /// Encryption algorithm used (AES256, ChaCha20, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Algorithm { get; set; } = string.Empty;

    /// <summary>
    /// Key derivation function used (PBKDF2, Argon2, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string KeyDerivationFunction { get; set; } = string.Empty;

    /// <summary>
    /// Salt used for key derivation (base64 encoded)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// Initialization vector for encryption (base64 encoded)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string InitializationVector { get; set; } = string.Empty;

    /// <summary>
    /// Number of iterations for key derivation
    /// </summary>
    public int KeyDerivationIterations { get; set; }

    /// <summary>
    /// Authentication tag for authenticated encryption (base64 encoded)
    /// </summary>
    [MaxLength(500)]
    public string? AuthenticationTag { get; set; }

    /// <summary>
    /// Key identifier for key rotation and management
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// Version of the encryption scheme for backward compatibility
    /// </summary>
    public int EncryptionVersion { get; set; } = 1;

    /// <summary>
    /// Whether this message uses end-to-end encryption
    /// </summary>
    public bool IsEndToEndEncrypted { get; set; } = true;

    /// <summary>
    /// Encrypted content hash for integrity verification
    /// </summary>
    [MaxLength(500)]
    public string? ContentHash { get; set; }

    /// <summary>
    /// Additional authenticated data (AAD) for encryption context
    /// </summary>
    [MaxLength(1000)]
    public string? AdditionalAuthenticatedData { get; set; }

    /// <summary>
    /// When the encryption was applied
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the encryption metadata was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}