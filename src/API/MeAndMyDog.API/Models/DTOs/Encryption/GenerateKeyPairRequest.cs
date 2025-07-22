namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for generating encryption keys
/// </summary>
public class GenerateKeyPairRequest
{
    /// <summary>
    /// Key type (RSA, ECDSA, Ed25519)
    /// </summary>
    public string KeyType { get; set; } = "RSA";

    /// <summary>
    /// Key size in bits
    /// </summary>
    public int KeySizeBits { get; set; } = 2048;

    /// <summary>
    /// Key usage (encryption, signing, both)
    /// </summary>
    public string KeyUsage { get; set; } = "both";

    /// <summary>
    /// Private key passphrase
    /// </summary>
    public string PrivateKeyPassphrase { get; set; } = string.Empty;

    /// <summary>
    /// Device information
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Key expiration in days (null for no expiration)
    /// </summary>
    public int? ExpirationDays { get; set; }

    /// <summary>
    /// Whether this should be the primary key
    /// </summary>
    public bool SetAsPrimary { get; set; } = true;
}