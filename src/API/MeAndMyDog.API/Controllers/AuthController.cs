using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs.Auth;
using MeAndMyDog.API.Services;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for authentication operations including login, registration, and password management
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="authService">Service for authentication operations</param>
    /// <param name="logger">Logger instance for this controller</param>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="model">Registration data</param>
    /// <returns>Authentication response with user details and tokens</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        try
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }
            
            _logger.LogInformation("New user registered: {Email}", model.Email);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", model.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }
    
    /// <summary>
    /// Authenticates a user and returns access tokens
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <returns>Authentication response with user details and tokens</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        try
        {
            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            
            _logger.LogInformation("User logged in: {Email}", model.Email);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", model.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
    
    /// <summary>
    /// Refreshes an expired access token using a refresh token
    /// </summary>
    /// <param name="model">Refresh token data</param>
    /// <returns>New authentication response with fresh tokens</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(model.RefreshToken);
            if (!result.Success)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }
            
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }
    
    /// <summary>
    /// Logs out a user and invalidates their refresh token
    /// </summary>
    /// <param name="model">Logout data containing refresh token</param>
    /// <returns>Logout confirmation</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutDto model)
    {
        try
        {
            await _authService.LogoutAsync(model.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }
    
    /// <summary>
    /// Initiates password reset process by sending reset link to user's email
    /// </summary>
    /// <param name="model">Forgot password data containing email address</param>
    /// <returns>Confirmation that reset link was sent</returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(model.Email);
            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }
            
            _logger.LogInformation("Password reset requested for: {Email}", model.Email);
            return Ok(new { message = "Password reset link sent to your email address" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for {Email}", model.Email);
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }
    
    /// <summary>
    /// Resets user's password using a valid reset token
    /// </summary>
    /// <param name="model">Password reset data including token and new password</param>
    /// <returns>Confirmation that password was reset successfully</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }
            
            _logger.LogInformation("Password reset successfully for: {Email}", model.Email);
            return Ok(new { message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", model.Email);
            return StatusCode(500, new { message = "An error occurred while resetting your password" });
        }
    }
}

