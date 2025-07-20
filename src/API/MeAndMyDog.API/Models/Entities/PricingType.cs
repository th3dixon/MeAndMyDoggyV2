namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Defines how a service is priced
/// </summary>
public enum PricingType
{
    /// <summary>
    /// Fixed price per service instance
    /// </summary>
    PerService = 1,
    
    /// <summary>
    /// Hourly rate pricing
    /// </summary>
    PerHour = 2,
    
    /// <summary>
    /// Daily rate pricing
    /// </summary>
    PerDay = 3,
    
    /// <summary>
    /// Per night pricing (e.g., overnight boarding)
    /// </summary>
    PerNight = 4,
    
    /// <summary>
    /// Weekly rate pricing
    /// </summary>
    PerWeek = 5,
    
    /// <summary>
    /// Monthly rate pricing
    /// </summary>
    PerMonth = 6
}