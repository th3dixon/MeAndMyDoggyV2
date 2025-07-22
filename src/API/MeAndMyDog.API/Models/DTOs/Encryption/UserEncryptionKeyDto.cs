namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for user encryption keys
/// </summary>
public class UserEncryptionKeyDto
{
    /// <summary>
    /// Key ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Key type (RSA, ECDSA, etc.)
    /// </summary>
    public string KeyType { get; set; } = string.Empty;

    /// <summary>
    /// Key usage (signing, encryption, both)
    /// </summary>
    public string KeyUsage { get; set; } = string.Empty;

    /// <summary>
    /// Public key in PEM format
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Key fingerprint
    /// </summary>
    public string Fingerprint { get; set; } = string.Empty;

    /// <summary>
    /// Key size in bits
    /// </summary>
    public int KeySizeBits { get; set; }

    /// <summary>
    /// Whether active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether primary key
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Device information
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Expiration date
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last used date
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }
}