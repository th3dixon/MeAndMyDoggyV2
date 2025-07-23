namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for service provider upgrade - mirrors ServiceProviderRegistrationDto
/// </summary>
public class ServiceProviderUpgradeDto
{
    /// <summary>
    /// Service category ID
    /// </summary>
    public Guid ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Offers emergency services
    /// </summary>
    public bool OffersEmergencyService { get; set; }
    
    /// <summary>
    /// Offers weekend services
    /// </summary>
    public bool OffersWeekendService { get; set; }
    
    /// <summary>
    /// Offers evening services
    /// </summary>
    public bool OffersEveningService { get; set; }
    
    /// <summary>
    /// Sub-services within this category
    /// </summary>
    public List<SubServiceUpgradeDto> SubServices { get; set; } = new();
}