namespace MeAndMyDog.API.Tests.TestModels;

/// <summary>
/// Decryption result for testing
/// </summary>
public class DecryptionResult
{
    public bool Success { get; set; }
    public string DecryptedContent { get; set; } = string.Empty;
}