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
    /// Whether the provider has a premium subscription
    /// </summary>
    public bool IsPremium { get; set; }
    
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
    /// Date of last completed job (null if no jobs completed)
    /// </summary>
    public DateTimeOffset? LastJobCompletedDate { get; set; }
    
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