using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a service offered by a specific provider
/// </summary>
public class ProviderService
{
    /// <summary>
    /// Unique identifier for the provider service relationship
    /// </summary>
    [Key]
    public Guid ProviderServiceId { get; set; }
    
    /// <summary>
    /// Foreign key to the provider offering this service
    /// </summary>
    [Required]
    public Guid ProviderId { get; set; }
    
    /// <summary>
    /// Foreign key to the service category being offered
    /// </summary>
    [Required]
    public Guid ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Whether the provider currently offers this service category
    /// </summary>
    public bool IsOffered { get; set; } = true;
    
    /// <summary>
    /// Provider-specific notes about this service category
    /// </summary>
    [MaxLength(1000)]
    public string? SpecialNotes { get; set; }
    
    /// <summary>
    /// Service area radius in miles for this specific service
    /// </summary>
    public int? ServiceRadiusMiles { get; set; }
    
    /// <summary>
    /// Whether the provider offers emergency/urgent bookings for this service
    /// </summary>
    public bool OffersEmergencyService { get; set; } = false;
    
    /// <summary>
    /// Whether the provider offers weekend availability for this service
    /// </summary>
    public bool OffersWeekendService { get; set; } = true;
    
    /// <summary>
    /// Whether the provider offers evening availability for this service
    /// </summary>
    public bool OffersEveningService { get; set; } = true;
    
    /// <summary>
    /// Timestamp when this provider service relationship was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Timestamp when this provider service relationship was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the service category details
    /// </summary>
    public ServiceCategory ServiceCategory { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the pricing information for this provider service
    /// </summary>
    public List<ProviderServicePricing> Pricing { get; set; } = new();
}