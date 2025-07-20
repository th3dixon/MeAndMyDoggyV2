using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents pricing configuration for a provider's service offering
/// </summary>
public class ProviderServicePricing
{
    /// <summary>
    /// Unique identifier for the provider service pricing record
    /// </summary>
    [Key]
    public Guid ProviderServicePricingId { get; set; }
    
    /// <summary>
    /// Foreign key to the ProviderService that this pricing belongs to
    /// </summary>
    [Required]
    public Guid ProviderServiceId { get; set; }
    
    /// <summary>
    /// Foreign key to the SubService that this pricing applies to
    /// </summary>
    [Required]
    public Guid SubServiceId { get; set; }
    
    /// <summary>
    /// Provider's price for this sub-service in GBP
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// How this service is priced (per service, per hour, etc.)
    /// </summary>
    [Required]
    public PricingType PricingType { get; set; }
    
    /// <summary>
    /// Whether the provider currently offers this specific sub-service
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// Minimum advance booking time in hours
    /// </summary>
    public int? MinAdvanceBookingHours { get; set; }
    
    /// <summary>
    /// Maximum advance booking time in days
    /// </summary>
    public int? MaxAdvanceBookingDays { get; set; }
    
    /// <summary>
    /// Whether this service has weekend pricing surcharge
    /// </summary>
    public bool HasWeekendSurcharge { get; set; } = false;
    
    /// <summary>
    /// Weekend surcharge percentage (e.g., 20 for 20%)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? WeekendSurchargePercentage { get; set; }
    
    /// <summary>
    /// Whether this service has evening pricing surcharge
    /// </summary>
    public bool HasEveningSurcharge { get; set; } = false;
    
    /// <summary>
    /// Evening surcharge percentage (e.g., 15 for 15%)
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? EveningSurchargePercentage { get; set; }
    
    /// <summary>
    /// Provider-specific notes for this sub-service
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Timestamp when this pricing record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Timestamp when this pricing record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the associated ProviderService entity
    /// </summary>
    public ProviderService ProviderService { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the associated SubService entity
    /// </summary>
    public SubService SubService { get; set; } = null!;
}