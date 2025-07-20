namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a service offered by a service provider
/// </summary>
public class Service
{
    /// <summary>
    /// Unique identifier for the service
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Service name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Service description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Service category
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Base price for the service
    /// </summary>
    public decimal? BasePrice { get; set; }
    
    /// <summary>
    /// Pricing model (Fixed, Hourly, Per Day, etc.)
    /// </summary>
    public string? PricingModel { get; set; }
    
    /// <summary>
    /// Estimated duration in minutes
    /// </summary>
    public int? DurationMinutes { get; set; }
    
    /// <summary>
    /// Service requirements (JSON array)
    /// </summary>
    public string? Requirements { get; set; }
    
    /// <summary>
    /// What's included in the service (JSON array)
    /// </summary>
    public string? Inclusions { get; set; }
    
    /// <summary>
    /// Service location options (Home, Business, Both)
    /// </summary>
    public string LocationType { get; set; } = "Both";
    
    /// <summary>
    /// Maximum travel distance in kilometers
    /// </summary>
    public int? MaxTravelDistance { get; set; }
    
    /// <summary>
    /// Travel cost per kilometer
    /// </summary>
    public decimal? TravelCostPerKm { get; set; }
    
    /// <summary>
    /// Whether the service is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the service was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the service was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}