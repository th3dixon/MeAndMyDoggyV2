namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for key generation
/// </summary>
public class GenerateKeyPairResponse
{
    /// <summary>
    /// Whether key generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Generated key information
    /// </summary>
    public UserEncryptionKeyDto? KeyInfo { get; set; }

    /// <summary>
    /// Public key fingerprint
    /// </summary>
    public string? Fingerprint { get; set; }
}