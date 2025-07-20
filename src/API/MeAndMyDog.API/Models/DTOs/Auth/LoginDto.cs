using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for user login requests
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}