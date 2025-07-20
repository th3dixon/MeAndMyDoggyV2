namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for logout requests
/// </summary>
public class LogoutDto
{
    /// <summary>
    /// The refresh token to invalidate during logout
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}