namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for message encryption information
/// </summary>
public class MessageEncryptionDto
{
    /// <summary>
    /// Encryption record ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Encryption algorithm used
    /// </summary>
    public string Algorithm { get; set; } = string.Empty;

    /// <summary>
    /// Key derivation function
    /// </summary>
    public string KeyDerivationFunction { get; set; } = string.Empty;

    /// <summary>
    /// Key identifier
    /// </summary>
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// Encryption version
    /// </summary>
    public int EncryptionVersion { get; set; }

    /// <summary>
    /// Whether end-to-end encrypted
    /// </summary>
    public bool IsEndToEndEncrypted { get; set; }

    /// <summary>
    /// When encrypted
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}