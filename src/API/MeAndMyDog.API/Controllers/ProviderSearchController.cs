using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs.ProviderSearch;
using MeAndMyDog.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for provider search operations including location-based search, filtering, and availability
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ProviderSearchController : ControllerBase
{
    private readonly IProviderSearchService _providerSearchService;
    private readonly ILocationService _locationService;
    private readonly ILogger<ProviderSearchController> _logger;

    /// <summary>
    /// Initializes a new instance of the ProviderSearchController
    /// </summary>
    /// <param name="providerSearchService">Service for provider search operations</param>
    /// <param name="locationService">Service for location operations</param>
    /// <param name="logger">Logger instance for this controller</param>
    public ProviderSearchController(
        IProviderSearchService providerSearchService,
        ILocationService locationService,
        ILogger<ProviderSearchController> logger)
    {
        _providerSearchService = providerSearchService;
        _locationService = locationService;
        _logger = logger;
    }

    /// <summary>
    /// Searches for service providers based on comprehensive filter criteria
    /// </summary>
    /// <param name="filters">Search filter criteria including location, services, dates, and preferences</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of matching service providers</returns>
    /// <response code="200">Returns the search results</response>
    /// <response code="400">If the search criteria are invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(ProviderSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProviderSearchResponseDto>> SearchProviders(
        [FromBody] ProviderSearchFilterDto filters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Provider search request received for location: {Location}", filters.Location);

            // Validate search criteria
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate location if provided
            if (!string.IsNullOrEmpty(filters.Location) && !filters.Latitude.HasValue)
            {
                if (!_locationService.IsValidUKPostcode(filters.Location))
                {
                    // Try to geocode as address
                    var geocodeResults = await _locationService.GeocodeAddressAsync(filters.Location, cancellationToken: cancellationToken);
                    if (!geocodeResults.Any())
                    {
                        return BadRequest(new { error = "Invalid location. Please provide a valid UK postcode or address." });
                    }
                }
            }

            // Perform the search
            var searchResults = await _providerSearchService.SearchProvidersAsync(filters, cancellationToken);

            _logger.LogInformation("Provider search completed. Found {Count} providers in {Ms}ms", 
                searchResults.TotalCount, searchResults.ExecutionTimeMs);

            return Ok(searchResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during provider search");
            return StatusCode(500, new { error = "An error occurred while searching for providers." });
        }
    }

    /// <summary>
    /// Gets detailed information about a specific service provider
    /// </summary>
    /// <param name="providerId">The unique identifier of the service provider</param>
    /// <param name="includeAvailability">Whether to include availability information</param>
    /// <param name="startDate">Start date for availability check (ISO 8601 format)</param>
    /// <param name="endDate">End date for availability check (ISO 8601 format)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed provider information</returns>
    /// <response code="200">Returns the provider details</response>
    /// <response code="404">If the provider is not found</response>
    /// <response code="400">If the provider ID is invalid</response>
    [HttpGet("{providerId}")]
    [ProducesResponseType(typeof(ProviderSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProviderSearchResultDto>> GetProviderDetails(
        [FromRoute] string providerId,
        [FromQuery] bool includeAvailability = false,
        [FromQuery] DateTimeOffset? startDate = null,
        [FromQuery] DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new { error = "Provider ID is required." });
            }

            var providerDetails = await _providerSearchService.GetProviderDetailsAsync(
                providerId, includeAvailability, startDate, endDate, cancellationToken);

            if (providerDetails == null)
            {
                return NotFound(new { error = "Provider not found." });
            }

            return Ok(providerDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider details for ID: {ProviderId}", providerId);
            return StatusCode(500, new { error = "An error occurred while retrieving provider details." });
        }
    }

    /// <summary>
    /// Gets service providers near a specific location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="radiusMiles">Search radius in miles (default: 10, max: 50)</param>
    /// <param name="serviceCategories">Optional comma-separated list of service category IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nearby service providers</returns>
    /// <response code="200">Returns nearby providers</response>
    /// <response code="400">If coordinates are invalid</response>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(List<ProviderSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProviderSearchResultDto>>> GetNearbyProviders(
        [FromQuery, Required] double latitude,
        [FromQuery, Required] double longitude,
        [FromQuery, Range(1, 50)] int radiusMiles = 10,
        [FromQuery] string? serviceCategories = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            {
                return BadRequest(new { error = "Invalid coordinates provided." });
            }

            // Parse service categories
            List<string>? categoryList = null;
            if (!string.IsNullOrEmpty(serviceCategories))
            {
                categoryList = serviceCategories.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            var nearbyProviders = await _providerSearchService.GetNearbyProvidersAsync(
                latitude, longitude, radiusMiles, categoryList, cancellationToken);

            return Ok(nearbyProviders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearby providers for location: {Lat}, {Lng}", latitude, longitude);
            return StatusCode(500, new { error = "An error occurred while finding nearby providers." });
        }
    }

    /// <summary>
    /// Checks availability for a specific provider
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="startDate">Start date/time for availability check</param>
    /// <param name="endDate">End date/time for availability check</param>
    /// <param name="serviceId">Optional service ID to check capacity</param>
    /// <param name="petCount">Number of pets requiring service (default: 1)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provider availability information</returns>
    /// <response code="200">Returns availability information</response>
    /// <response code="400">If parameters are invalid</response>
    /// <response code="404">If provider is not found</response>
    [HttpGet("{providerId}/availability")]
    [ProducesResponseType(typeof(ProviderAvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderAvailabilityDto>> CheckProviderAvailability(
        [FromRoute] string providerId,
        [FromQuery, Required] DateTimeOffset startDate,
        [FromQuery, Required] DateTimeOffset endDate,
        [FromQuery] string? serviceId = null,
        [FromQuery, Range(1, 20)] int petCount = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new { error = "Provider ID is required." });
            }

            if (startDate >= endDate)
            {
                return BadRequest(new { error = "Start date must be before end date." });
            }

            if (startDate < DateTimeOffset.UtcNow.AddMinutes(-30))
            {
                return BadRequest(new { error = "Start date cannot be in the past." });
            }

            var availability = await _providerSearchService.CheckProviderAvailabilityAsync(
                providerId, startDate, endDate, serviceId, petCount, cancellationToken);

            return Ok(availability);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability for provider: {ProviderId}", providerId);
            return StatusCode(500, new { error = "An error occurred while checking provider availability." });
        }
    }

    /// <summary>
    /// Gets available time slots for a provider
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="startDate">Start date for slot search</param>
    /// <param name="endDate">End date for slot search</param>
    /// <param name="durationMinutes">Required service duration in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available time slots</returns>
    /// <response code="200">Returns available time slots</response>
    /// <response code="400">If parameters are invalid</response>
    /// <response code="404">If provider is not found</response>
    [HttpGet("{providerId}/timeslots")]
    [ProducesResponseType(typeof(List<AvailabilitySlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AvailabilitySlotDto>>> GetAvailableTimeSlots(
        [FromRoute] string providerId,
        [FromQuery, Required] DateTimeOffset startDate,
        [FromQuery, Required] DateTimeOffset endDate,
        [FromQuery, Range(15, 1440)] int durationMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new { error = "Provider ID is required." });
            }

            if (startDate >= endDate)
            {
                return BadRequest(new { error = "Start date must be before end date." });
            }

            var timeSlots = await _providerSearchService.GetAvailableTimeSlotsAsync(
                providerId, startDate, endDate, durationMinutes, cancellationToken);

            return Ok(timeSlots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time slots for provider: {ProviderId}", providerId);
            return StatusCode(500, new { error = "An error occurred while retrieving time slots." });
        }
    }

    /// <summary>
    /// Calculates pricing for a service booking
    /// </summary>
    /// <param name="providerId">Provider identifier</param>
    /// <param name="subServiceId">Sub-service identifier</param>
    /// <param name="startDate">Service start date/time</param>
    /// <param name="durationMinutes">Service duration in minutes</param>
    /// <param name="petCount">Number of pets (default: 1)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed pricing breakdown</returns>
    /// <response code="200">Returns pricing information</response>
    /// <response code="400">If parameters are invalid</response>
    /// <response code="404">If provider or service is not found</response>
    [HttpGet("{providerId}/pricing")]
    [ProducesResponseType(typeof(ServicePricingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServicePricingDto>> CalculateServicePricing(
        [FromRoute] string providerId,
        [FromQuery, Required] string subServiceId,
        [FromQuery, Required] DateTimeOffset startDate,
        [FromQuery, Range(15, 1440)] int durationMinutes,
        [FromQuery, Range(1, 20)] int petCount = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new { error = "Provider ID is required." });
            }

            if (string.IsNullOrEmpty(subServiceId))
            {
                return BadRequest(new { error = "Sub-service ID is required." });
            }

            var pricing = await _providerSearchService.CalculateServicePricingAsync(
                providerId, subServiceId, startDate, durationMinutes, petCount, cancellationToken);

            return Ok(pricing);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Pricing not found for provider: {ProviderId}, sub-service: {SubServiceId}", providerId, subServiceId);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating pricing for provider: {ProviderId}", providerId);
            return StatusCode(500, new { error = "An error occurred while calculating pricing." });
        }
    }

    /// <summary>
    /// Gets trending/popular providers in a specific area
    /// </summary>
    /// <param name="postcode">UK postcode for the search area</param>
    /// <param name="radiusMiles">Search radius in miles (default: 10, max: 25)</param>
    /// <param name="serviceCategory">Optional service category ID filter</param>
    /// <param name="limit">Maximum number of results (default: 10, max: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of trending providers</returns>
    /// <response code="200">Returns trending providers</response>
    /// <response code="400">If postcode is invalid</response>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(List<ProviderSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProviderSearchResultDto>>> GetTrendingProviders(
        [FromQuery, Required] string postcode,
        [FromQuery, Range(1, 25)] int radiusMiles = 10,
        [FromQuery] string? serviceCategory = null,
        [FromQuery, Range(1, 20)] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(postcode))
            {
                return BadRequest(new { error = "Postcode is required." });
            }

            if (!_locationService.IsValidUKPostcode(postcode))
            {
                return BadRequest(new { error = "Invalid UK postcode format." });
            }

            var trendingProviders = await _providerSearchService.GetTrendingProvidersAsync(
                postcode, radiusMiles, serviceCategory, limit, cancellationToken);

            return Ok(trendingProviders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending providers for postcode: {Postcode}", postcode);
            return StatusCode(500, new { error = "An error occurred while retrieving trending providers." });
        }
    }

    /// <summary>
    /// Searches for emergency service providers
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="serviceCategories">Required service category IDs (comma-separated)</param>
    /// <param name="radiusMiles">Search radius in miles (default: 25, max: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available emergency providers</returns>
    /// <response code="200">Returns emergency providers</response>
    /// <response code="400">If parameters are invalid</response>
    [HttpGet("emergency")]
    [ProducesResponseType(typeof(List<ProviderSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProviderSearchResultDto>>> SearchEmergencyProviders(
        [FromQuery, Required] double latitude,
        [FromQuery, Required] double longitude,
        [FromQuery, Required] string serviceCategories,
        [FromQuery, Range(1, 50)] int radiusMiles = 25,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            {
                return BadRequest(new { error = "Invalid coordinates provided." });
            }

            if (string.IsNullOrEmpty(serviceCategories))
            {
                return BadRequest(new { error = "Service categories are required for emergency search." });
            }

            var categoryList = serviceCategories.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!categoryList.Any())
            {
                return BadRequest(new { error = "At least one service category is required." });
            }

            var emergencyProviders = await _providerSearchService.SearchEmergencyProvidersAsync(
                latitude, longitude, categoryList, radiusMiles, cancellationToken);

            return Ok(emergencyProviders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching emergency providers");
            return StatusCode(500, new { error = "An error occurred while searching for emergency providers." });
        }
    }

    /// <summary>
    /// Gets location suggestions for autocomplete
    /// </summary>
    /// <param name="query">Partial location string</param>
    /// <param name="maxResults">Maximum number of suggestions (default: 10, max: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of location suggestions</returns>
    /// <response code="200">Returns location suggestions</response>
    /// <response code="400">If query is too short</response>
    [HttpGet("locations/suggestions")]
    [ProducesResponseType(typeof(List<LocationSuggestion>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<LocationSuggestion>>> GetLocationSuggestions(
        [FromQuery, Required, MinLength(2)] string query,
        [FromQuery, Range(1, 20)] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return BadRequest(new { error = "Query must be at least 2 characters long." });
            }

            var suggestions = await _locationService.GetLocationSuggestionsAsync(
                query, "GB", maxResults, cancellationToken);

            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location suggestions for query: {Query}", query);
            return StatusCode(500, new { error = "An error occurred while retrieving location suggestions." });
        }
    }

    /// <summary>
    /// Invalidates search cache (admin operation)
    /// </summary>
    /// <param name="cacheKey">Specific cache key to invalidate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success confirmation</returns>
    /// <response code="200">Cache invalidated successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not authorized</response>
    [HttpDelete("cache/{cacheKey}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> InvalidateSearchCache(
        [FromRoute] string cacheKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _providerSearchService.InvalidateSearchCacheAsync(cacheKey, cancellationToken);
            
            _logger.LogInformation("Search cache invalidated for key: {CacheKey} by user: {UserId}", 
                cacheKey, User.Identity?.Name);

            return Ok(new { message = "Cache invalidated successfully.", cacheKey });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating search cache for key: {CacheKey}", cacheKey);
            return StatusCode(500, new { error = "An error occurred while invalidating cache." });
        }
    }
}