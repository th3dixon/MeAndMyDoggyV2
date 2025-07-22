namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for price range information
/// </summary>
public class PriceRangeDto
{
    /// <summary>
    /// Minimum price across all services
    /// </summary>
    public decimal MinPrice { get; set; }
    
    /// <summary>
    /// Maximum price across all services
    /// </summary>
    public decimal MaxPrice { get; set; }
    
    /// <summary>
    /// Most common pricing type
    /// </summary>
    public string? CommonPricingType { get; set; }
}