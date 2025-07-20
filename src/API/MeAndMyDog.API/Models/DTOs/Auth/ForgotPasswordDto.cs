using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for forgot password requests
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// Email address to send password reset link to
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}