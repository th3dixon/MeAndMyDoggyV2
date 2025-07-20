namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for provider search results
/// </summary>
public class ProviderSearchResultDto
{
    /// <summary>
    /// Provider identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Business name
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;
    
    /// <summary>
    /// Provider's full name
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;
    
    /// <summary>
    /// Business description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Provider location information
    /// </summary>
    public ProviderLocationDto Location { get; set; } = new();
    
    /// <summary>
    /// Distance from search location in miles
    /// </summary>
    public double? DistanceMiles { get; set; }
    
    /// <summary>
    /// Average rating (1-5 stars)
    /// </summary>
    public decimal Rating { get; set; }
    
    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; }
    
    /// <summary>
    /// Whether the provider is verified
    /// </summary>
    public bool IsVerified { get; set; }
    
    /// <summary>
    /// Services offered by this provider
    /// </summary>
    public List<ProviderServiceDto> Services { get; set; } = new();
    
    /// <summary>
    /// Price range for services
    /// </summary>
    public PriceRangeDto PriceRange { get; set; } = new();
    
    /// <summary>
    /// Average response time in hours
    /// </summary>
    public decimal ResponseTimeHours { get; set; }
    
    /// <summary>
    /// Reliability score (0-1)
    /// </summary>
    public decimal ReliabilityScore { get; set; }
    
    /// <summary>
    /// Years of experience
    /// </summary>
    public int? YearsOfExperience { get; set; }
    
    /// <summary>
    /// Specializations
    /// </summary>
    public List<string> Specializations { get; set; } = new();
    
    /// <summary>
    /// Availability information (if requested)
    /// </summary>
    public ProviderAvailabilityDto? Availability { get; set; }
    
    /// <summary>
    /// Profile image URL
    /// </summary>
    public string? ProfileImageUrl { get; set; }
    
    /// <summary>
    /// Business website
    /// </summary>
    public string? Website { get; set; }
    
    /// <summary>
    /// Whether provider offers emergency services
    /// </summary>
    public bool OffersEmergencyService { get; set; }
    
    /// <summary>
    /// Whether provider offers weekend services
    /// </summary>
    public bool OffersWeekendService { get; set; }
    
    /// <summary>
    /// Whether provider offers evening services
    /// </summary>
    public bool OffersEveningService { get; set; }
}

/// <summary>
/// DTO for provider location information
/// </summary>
public class ProviderLocationDto
{
    /// <summary>
    /// Postcode
    /// </summary>
    public string Postcode { get; set; } = string.Empty;
    
    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Service radius in miles
    /// </summary>
    public int ServiceRadiusMiles { get; set; }
    
    /// <summary>
    /// Latitude (optional for privacy)
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitude (optional for privacy)
    /// </summary>
    public double? Longitude { get; set; }
}

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