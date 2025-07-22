using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity for managing user encryption keys
/// </summary>
public class UserEncryptionKey
{
    /// <summary>
    /// Unique identifier for the user key
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID this key belongs to
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Key type (RSA, ECDSA, Ed25519, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string KeyType { get; set; } = string.Empty;

    /// <summary>
    /// Key usage (signing, encryption, both)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string KeyUsage { get; set; } = string.Empty;

    /// <summary>
    /// Public key in PEM format
    /// </summary>
    [Required]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Private key in encrypted PEM format (encrypted with user's master password)
    /// </summary>
    [Required]
    public string EncryptedPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// Key fingerprint for identification
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Fingerprint { get; set; } = string.Empty;

    /// <summary>
    /// Key strength/size in bits
    /// </summary>
    public int KeySizeBits { get; set; }

    /// <summary>
    /// Whether this key is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is the primary key for the user
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Device or client that generated this key
    /// </summary>
    [MaxLength(200)]
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// When the key expires (null for no expiration)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// When the key was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the key was last used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// When the key was revoked (null if not revoked)
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Reason for key revocation
    /// </summary>
    [MaxLength(500)]
    public string? RevocationReason { get; set; }
}