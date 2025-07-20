using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a specific sub-service within a service category
/// </summary>
public class SubService
{
    /// <summary>
    /// Unique identifier for the sub-service
    /// </summary>
    [Key]
    public Guid SubServiceId { get; set; }
    
    /// <summary>
    /// Foreign key to the parent service category
    /// </summary>
    [Required]
    public Guid ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Name of the sub-service
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the sub-service
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in minutes for this service
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 minute and 24 hours")]
    public int? DurationMinutes { get; set; }
    
    /// <summary>
    /// Suggested minimum price in GBP
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be non-negative")]
    public decimal SuggestedMinPrice { get; set; }
    
    /// <summary>
    /// Suggested maximum price in GBP
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be non-negative")]
    public decimal SuggestedMaxPrice { get; set; }
    
    /// <summary>
    /// Default pricing type for this sub-service
    /// </summary>
    public PricingType DefaultPricingType { get; set; } = PricingType.PerService;
    
    /// <summary>
    /// Whether this sub-service is active and available
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Display order for sorting sub-services
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Display order must be non-negative")]
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// When this sub-service was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When this sub-service was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Parent service category (Many-to-One relationship)
    /// </summary>
    public ServiceCategory ServiceCategory { get; set; } = null!;
    
    /// <summary>
    /// Provider-specific pricing for this sub-service (One-to-Many relationship)
    /// </summary>
    public List<ProviderServicePricing> ProviderPricing { get; set; } = new();
}