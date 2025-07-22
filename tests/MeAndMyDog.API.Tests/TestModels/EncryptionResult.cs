namespace MeAndMyDog.API.Tests.TestModels;

/// <summary>
/// Encryption result for testing
/// </summary>
public class EncryptionResult
{
    public bool Success { get; set; }
    public string EncryptedContent { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
}