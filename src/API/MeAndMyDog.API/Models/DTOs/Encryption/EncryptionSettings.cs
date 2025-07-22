namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Encryption configuration settings
/// </summary>
public class EncryptionSettings
{
    /// <summary>
    /// Default encryption algorithm
    /// </summary>
    public string DefaultAlgorithm { get; set; } = "AES256-GCM";

    /// <summary>
    /// Default key derivation function
    /// </summary>
    public string DefaultKDF { get; set; } = "PBKDF2";

    /// <summary>
    /// Default key derivation iterations
    /// </summary>
    public int DefaultKDFIterations { get; set; } = 100000;

    /// <summary>
    /// Key rotation interval in days
    /// </summary>
    public int KeyRotationIntervalDays { get; set; } = 90;

    /// <summary>
    /// Maximum key age in days
    /// </summary>
    public int MaxKeyAgeDays { get; set; } = 365;

    /// <summary>
    /// Whether to require end-to-end encryption for all messages
    /// </summary>
    public bool RequireEndToEndEncryption { get; set; } = false;

    /// <summary>
    /// Whether to enable perfect forward secrecy
    /// </summary>
    public bool EnablePerfectForwardSecrecy { get; set; } = true;

    /// <summary>
    /// Supported encryption algorithms
    /// </summary>
    public string[] SupportedAlgorithms { get; set; } = { "AES256-GCM", "ChaCha20-Poly1305" };

    /// <summary>
    /// Supported key types
    /// </summary>
    public string[] SupportedKeyTypes { get; set; } = { "RSA", "ECDSA", "Ed25519" };

    /// <summary>
    /// Minimum key size for RSA keys
    /// </summary>
    public int MinimumRSAKeySize { get; set; } = 2048;

    /// <summary>
    /// Whether to enable key backup and recovery
    /// </summary>
    public bool EnableKeyBackup { get; set; } = true;
}