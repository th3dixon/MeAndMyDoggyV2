using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for password reset requests
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// Email address of the user resetting password
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Password reset token received via email
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// New password (minimum 8 characters)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Confirmation of the new password
    /// </summary>
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}