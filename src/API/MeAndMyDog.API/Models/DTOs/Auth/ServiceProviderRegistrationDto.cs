namespace MeAndMyDog.API.Models.DTOs.Auth;

/// <summary>
/// Data transfer object for service provider registration details
/// </summary>
public class ServiceProviderRegistrationDto
{
    /// <summary>
    /// The service category identifier
    /// </summary>
    public string ServiceCategoryId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the provider offers emergency services for this category
    /// </summary>
    public bool OffersEmergencyService { get; set; }
    
    /// <summary>
    /// Whether the provider offers weekend services for this category
    /// </summary>
    public bool OffersWeekendService { get; set; }
    
    /// <summary>
    /// Whether the provider offers evening services for this category
    /// </summary>
    public bool OffersEveningService { get; set; }
    
    /// <summary>
    /// List of sub-services offered by the provider
    /// </summary>
    public List<SubServiceRegistrationDto>? SubServices { get; set; }
}