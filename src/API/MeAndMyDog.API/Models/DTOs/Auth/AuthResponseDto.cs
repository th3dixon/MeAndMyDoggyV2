namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for authentication responses
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Authenticated user information
    /// </summary>
    public UserDto User { get; set; } = null!;
}