namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for sub-service information
/// </summary>
public class ProviderSubServiceDto
{
    /// <summary>
    /// Sub-service ID
    /// </summary>
    public string SubServiceId { get; set; } = string.Empty;
    
    /// <summary>
    /// Sub-service name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Price for this sub-service
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Pricing type (per hour, per day, etc.)
    /// </summary>
    public string PricingType { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in minutes (if applicable)
    /// </summary>
    public int? DurationMinutes { get; set; }
    
    /// <summary>
    /// Whether this sub-service is currently available
    /// </summary>
    public bool IsAvailable { get; set; }
}