using MeAndMyDog.API.Models.DTOs.ProviderSearch;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for provider search operations
/// </summary>
public interface IProviderSearchService
{
    /// <summary>
    /// Searches for service providers based on filter criteria
    /// </summary>
    /// <param name="filters">Search filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    Task<ProviderSearchResponseDto> SearchProvidersAsync(
        ProviderSearchFilterDto filters, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets detailed information about a specific provider
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="includeAvailability">Whether to include availability information</param>
    /// <param name="startDate">Start date for availability check</param>
    /// <param name="endDate">End date for availability check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed provider information</returns>
    Task<ProviderSearchResultDto?> GetProviderDetailsAsync(
        string providerId, 
        bool includeAvailability = false,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets providers near a specific location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="radiusMiles">Search radius in miles</param>
    /// <param name="serviceCategories">Optional service category filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nearby providers</returns>
    Task<List<ProviderSearchResultDto>> GetNearbyProvidersAsync(
        double latitude, 
        double longitude, 
        int radiusMiles = 10,
        List<string>? serviceCategories = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks provider availability for specific dates and services
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="startDate">Start date/time</param>
    /// <param name="endDate">End date/time</param>
    /// <param name="serviceId">Optional service ID to check capacity</param>
    /// <param name="petCount">Number of pets requiring service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Availability information</returns>
    Task<ProviderAvailabilityDto> CheckProviderAvailabilityAsync(
        string providerId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string? serviceId = null,
        int petCount = 1,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets available time slots for a provider within a date range
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="serviceDurationMinutes">Duration of service in minutes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available time slots</returns>
    Task<List<AvailabilitySlotDto>> GetAvailableTimeSlotsAsync(
        string providerId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        int serviceDurationMinutes = 60,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates pricing for a service booking
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="subServiceId">Sub-service identifier</param>
    /// <param name="startDate">Service start date/time</param>
    /// <param name="durationMinutes">Service duration in minutes</param>
    /// <param name="petCount">Number of pets</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pricing breakdown</returns>
    Task<ServicePricingDto> CalculateServicePricingAsync(
        string providerId,
        string subServiceId,
        DateTimeOffset startDate,
        int durationMinutes,
        int petCount = 1,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets trending/popular providers in an area
    /// </summary>
    /// <param name="postcode">Postcode or location</param>
    /// <param name="radiusMiles">Search radius</param>
    /// <param name="serviceCategory">Optional service category filter</param>
    /// <param name="limit">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of trending providers</returns>
    Task<List<ProviderSearchResultDto>> GetTrendingProvidersAsync(
        string postcode,
        int radiusMiles = 10,
        string? serviceCategory = null,
        int limit = 10,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs an emergency service provider search
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="serviceCategories">Required service categories</param>
    /// <param name="radiusMiles">Search radius in miles</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available emergency providers</returns>
    Task<List<ProviderSearchResultDto>> SearchEmergencyProvidersAsync(
        double latitude,
        double longitude,
        List<string> serviceCategories,
        int radiusMiles = 25,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalidates search cache for a specific location or provider
    /// </summary>
    /// <param name="key">Cache key to invalidate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task InvalidateSearchCacheAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for service pricing calculations
/// </summary>
public class ServicePricingDto
{
    /// <summary>
    /// Base price for the service
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Weekend surcharge amount
    /// </summary>
    public decimal WeekendSurcharge { get; set; }
    
    /// <summary>
    /// Evening surcharge amount
    /// </summary>
    public decimal EveningSurcharge { get; set; }
    
    /// <summary>
    /// Emergency service surcharge
    /// </summary>
    public decimal EmergencySurcharge { get; set; }
    
    /// <summary>
    /// Multiple pet surcharge
    /// </summary>
    public decimal MultiplePetSurcharge { get; set; }
    
    /// <summary>
    /// Total price including all surcharges
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// Pricing breakdown details
    /// </summary>
    public List<PricingBreakdownItem> Breakdown { get; set; } = new();
    
    /// <summary>
    /// Currency code (e.g., "GBP")
    /// </summary>
    public string Currency { get; set; } = "GBP";
}

/// <summary>
/// Individual pricing breakdown item
/// </summary>
public class PricingBreakdownItem
{
    /// <summary>
    /// Description of the charge
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Amount for this item
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Type of charge: base, surcharge, discount, etc.
    /// </summary>
    public string Type { get; set; } = string.Empty;
}