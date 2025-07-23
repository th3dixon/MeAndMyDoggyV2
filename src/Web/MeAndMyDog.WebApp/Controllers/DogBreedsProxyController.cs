using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Proxy controller for dog breed API endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DogBreedsProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DogBreedsProxyController> _logger;

    public DogBreedsProxyController(IHttpClientFactory httpClientFactory, ILogger<DogBreedsProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Search dog breeds for autocomplete functionality
    /// </summary>
    /// <param name="query">Search query for breed names</param>
    /// <param name="limit">Maximum number of results to return</param>
    /// <returns>List of matching dog breeds</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchBreeds([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.GetAsync($"api/v1/DogBreeds/search?query={Uri.EscapeDataString(query ?? "")}&limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            _logger.LogWarning("API request failed with status: {StatusCode}", response.StatusCode);
            
            // Fallback data for common breeds if API fails
            var fallbackBreeds = GetFallbackBreeds(query, limit);
            return Ok(fallbackBreeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching dog breeds with query: {Query}", query);
            
            // Return fallback data
            var fallbackBreeds = GetFallbackBreeds(query, limit);
            return Ok(fallbackBreeds);
        }
    }

    /// <summary>
    /// Get popular dog breeds
    /// </summary>
    /// <param name="limit">Maximum number of results to return</param>
    /// <returns>List of popular dog breeds</returns>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularBreeds([FromQuery] int limit = 20)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.GetAsync($"api/v1/DogBreeds/popular?limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            _logger.LogWarning("API request failed with status: {StatusCode}", response.StatusCode);
            
            // Fallback data
            var fallbackBreeds = GetFallbackBreeds("", limit);
            return Ok(fallbackBreeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular dog breeds");
            
            // Return fallback data
            var fallbackBreeds = GetFallbackBreeds("", limit);
            return Ok(fallbackBreeds);
        }
    }

    /// <summary>
    /// Provides fallback breed data when API is unavailable
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="limit">Maximum results</param>
    /// <returns>Fallback breed list</returns>
    private static List<object> GetFallbackBreeds(string? query, int limit)
    {
        var commonBreeds = new List<object>
        {
            new { Id = 1, Name = "Labrador Retriever", SizeCategory = "Large" },
            new { Id = 2, Name = "Golden Retriever", SizeCategory = "Large" },
            new { Id = 3, Name = "German Shepherd", SizeCategory = "Large" },
            new { Id = 4, Name = "Bulldog", SizeCategory = "Medium" },
            new { Id = 5, Name = "Poodle", SizeCategory = "Medium" },
            new { Id = 6, Name = "Beagle", SizeCategory = "Medium" },
            new { Id = 7, Name = "Rottweiler", SizeCategory = "Large" },
            new { Id = 8, Name = "Yorkshire Terrier", SizeCategory = "Small" },
            new { Id = 9, Name = "Dachshund", SizeCategory = "Small" },
            new { Id = 10, Name = "Siberian Husky", SizeCategory = "Large" },
            new { Id = 11, Name = "Australian Shepherd", SizeCategory = "Medium" },
            new { Id = 12, Name = "Boxer", SizeCategory = "Large" },
            new { Id = 13, Name = "Border Collie", SizeCategory = "Medium" },
            new { Id = 14, Name = "Boston Terrier", SizeCategory = "Small" },
            new { Id = 15, Name = "Cocker Spaniel", SizeCategory = "Medium" },
            new { Id = 16, Name = "Shih Tzu", SizeCategory = "Small" },
            new { Id = 17, Name = "Chihuahua", SizeCategory = "Small" },
            new { Id = 18, Name = "Pomeranian", SizeCategory = "Small" },
            new { Id = 19, Name = "French Bulldog", SizeCategory = "Small" },
            new { Id = 20, Name = "Maltese", SizeCategory = "Small" },
            new { Id = 21, Name = "Mixed Breed", SizeCategory = "Medium" }
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchQuery = query.ToLower();
            commonBreeds = commonBreeds
                .Where(b => ((string)((dynamic)b).Name).ToLower().Contains(searchQuery))
                .ToList();
        }

        return commonBreeds.Take(limit).ToList();
    }
}