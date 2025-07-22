namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for decrypted message
/// </summary>
public class DecryptedMessageResponse
{
    /// <summary>
    /// Whether decryption was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Decrypted plain text content
    /// </summary>
    public string? PlainTextContent { get; set; }

    /// <summary>
    /// Encryption information
    /// </summary>
    public MessageEncryptionDto? EncryptionInfo { get; set; }

    /// <summary>
    /// Whether integrity check passed
    /// </summary>
    public bool IntegrityVerified { get; set; }
}