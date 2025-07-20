namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents the geographical location data for a service provider
/// Optimized for spatial queries and distance-based searches
/// </summary>
public class ProviderLocation
{
    /// <summary>
    /// Unique identifier for the provider location
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Postcode for address lookup and validation
    /// </summary>
    public string Postcode { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address string
    /// </summary>
    public string? FullAddress { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County or region
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Country (typically "United Kingdom")
    /// </summary>
    public string Country { get; set; } = "United Kingdom";
    
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double Longitude { get; set; }
    
    /// <summary>
    /// Service radius in miles that the provider covers
    /// </summary>
    public int ServiceRadiusMiles { get; set; } = 10;
    
    /// <summary>
    /// Whether this is the primary location for the provider
    /// </summary>
    public bool IsPrimary { get; set; } = true;
    
    /// <summary>
    /// Whether this location is currently active for service
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Location type: Home, Business, Service_Area, etc.
    /// </summary>
    public string LocationType { get; set; } = "Business";
    
    /// <summary>
    /// Notes about this specific location
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When this location was added
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When this location was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}