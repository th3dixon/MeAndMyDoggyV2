using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for sub-service registration details
/// </summary>
public class SubServiceRegistrationDto
{
    /// <summary>
    /// The sub-service identifier
    /// </summary>
    public Guid SubServiceId { get; set; }
    
    /// <summary>
    /// Price for this sub-service
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Pricing type (PerHour, PerService, etc.)
    /// </summary>
    public PricingType PricingType { get; set; }
}