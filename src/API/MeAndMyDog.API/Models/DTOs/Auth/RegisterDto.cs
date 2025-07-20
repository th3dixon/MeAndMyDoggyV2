using System.ComponentModel.DataAnnotations;
using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's password (minimum 8 characters)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number
    /// </summary>
    [Phone]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// User's postal code
    /// </summary>
    [Required]
    public string PostCode { get; set; } = string.Empty;
    
    /// <summary>
    /// User's city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// First line of user's address
    /// </summary>
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// Second line of user's address
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// User's county
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Type of user account (PetOwner or ServiceProvider)
    /// </summary>
    [Required]
    public UserType UserType { get; set; }
    
    /// <summary>
    /// Business name (required for service providers)
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// Company registration number
    /// </summary>
    public string? CompanyNumber { get; set; }
    
    /// <summary>
    /// VAT registration number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Service selections for service providers
    /// </summary>
    public List<ServiceProviderRegistrationDto>? Services { get; set; }
}