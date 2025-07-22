namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for key revocation
/// </summary>
public class RevokeKeyRequest
{
    /// <summary>
    /// Key ID to revoke
    /// </summary>
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// User's private key passphrase for verification
    /// </summary>
    public string PrivateKeyPassphrase { get; set; } = string.Empty;
}