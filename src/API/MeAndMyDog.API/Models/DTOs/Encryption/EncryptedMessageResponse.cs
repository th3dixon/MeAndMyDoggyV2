namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for encrypted message
/// </summary>
public class EncryptedMessageResponse
{
    /// <summary>
    /// Whether encryption was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Message ID
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Encrypted content
    /// </summary>
    public string? EncryptedContent { get; set; }

    /// <summary>
    /// Encryption metadata
    /// </summary>
    public MessageEncryptionDto? EncryptionInfo { get; set; }

    /// <summary>
    /// Key information used
    /// </summary>
    public string? KeyId { get; set; }
}