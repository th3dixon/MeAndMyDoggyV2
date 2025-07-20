using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Controller for service provider search and discovery functionality
/// </summary>
public class SearchController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SearchController> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the SearchController
    /// </summary>
    /// <param name="httpClientFactory">HTTP client factory for API calls</param>
    /// <param name="logger">Logger instance</param>
    public SearchController(IHttpClientFactory httpClientFactory, ILogger<SearchController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Main service discovery page
    /// </summary>
    /// <param name="location">Initial location filter</param>
    /// <param name="service">Initial service category filter</param>
    /// <returns>Service discovery view</returns>
    [HttpGet]
    public async Task<IActionResult> Index(string? location = null, string? service = null)
    {
        try
        {
            _logger.LogInformation("Loading service discovery page with location: {Location}, service: {Service}", location, service);

            // Get service categories for the dropdown
            var serviceCategories = await GetServiceCategoriesAsync();
            
            // Pass initial filters to view
            ViewBag.InitialLocation = location;
            ViewBag.InitialService = service;
            ViewBag.ServiceCategories = serviceCategories;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service discovery page");
            TempData["Error"] = "Unable to load the service discovery page. Please try again.";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// API endpoint for searching providers (AJAX)
    /// </summary>
    /// <param name="filters">Search filter parameters</param>
    /// <returns>JSON response with search results</returns>
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] ProviderSearchRequest filters)
    {
        try
        {
            _logger.LogInformation("Provider search request received for location: {Location}", filters.Location);

            var httpClient = _httpClientFactory.CreateClient("API");

            // Build the search filter DTO
            var searchFilter = new
            {
                Location = filters.Location,
                Latitude = filters.Latitude,
                Longitude = filters.Longitude,
                RadiusMiles = filters.RadiusMiles ?? 10,
                ServiceCategoryIds = filters.ServiceCategoryIds,
                SubServiceIds = filters.SubServiceIds,
                StartDate = filters.StartDate,
                EndDate = filters.EndDate,
                PetCount = filters.PetCount ?? 1,
                MinPrice = filters.MinPrice,
                MaxPrice = filters.MaxPrice,
                MinRating = filters.MinRating,
                VerifiedOnly = filters.VerifiedOnly ?? false,
                EmergencyServiceOnly = filters.EmergencyServiceOnly ?? false,
                WeekendAvailable = filters.WeekendAvailable ?? false,
                EveningAvailable = filters.EveningAvailable ?? false,
                SortBy = filters.SortBy ?? "distance",
                SortDirection = filters.SortDirection ?? "asc",
                Page = filters.Page ?? 1,
                PageSize = filters.PageSize ?? 20,
                IncludeAvailability = filters.IncludeAvailability ?? false
            };

            var json = JsonSerializer.Serialize(searchFilter, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("api/v1/ProviderSearch/search", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResults = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                _logger.LogInformation("Provider search completed successfully");
                return Json(new { success = true, data = searchResults });
            }
            else
            {
                _logger.LogWarning("Provider search API returned error: {StatusCode}", response.StatusCode);
                var errorContent = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Search failed. Please try again.", details = errorContent });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during provider search");
            return Json(new { success = false, message = "An error occurred while searching. Please try again." });
        }
    }

    /// <summary>
    /// Get service categories for dropdowns
    /// </summary>
    /// <returns>JSON response with service categories</returns>
    [HttpGet]
    public async Task<IActionResult> GetServiceCategories()
    {
        try
        {
            var serviceCategories = await GetServiceCategoriesAsync();
            return Json(new { success = true, data = serviceCategories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service categories");
            return Json(new { success = false, message = "Unable to load service categories" });
        }
    }

    /// <summary>
    /// Get provider details by ID
    /// </summary>
    /// <param name="id">Provider ID</param>
    /// <param name="includeAvailability">Whether to include availability data</param>
    /// <param name="startDate">Start date for availability check</param>
    /// <param name="endDate">End date for availability check</param>
    /// <returns>JSON response with provider details</returns>
    [HttpGet]
    public async Task<IActionResult> GetProvider(string id, bool includeAvailability = false, 
        DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Provider ID is required" });
            }

            var httpClient = _httpClientFactory.CreateClient("API");
            
            var queryParams = new List<string>();
            if (includeAvailability) queryParams.Add($"includeAvailability={includeAvailability}");
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:O}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:O}");
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await httpClient.GetAsync($"api/v1/ProviderSearch/{id}{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var providerDetails = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                return Json(new { success = true, data = providerDetails });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Json(new { success = false, message = "Provider not found" });
            }
            else
            {
                _logger.LogWarning("Get provider API returned error: {StatusCode}", response.StatusCode);
                return Json(new { success = false, message = "Unable to load provider details" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider details for ID: {ProviderId}", id);
            return Json(new { success = false, message = "An error occurred while loading provider details" });
        }
    }

    /// <summary>
    /// Get nearby providers for map display
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="radiusMiles">Search radius in miles</param>
    /// <param name="serviceCategories">Comma-separated service category IDs</param>
    /// <returns>JSON response with nearby providers</returns>
    [HttpGet]
    public async Task<IActionResult> GetNearby(double latitude, double longitude, 
        int radiusMiles = 10, string? serviceCategories = null)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            
            var queryParams = new List<string>
            {
                $"latitude={latitude}",
                $"longitude={longitude}",
                $"radiusMiles={radiusMiles}"
            };
            
            if (!string.IsNullOrEmpty(serviceCategories))
            {
                queryParams.Add($"serviceCategories={serviceCategories}");
            }
            
            var queryString = "?" + string.Join("&", queryParams);
            var response = await httpClient.GetAsync($"api/v1/ProviderSearch/nearby{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var nearbyProviders = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                return Json(new { success = true, data = nearbyProviders });
            }
            else
            {
                _logger.LogWarning("Get nearby providers API returned error: {StatusCode}", response.StatusCode);
                return Json(new { success = false, message = "Unable to load nearby providers" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearby providers");
            return Json(new { success = false, message = "An error occurred while loading nearby providers" });
        }
    }

    /// <summary>
    /// Get location suggestions for autocomplete
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <returns>JSON response with location suggestions</returns>
    [HttpGet]
    public async Task<IActionResult> GetLocationSuggestions(string query, int maxResults = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return Json(new { success = false, message = "Query must be at least 2 characters" });
            }

            var httpClient = _httpClientFactory.CreateClient("API");
            var queryString = $"?query={Uri.EscapeDataString(query)}&maxResults={maxResults}";
            var response = await httpClient.GetAsync($"api/v1/ProviderSearch/locations/suggestions{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var suggestions = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                return Json(new { success = true, data = suggestions });
            }
            else
            {
                _logger.LogWarning("Get location suggestions API returned error: {StatusCode}", response.StatusCode);
                return Json(new { success = false, message = "Unable to load location suggestions" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location suggestions for query: {Query}", query);
            return Json(new { success = false, message = "An error occurred while loading suggestions" });
        }
    }

    #region Private Methods

    /// <summary>
    /// Helper method to get service categories from API
    /// </summary>
    /// <returns>Service categories object</returns>
    private async Task<object?> GetServiceCategoriesAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.GetAsync("api/v1/ServiceCatalog/public/categories");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
            }
            else
            {
                _logger.LogWarning("Service categories API returned error: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service categories from API");
            return null;
        }
    }

    #endregion
}

/// <summary>
/// Request model for provider search
/// </summary>
public class ProviderSearchRequest
{
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? RadiusMiles { get; set; }
    public List<string>? ServiceCategoryIds { get; set; }
    public List<string>? SubServiceIds { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? PetCount { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public bool? VerifiedOnly { get; set; }
    public bool? EmergencyServiceOnly { get; set; }
    public bool? WeekendAvailable { get; set; }
    public bool? EveningAvailable { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeAvailability { get; set; }
}