using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MeAndMyDog.API.Models.DTOs.Auth;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Models.Common;
using MeAndMyDog.API.Services;
using System.Linq;

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
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterDto model)
    {
        try
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
            {
                var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                    result.Errors, 
                    "Registration failed"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return BadRequest(errorResponse);
            }
            
            _logger.LogInformation("New user registered: {Email}", model.Email);
            var successResponse = ApiResponse<AuthResponseDto>.SuccessResponse(
                result.Data!, 
                "User registered successfully"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", model.Email);
            var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                "An error occurred during registration"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Authenticates a user and returns access tokens
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <returns>Authentication response with user details and tokens</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto model)
    {
        try
        {
            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid credentials"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return Unauthorized(errorResponse);
            }
            
            _logger.LogInformation("User logged in: {Email}", model.Email);
            var successResponse = ApiResponse<AuthResponseDto>.SuccessResponse(
                result.Data!, 
                "Login successful"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", model.Email);
            var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                "An error occurred during login"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Refreshes an expired access token using a refresh token
    /// </summary>
    /// <param name="model">Refresh token data</param>
    /// <returns>New authentication response with fresh tokens</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto model)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(model.RefreshToken);
            if (!result.Success)
            {
                var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid refresh token"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return Unauthorized(errorResponse);
            }
            
            var successResponse = ApiResponse<AuthResponseDto>.SuccessResponse(
                result.Data!, 
                "Token refreshed successfully"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                "An error occurred during token refresh"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Logs out a user and invalidates their refresh token
    /// </summary>
    /// <param name="model">Logout data containing refresh token</param>
    /// <returns>Logout confirmation</returns>
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] LogoutDto model)
    {
        try
        {
            await _authService.LogoutAsync(model.RefreshToken);
            var successResponse = ApiResponse<object>.SuccessResponse(
                new { success = true }, 
                "Logged out successfully"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "An error occurred during logout"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Initiates password reset process by sending reset link to user's email
    /// </summary>
    /// <param name="model">Forgot password data containing email address</param>
    /// <returns>Confirmation that reset link was sent</returns>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(model.Email);
            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    result.Errors, 
                    "Password reset request failed"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return BadRequest(errorResponse);
            }
            
            _logger.LogInformation("Password reset requested for: {Email}", model.Email);
            var successResponse = ApiResponse<object>.SuccessResponse(
                new { success = true }, 
                "Password reset link sent to your email address"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for {Email}", model.Email);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "An error occurred while processing your request"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Resets user's password using a valid reset token
    /// </summary>
    /// <param name="model">Password reset data including token and new password</param>
    /// <returns>Confirmation that password was reset successfully</returns>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordDto model)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(model);
            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    result.Errors, 
                    "Password reset failed"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return BadRequest(errorResponse);
            }
            
            _logger.LogInformation("Password reset successfully for: {Email}", model.Email);
            var successResponse = ApiResponse<object>.SuccessResponse(
                new { success = true }, 
                "Password reset successfully"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", model.Email);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "An error occurred while resetting your password"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
    
    /// <summary>
    /// Gets the current authenticated user's information
    /// </summary>
    /// <returns>Current user data</returns>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
            var userType = User.FindFirst("UserType")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "User not found in token"
                );
                errorResponse.CorrelationId = HttpContext.TraceIdentifier;
                return Unauthorized(errorResponse);
            }

            var userData = new
            {
                id = userId,
                email = email,
                name = name,
                firstName = firstName,
                lastName = lastName,
                userType = userType,
                roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            var successResponse = ApiResponse<object>.SuccessResponse(
                userData,
                "User information retrieved successfully"
            );
            successResponse.CorrelationId = HttpContext.TraceIdentifier;
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user information for user ID: {UserId}", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "An error occurred while retrieving user information"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return StatusCode(500, errorResponse);
        }
    }
}

