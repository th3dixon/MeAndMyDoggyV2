namespace MeAndMyDog.API.Models.DTOs.ServiceCatalog;

/// <summary>
/// Data transfer object for sub-service information
/// </summary>
public class SubServiceDto
{
    /// <summary>
    /// Unique identifier for the sub-service
    /// </summary>
    public Guid SubServiceId { get; set; }
    
    /// <summary>
    /// Name of the sub-service
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the sub-service
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in minutes for this service
    /// </summary>
    public int? DurationMinutes { get; set; }
    
    /// <summary>
    /// Suggested minimum price for this service
    /// </summary>
    public decimal SuggestedMinPrice { get; set; }
    
    /// <summary>
    /// Suggested maximum price for this service
    /// </summary>
    public decimal SuggestedMaxPrice { get; set; }
    
    /// <summary>
    /// Type of pricing (PerHour, PerService, etc.)
    /// </summary>
    public string PricingType { get; set; } = string.Empty;
    
    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
}