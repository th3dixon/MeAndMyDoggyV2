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
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the SearchController
    /// </summary>
    /// <param name="httpClientFactory">HTTP client factory for API calls</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Configuration instance</param>
    public SearchController(IHttpClientFactory httpClientFactory, ILogger<SearchController> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
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
            ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"] ?? "";

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
            if (serviceCategories != null)
            {
                return Json(new { success = true, data = serviceCategories });
            }
            else
            {
                _logger.LogWarning("Service categories API returned null, returning fallback data");
                return Json(new { success = false, message = "Service categories API unavailable" });
            }
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
    /// Get provider pricing information
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <returns>JSON response with pricing data</returns>
    [HttpGet]
    public async Task<IActionResult> GetProviderPricing(string providerId, string? subServiceId = null, DateTimeOffset? startDate = null, int? durationMinutes = null, int petCount = 1)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return Json(new { success = false, message = "Provider ID is required" });
            }

            var httpClient = _httpClientFactory.CreateClient("API");
            
            // If no subServiceId provided, try to get provider details to find their first service
            if (string.IsNullOrEmpty(subServiceId))
            {
                var providerResponse = await httpClient.GetAsync($"api/v1/ProviderSearch/{providerId}");
                if (providerResponse.IsSuccessStatusCode)
                {
                    var providerContent = await providerResponse.Content.ReadAsStringAsync();
                    var providerData = JsonSerializer.Deserialize<JsonElement>(providerContent, _jsonOptions);
                    
                    // Try to extract first service's subServiceId from provider data
                    if (providerData.TryGetProperty("services", out var services) && services.ValueKind == JsonValueKind.Array)
                    {
                        var firstService = services.EnumerateArray().FirstOrDefault();
                        if (firstService.ValueKind != JsonValueKind.Undefined && 
                            firstService.TryGetProperty("serviceId", out var serviceIdProperty))
                        {
                            subServiceId = serviceIdProperty.GetString();
                        }
                    }
                }
                
                // If still no subServiceId, return an error asking for it
                if (string.IsNullOrEmpty(subServiceId))
                {
                    return Json(new { success = false, message = "Service information not available for pricing" });
                }
            }
            
            // API requires subServiceId, startDate, and durationMinutes - provide defaults for optional ones
            var effectiveStartDate = startDate ?? DateTimeOffset.Now.AddDays(1);
            var effectiveDurationMinutes = durationMinutes ?? 60; // Default 1 hour
            
            var queryString = $"?subServiceId={subServiceId}&startDate={effectiveStartDate:O}&durationMinutes={effectiveDurationMinutes}&petCount={petCount}";
            var response = await httpClient.GetAsync($"api/v1/ProviderSearch/{providerId}/pricing{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var pricingData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                return Json(new { success = true, data = pricingData });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Json(new { success = false, message = "Provider not found" });
            }
            else
            {
                _logger.LogWarning("Get provider pricing API returned error: {StatusCode}", response.StatusCode);
                return Json(new { success = false, message = "Unable to load pricing information" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider pricing for ID: {ProviderId}", providerId);
            return Json(new { success = false, message = "An error occurred while loading pricing information" });
        }
    }

    /// <summary>
    /// Get provider availability
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <param name="startDate">Start date for availability check</param>
    /// <param name="endDate">End date for availability check</param>
    /// <returns>JSON response with availability data</returns>
    [HttpGet]
    public async Task<IActionResult> GetProviderAvailability(string providerId, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
    {
        try
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return Json(new { success = false, message = "Provider ID is required" });
            }

            var httpClient = _httpClientFactory.CreateClient("API");
            
            // API requires startDate and endDate - provide defaults if not specified
            var effectiveStartDate = startDate ?? DateTimeOffset.Now;
            var effectiveEndDate = endDate ?? DateTimeOffset.Now.AddDays(14);
            
            var queryString = $"?startDate={effectiveStartDate:O}&endDate={effectiveEndDate:O}";
            var response = await httpClient.GetAsync($"api/v1/ProviderSearch/{providerId}/availability{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var availabilityData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                
                return Json(new { success = true, data = availabilityData });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Json(new { success = false, message = "Provider not found" });
            }
            else
            {
                _logger.LogWarning("Get provider availability API returned error: {StatusCode}", response.StatusCode);
                return Json(new { success = false, message = "Unable to load availability information" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider availability for ID: {ProviderId}", providerId);
            return Json(new { success = false, message = "An error occurred while loading availability information" });
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

    /// <summary>
    /// Get address suggestions using Google Places API exclusively
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <returns>JSON response with address suggestions</returns>
    [HttpGet]
    public async Task<IActionResult> GetAddressSuggestions(string query, int maxResults = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return Json(new { success = false, message = "Query must be at least 2 characters" });
            }

            _logger.LogInformation("Getting address suggestions for query: {Query}", query);

            // Use Google Places API for all address searches
            var results = await SearchGooglePlaces(query);

            return Json(new { success = true, data = results.Take(maxResults) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address suggestions for query: {Query}", query);
            return Json(new { success = false, message = "An error occurred while searching for addresses" });
        }
    }

    #region Private Methods

    private async Task<List<object>> SearchGooglePlaces(string query)
    {
        try
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Google Maps API key not configured");
                return new List<object>();
            }

            var httpClient = _httpClientFactory.CreateClient();
            
            // Use broader search to include geocode results for better address coverage
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(query)}&key={apiKey}&types=geocode&components=country:uk";
            
            _logger.LogDebug("Google Places API request: {Url}", url.Replace(apiKey, "***"));
            
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                
                if (data.TryGetProperty("predictions", out var predictions))
                {
                    var addresses = new List<object>();
                    var index = 0;
                    foreach (var prediction in predictions.EnumerateArray().Take(8)) // Get more results
                    {
                        var placeId = prediction.GetProperty("place_id").GetString();
                        var details = await GetGooglePlaceDetails(placeId, apiKey, httpClient, index++);
                        if (details != null)
                        {
                            addresses.Add(details);
                        }
                    }
                    _logger.LogInformation("Found {Count} address suggestions for query: {Query}", addresses.Count, query);
                    return addresses;
                }
                else
                {
                    _logger.LogWarning("No predictions found in Google Places response for: {Query}", query);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Google Places API request failed: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google Places search failed for: {Query}", query);
        }
        
        return new List<object>();
    }

    private async Task<object?> GetGooglePlaceDetails(string placeId, string apiKey, HttpClient httpClient, int index = 0)
    {
        try
        {
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&key={apiKey}&fields=formatted_address,geometry,address_components";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                
                if (data.TryGetProperty("result", out var result))
                {
                    var addressComponents = result.GetProperty("address_components");
                    
                    string GetComponent(string type)
                    {
                        foreach (var component in addressComponents.EnumerateArray())
                        {
                            var types = component.GetProperty("types");
                            foreach (var componentType in types.EnumerateArray())
                            {
                                if (componentType.GetString() == type)
                                {
                                    return component.GetProperty("long_name").GetString();
                                }
                            }
                        }
                        return "";
                    }
                    
                    // Enhanced city extraction with multiple fallbacks
                    string GetCity()
                    {
                        // Try multiple city component types in order of preference
                        var city = GetComponent("locality");
                        if (!string.IsNullOrEmpty(city)) return city;
                        
                        city = GetComponent("postal_town");
                        if (!string.IsNullOrEmpty(city)) return city;
                        
                        city = GetComponent("administrative_area_level_2");
                        if (!string.IsNullOrEmpty(city)) return city;
                        
                        city = GetComponent("administrative_area_level_3");
                        if (!string.IsNullOrEmpty(city)) return city;
                        
                        city = GetComponent("sublocality");
                        if (!string.IsNullOrEmpty(city)) return city;
                        
                        return "";
                    }
                    
                    var streetNumber = GetComponent("street_number");
                    var streetName = GetComponent("route");
                    var subpremise = GetComponent("subpremise"); // Flat/apartment number
                    var premise = GetComponent("premise"); // Building name
                    
                    // Build address line 1 (street address)
                    var addressLine1 = "";
                    if (!string.IsNullOrEmpty(streetNumber) && !string.IsNullOrEmpty(streetName))
                    {
                        addressLine1 = $"{streetNumber} {streetName}";
                    }
                    else if (!string.IsNullOrEmpty(streetName))
                    {
                        addressLine1 = streetName;
                    }
                    else if (!string.IsNullOrEmpty(premise))
                    {
                        addressLine1 = premise;
                    }
                    
                    // Build address line 2 (flat/apartment/suite)
                    var addressLine2 = "";
                    if (!string.IsNullOrEmpty(subpremise))
                    {
                        addressLine2 = subpremise;
                    }
                    else if (!string.IsNullOrEmpty(premise) && !string.IsNullOrEmpty(addressLine1) && premise != addressLine1)
                    {
                        addressLine2 = premise;
                    }
                    
                    var city = GetCity();
                    var county = GetComponent("administrative_area_level_1");
                    var postcode = GetComponent("postal_code");
                    
                    _logger.LogDebug("Parsed address components - AddressLine1: {AddressLine1}, AddressLine2: {AddressLine2}, City: {City}, County: {County}, Postcode: {Postcode}", 
                        addressLine1, addressLine2, city, county, postcode);
                    
                    return new
                    {
                        // Unique identifiers for Alpine.js
                        addressId = placeId, // Use Google Place ID as unique identifier
                        cacheId = $"google-{index}-{placeId.Substring(0, Math.Min(8, placeId.Length))}", // Fallback unique key
                        
                        // Address information
                        displayText = result.GetProperty("formatted_address").GetString(),
                        addressLine1 = addressLine1,
                        addressLine2 = addressLine2,
                        city = city,
                        county = county,
                        postcodeFormatted = postcode,
                        latitude = result.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble(),
                        longitude = result.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble(),
                        source = "google-places",
                        
                        // Debug information (can be removed later)
                        debug = new
                        {
                            streetNumber = streetNumber,
                            streetName = streetName,
                            subpremise = subpremise,
                            premise = premise,
                            locality = GetComponent("locality"),
                            postalTown = GetComponent("postal_town"),
                            adminLevel2 = GetComponent("administrative_area_level_2"),
                            adminLevel3 = GetComponent("administrative_area_level_3"),
                            sublocality = GetComponent("sublocality")
                        }
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get Google Place details for: {PlaceId}", placeId);
        }
        
        return null;
    }

    /// <summary>
    /// Helper method to get service categories from API
    /// </summary>
    /// <returns>Service categories object</returns>
    private async Task<object?> GetServiceCategoriesAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var fullUrl = $"{httpClient.BaseAddress}api/v1/ServiceCatalog/public/categories";
            _logger.LogInformation("Attempting to fetch service categories from: {Url}", fullUrl);
            
            var response = await httpClient.GetAsync("api/v1/ServiceCatalog/public/categories");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully retrieved service categories");
                return JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
            }
            else
            {
                _logger.LogWarning("Service categories API returned error: {StatusCode} from {Url}", response.StatusCode, fullUrl);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling service categories API. This is expected if the API backend is not running.");
            return null;
        }
    }

    #endregion
}