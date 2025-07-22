namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for decrypting a message
/// </summary>
public class DecryptMessageRequest
{
    /// <summary>
    /// Message ID to decrypt
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// User's private key password/passphrase
    /// </summary>
    public string? PrivateKeyPassphrase { get; set; }

    /// <summary>
    /// Device information for audit logging
    /// </summary>
    public string? DeviceInfo { get; set; }
}