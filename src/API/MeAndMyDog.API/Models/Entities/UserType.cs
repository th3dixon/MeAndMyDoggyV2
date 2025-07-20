namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Defines the type of user in the system
/// </summary>
public enum UserType
{
    /// <summary>
    /// User who owns pets and seeks services
    /// </summary>
    PetOwner = 1,
    
    /// <summary>
    /// User who provides pet services
    /// </summary>
    ServiceProvider = 2,
    
    /// <summary>
    /// User who is both a pet owner and service provider
    /// </summary>
    Both = 3
}