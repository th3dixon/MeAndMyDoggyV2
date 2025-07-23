namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for upgrade promotion display on dashboard
/// </summary>
public class ProviderUpgradePromotionDto
{
    /// <summary>
    /// Whether to show upgrade promotion
    /// </summary>
    public bool ShowPromotion { get; set; }
    
    /// <summary>
    /// Promotional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Benefits of upgrading
    /// </summary>
    public List<string> Benefits { get; set; } = new();
    
    /// <summary>
    /// Call to action text
    /// </summary>
    public string CallToAction { get; set; } = "Become a Service Provider";
}