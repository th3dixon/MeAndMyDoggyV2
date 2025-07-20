using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for provider search filter criteria
/// </summary>
public class ProviderSearchFilterDto
{
    /// <summary>
    /// Location filter - postcode, city, or address
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Latitude coordinate for exact location search
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitude coordinate for exact location search
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Search radius in miles (default: 10)
    /// </summary>
    [Range(1, 100)]
    public int RadiusMiles { get; set; } = 10;
    
    /// <summary>
    /// Service category IDs to filter by
    /// </summary>
    public List<string>? ServiceCategoryIds { get; set; }
    
    /// <summary>
    /// Sub-service IDs to filter by
    /// </summary>
    public List<string>? SubServiceIds { get; set; }
    
    /// <summary>
    /// Minimum date/time for service availability
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }
    
    /// <summary>
    /// Maximum date/time for service availability
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    
    /// <summary>
    /// Number of pets requiring service
    /// </summary>
    [Range(1, 20)]
    public int PetCount { get; set; } = 1;
    
    /// <summary>
    /// Minimum price range
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? MinPrice { get; set; }
    
    /// <summary>
    /// Maximum price range
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; }
    
    /// <summary>
    /// Minimum provider rating (1-5 stars)
    /// </summary>
    [Range(1, 5)]
    public decimal? MinRating { get; set; }
    
    /// <summary>
    /// Only show verified providers
    /// </summary>
    public bool VerifiedOnly { get; set; } = false;
    
    /// <summary>
    /// Only show providers with emergency services
    /// </summary>
    public bool EmergencyServiceOnly { get; set; } = false;
    
    /// <summary>
    /// Only show providers available on weekends
    /// </summary>
    public bool WeekendAvailable { get; set; } = false;
    
    /// <summary>
    /// Only show providers available in evenings
    /// </summary>
    public bool EveningAvailable { get; set; } = false;
    
    /// <summary>
    /// Sort criteria: distance, price, rating, availability
    /// </summary>
    public string SortBy { get; set; } = "distance";
    
    /// <summary>
    /// Sort direction: asc, desc
    /// </summary>
    public string SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of results per page
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
    
    /// <summary>
    /// Whether to include availability details in results
    /// </summary>
    public bool IncludeAvailability { get; set; } = false;
}