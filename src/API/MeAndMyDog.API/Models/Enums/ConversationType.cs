namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of conversations
/// </summary>
public enum ConversationType
{
    /// <summary>
    /// Direct conversation between two users
    /// </summary>
    Direct = 0,
    
    /// <summary>
    /// Group conversation with multiple users
    /// </summary>
    Group = 1,
    
    /// <summary>
    /// Support conversation with customer service
    /// </summary>
    Support = 2,
    
    /// <summary>
    /// Booking-related conversation between pet owner and service provider
    /// </summary>
    Booking = 3,
    
    /// <summary>
    /// System-generated conversation for automated messages
    /// </summary>
    System = 4
}