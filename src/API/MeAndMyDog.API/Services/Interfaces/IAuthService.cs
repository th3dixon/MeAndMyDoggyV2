using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.DTOs.Auth;

namespace MeAndMyDog.API.Services;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="model">Registration data</param>
    /// <returns>Service result with authentication response</returns>
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto model);
    
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <returns>Service result with authentication response</returns>
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model);
    
    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns>Service result with new authentication tokens</returns>
    Task<ServiceResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    
    /// <summary>
    /// Logs out a user and invalidates their refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate</param>
    /// <returns>Task representing the logout operation</returns>
    Task LogoutAsync(string refreshToken);
    
    /// <summary>
    /// Initiates password reset process for a user
    /// </summary>
    /// <param name="email">Email address for password reset</param>
    /// <returns>Service result indicating success or failure</returns>
    Task<ServiceResult<object>> ForgotPasswordAsync(string email);
    
    /// <summary>
    /// Resets a user's password using a reset token
    /// </summary>
    /// <param name="model">Password reset data</param>
    /// <returns>Service result indicating success or failure</returns>
    Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordDto model);
}