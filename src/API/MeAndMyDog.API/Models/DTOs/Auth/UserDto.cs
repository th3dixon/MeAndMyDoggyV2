namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to user's profile photo
    /// </summary>
    public string? ProfilePhotoUrl { get; set; }
    
    /// <summary>
    /// Type of user account
    /// </summary>
    public string UserType { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if user is a service provider
    /// </summary>
    public bool IsServiceProvider { get; set; }
    
    /// <summary>
    /// Service provider ID if user is a service provider
    /// </summary>
    public Guid? ServiceProviderId { get; set; }
}