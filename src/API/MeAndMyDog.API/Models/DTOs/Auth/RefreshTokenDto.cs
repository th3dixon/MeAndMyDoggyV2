namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for refresh token requests
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// The refresh token to exchange for a new access token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}