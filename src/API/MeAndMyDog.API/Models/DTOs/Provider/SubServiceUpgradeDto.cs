using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for sub-service upgrade - mirrors SubServiceRegistrationDto
/// </summary>
public class SubServiceUpgradeDto
{
    /// <summary>
    /// Sub-service ID
    /// </summary>
    public Guid SubServiceId { get; set; }
    
    /// <summary>
    /// Price for this sub-service
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Pricing type (PerHour/PerService)
    /// </summary>
    public PricingType PricingType { get; set; }
}