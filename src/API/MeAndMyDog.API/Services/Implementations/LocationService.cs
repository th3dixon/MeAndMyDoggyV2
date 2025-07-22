using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Implementation of location service providing geocoding and geospatial operations
/// </summary>
public class LocationService : ILocationService
{
    private readonly ILogger<LocationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    
    // UK postcode regex pattern
    private static readonly Regex UKPostcodeRegex = new(
        @"^([A-Z]{1,2}[0-9][A-Z0-9]?) ?([0-9][A-Z]{2})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Cache configuration
    private static readonly TimeSpan LocationSuggestionsCacheExpiry = TimeSpan.FromDays(30);
    private const string LocationSuggestionsCacheKeyPrefix = "location_suggestions:";

    /// <summary>
    /// Initializes a new instance of the LocationService
    /// </summary>
    /// <param name="logger">Logger for recording service operations</param>
    /// <param name="httpClient">HTTP client for external API calls</param>
    /// <param name="context">Database context for address lookup</param>
    /// <param name="cache">Memory cache for performance optimization</param>
    /// <param name="configuration">Configuration for accessing API keys</param>
    public LocationService(ILogger<LocationService> logger, HttpClient httpClient, ApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _context = context;
        _cache = cache;
        _configuration = configuration;
    }

    /// <summary>
    /// Gets geographical coordinates from a UK postcode (legacy method)
    /// </summary>
    public async Task<LocationCoordinates?> GetCoordinatesFromPostCodeAsync(string postCode)
    {
        return await GetCoordinatesFromPostcodeAsync(postCode);
    }

    /// <summary>
    /// Converts a UK postcode or city name to latitude and longitude coordinates
    /// Enhanced to support both postcodes and city names with database lookup
    /// </summary>
    public async Task<LocationCoordinates?> GetCoordinatesFromPostcodeAsync(
        string location, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(location))
                return null;

            // Create cache key for coordinate lookups
            var cacheKey = $"coordinates:{location.ToLower().Trim()}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out LocationCoordinates? cachedCoordinates))
            {
                _logger.LogDebug("Returning cached coordinates for location: {Location}", location);
                return cachedCoordinates;
            }

            LocationCoordinates? coordinates = null;

            // First, try as UK postcode
            var normalizedPostcode = NormalizeUKPostcode(location);
            if (normalizedPostcode != null)
            {
                coordinates = await GetCoordinatesFromDatabasePostcode(normalizedPostcode, cancellationToken);
                if (coordinates != null)
                {
                    _logger.LogDebug("Found coordinates for postcode {Postcode}: {Lat}, {Lng}", 
                        normalizedPostcode, coordinates.Latitude, coordinates.Longitude);
                }
            }

            // If no postcode match, try as city name
            if (coordinates == null)
            {
                coordinates = await GetCoordinatesFromDatabaseCity(location, cancellationToken);
                if (coordinates != null)
                {
                    _logger.LogDebug("Found coordinates for city {City}: {Lat}, {Lng}", 
                        location, coordinates.Latitude, coordinates.Longitude);
                }
            }

            // If no database match, try Google Places API as fallback
            if (coordinates == null)
            {
                coordinates = await GetCoordinatesFromGooglePlacesAsync(location, cancellationToken);
                if (coordinates != null)
                {
                    _logger.LogDebug("Found coordinates from Google Places API for location {Location}: {Lat}, {Lng}", 
                        location, coordinates.Latitude, coordinates.Longitude);
                }
            }

            // Cache the result (even if null) with 30-day expiry
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30),
                SlidingExpiration = TimeSpan.FromDays(7),
                Priority = CacheItemPriority.Normal
            };
            _cache.Set(cacheKey, coordinates, cacheOptions);

            if (coordinates == null)
            {
                _logger.LogWarning("No coordinates found for location: {Location}", location);
            }

            return coordinates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coordinates for location: {Location}", location);
            return null;
        }
    }

    /// <summary>
    /// Reverse geocodes coordinates to an address
    /// </summary>
    public async Task<AddressInfo?> GetAddressFromCoordinatesAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                return null;

            // Mock implementation - in production, use a real reverse geocoding service
            var addressInfo = GetMockAddressForCoordinates(latitude, longitude);
            
            return await Task.FromResult(addressInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverse geocoding coordinates: {Lat}, {Lng}", latitude, longitude);
            return null;
        }
    }

    /// <summary>
    /// Calculates the distance in miles between two geographical points (legacy method)
    /// </summary>
    public double CalculateDistanceMiles(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        return CalculateDistance((double)lat1, (double)lon1, (double)lat2, (double)lon2, DistanceUnit.Miles);
    }

    /// <summary>
    /// Calculates the distance between two geographical points using Haversine formula
    /// </summary>
    public double CalculateDistance(
        double lat1, 
        double lng1, 
        double lat2, 
        double lng2, 
        DistanceUnit unit = DistanceUnit.Miles)
    {
        const double R = 6371; // Earth's radius in kilometers
        
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        var distance = R * c; // Distance in kilometers
        
        return unit == DistanceUnit.Miles ? distance * 0.621371 : distance;
    }

    /// <summary>
    /// Finds all points within a specified radius of a center point
    /// </summary>
    public List<LocationWithDistance> FindPointsWithinRadius(
        double centerLat, 
        double centerLng, 
        double radiusMiles, 
        IEnumerable<LocationCoordinates> points)
    {
        var results = new List<LocationWithDistance>();

        foreach (var point in points)
        {
            var distance = CalculateDistance(
                centerLat, centerLng, 
                (double)point.Latitude, (double)point.Longitude, 
                DistanceUnit.Miles);

            if (distance <= radiusMiles)
            {
                results.Add(new LocationWithDistance
                {
                    Latitude = point.Latitude,
                    Longitude = point.Longitude,
                    City = point.City,
                    County = point.County,
                    Distance = distance,
                    Unit = DistanceUnit.Miles
                });
            }
        }

        return results.OrderBy(r => r.Distance).ToList();
    }

    /// <summary>
    /// Validates a UK postcode format
    /// </summary>
    public bool IsValidUKPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return false;

        return UKPostcodeRegex.IsMatch(postcode.Trim());
    }

    /// <summary>
    /// Normalizes a UK postcode to standard format
    /// </summary>
    public string? NormalizeUKPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return null;

        var cleaned = postcode.Trim().ToUpperInvariant().Replace(" ", "");
        
        if (cleaned.Length < 5 || cleaned.Length > 7)
            return null;

        // Insert space before the last 3 characters
        if (cleaned.Length >= 3)
        {
            var formatted = cleaned.Insert(cleaned.Length - 3, " ");
            return IsValidUKPostcode(formatted) ? formatted : null;
        }

        return null;
    }

    /// <summary>
    /// Gets the bounding box coordinates for a given center point and radius
    /// </summary>
    public BoundingBox GetBoundingBox(double centerLat, double centerLng, double radiusMiles)
    {
        const double earthRadiusMiles = 3959;
        var radiusInRadians = radiusMiles / earthRadiusMiles;

        var latRadian = ToRadians(centerLat);
        var lngRadian = ToRadians(centerLng);

        var minLat = centerLat - ToDegrees(radiusInRadians);
        var maxLat = centerLat + ToDegrees(radiusInRadians);

        var deltaLng = Math.Asin(Math.Sin(radiusInRadians) / Math.Cos(latRadian));
        var minLng = centerLng - ToDegrees(deltaLng);
        var maxLng = centerLng + ToDegrees(deltaLng);

        return new BoundingBox
        {
            North = maxLat,
            South = minLat,
            East = maxLng,
            West = minLng
        };
    }

    /// <summary>
    /// Geocodes a free-form address string
    /// </summary>
    public async Task<List<GeocodeResult>> GeocodeAddressAsync(
        string address, 
        string countryCode = "GB", 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(address))
                return new List<GeocodeResult>();

            // Mock implementation - in production, use a real geocoding service
            var results = GetMockGeocodeResults(address, countryCode);
            
            return await Task.FromResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", address);
            return new List<GeocodeResult>();
        }
    }

    /// <summary>
    /// Gets location suggestions for autocomplete functionality using database lookup with caching
    /// </summary>
    public async Task<List<LocationSuggestion>> GetLocationSuggestionsAsync(
        string query, 
        string countryCode = "GB", 
        int maxResults = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<LocationSuggestion>();

            // Create cache key based on normalized query and parameters
            var normalizedQuery = query.ToLower().Trim();
            var cacheKey = $"{LocationSuggestionsCacheKeyPrefix}{normalizedQuery}:{countryCode}:{maxResults}";

            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out List<LocationSuggestion>? cachedSuggestions))
            {
                _logger.LogDebug("Returning cached location suggestions for query: {Query}", query);
                return cachedSuggestions ?? new List<LocationSuggestion>();
            }

            _logger.LogDebug("Cache miss for location suggestions query: {Query}, executing database search", query);

            var suggestions = new List<LocationSuggestion>();

            // Search postcodes first (most specific)
            await AddPostcodeSuggestions(normalizedQuery, suggestions, maxResults, cancellationToken);

            // If not enough results, search cities
            if (suggestions.Count < maxResults)
            {
                await AddCitySuggestions(normalizedQuery, suggestions, maxResults, cancellationToken);
            }

            // If still not enough, search postcode areas
            if (suggestions.Count < maxResults)
            {
                await AddPostcodeAreaSuggestions(normalizedQuery, suggestions, maxResults, cancellationToken);
            }

            var finalResults = suggestions.Take(maxResults).ToList();

            // Cache the results with sliding expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = LocationSuggestionsCacheExpiry,
                SlidingExpiration = TimeSpan.FromDays(7), // Reset expiry if accessed within 7 days
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, finalResults, cacheEntryOptions);

            _logger.LogDebug("Cached {Count} location suggestions for query: {Query}", finalResults.Count, query);

            return finalResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location suggestions for query: {Query}", query);
            return new List<LocationSuggestion>();
        }
    }

    #region Database Search Helper Methods

    /// <summary>
    /// Adds postcode suggestions to the results list
    /// </summary>
    private async Task AddPostcodeSuggestions(string queryLower, List<LocationSuggestion> suggestions, int maxResults, CancellationToken cancellationToken)
    {
        if (suggestions.Count >= maxResults) return;

        var remaining = maxResults - suggestions.Count;
        
        // Search for postcodes that start with the query (most relevant)
        var postcodes = await _context.Postcodes
            .Where(p => p.IsActive && 
                       (p.PostcodeFormatted.ToLower().StartsWith(queryLower) ||
                        p.PostcodeCode.ToLower().StartsWith(queryLower.Replace(" ", ""))))
            .OrderBy(p => p.PostcodeFormatted.Length) // Prefer shorter/exact matches
            .Take(remaining)
            .Select(p => new
            {
                p.PostcodeFormatted,
                p.Latitude,
                p.Longitude,
                p.PostcodeArea,
                p.PostcodeDistrict
            })
            .ToListAsync(cancellationToken);

        foreach (var postcode in postcodes)
        {
            suggestions.Add(new LocationSuggestion
            {
                DisplayText = postcode.PostcodeFormatted,
                Description = $"{postcode.PostcodeFormatted}, {postcode.PostcodeArea} Area, UK",
                LocationType = "postcode",
                Postcode = postcode.PostcodeFormatted,
                Coordinates = new LocationCoordinates
                {
                    Latitude = postcode.Latitude,
                    Longitude = postcode.Longitude
                }
            });
        }
    }

    /// <summary>
    /// Adds city suggestions to the results list with first postcode coordinates
    /// </summary>
    private async Task AddCitySuggestions(string queryLower, List<LocationSuggestion> suggestions, int maxResults, CancellationToken cancellationToken)
    {
        if (suggestions.Count >= maxResults) return;

        var remaining = maxResults - suggestions.Count;

        // Get cities with their first postcode for precise coordinates
        var citiesWithPostcodes = await (
            from city in _context.Cities.Include(c => c.County)
            join postcode in _context.Postcodes on city.CityName equals postcode.PostcodeDistrict into postcodes
            from p in postcodes.Take(1) // Get first postcode for this city
            where city.IsActive && 
                  city.CityName.ToLower().StartsWith(queryLower) &&
                  p.IsActive
            orderby city.CityName.Length, city.CityName // Prefer shorter names first
            select new
            {
                city.CityName,
                CountyName = city.County.CountyName,
                PostcodeFormatted = p.PostcodeFormatted,
                Latitude = p.Latitude,
                Longitude = p.Longitude
            })
            .Take(remaining)
            .ToListAsync(cancellationToken);

        // If no postcodes found, fall back to city coordinates
        if (!citiesWithPostcodes.Any())
        {
            var citiesOnly = await _context.Cities
                .Include(c => c.County)
                .Where(c => c.IsActive && 
                           c.CityName.ToLower().StartsWith(queryLower) &&
                           c.Latitude.HasValue && c.Longitude.HasValue)
                .OrderBy(c => c.CityName.Length)
                .ThenBy(c => c.CityName)
                .Take(remaining)
                .Select(c => new
                {
                    c.CityName,
                    CountyName = c.County.CountyName,
                    PostcodeFormatted = (string?)null,
                    Latitude = c.Latitude!.Value, // Convert nullable to non-nullable
                    Longitude = c.Longitude!.Value // Convert nullable to non-nullable
                })
                .ToListAsync(cancellationToken);
            
            citiesWithPostcodes.AddRange(citiesOnly);
        }

        foreach (var city in citiesWithPostcodes)
        {
            suggestions.Add(new LocationSuggestion
            {
                DisplayText = city.CityName,
                Description = $"{city.CityName}, {city.CountyName}, UK",
                LocationType = "city",
                Postcode = city.PostcodeFormatted, // Include postcode if available
                Coordinates = new LocationCoordinates
                {
                    Latitude = city.Latitude, // Already non-nullable decimal
                    Longitude = city.Longitude, // Already non-nullable decimal
                    City = city.CityName,
                    County = city.CountyName
                }
            });
        }
    }

    /// <summary>
    /// Adds postcode area suggestions to the results list
    /// </summary>
    private async Task AddPostcodeAreaSuggestions(string queryLower, List<LocationSuggestion> suggestions, int maxResults, CancellationToken cancellationToken)
    {
        if (suggestions.Count >= maxResults) return;

        var remaining = maxResults - suggestions.Count;

        var areas = await _context.PostcodeAreas
            .Where(pa => (pa.PostcodeAreaCode.ToLower().StartsWith(queryLower) ||
                         pa.AreaName.ToLower().StartsWith(queryLower)) &&
                        pa.CenterLatitude.HasValue && pa.CenterLongitude.HasValue)
            .OrderBy(pa => pa.PostcodeAreaCode.Length)
            .ThenBy(pa => pa.AreaName)
            .Take(remaining)
            .Select(pa => new
            {
                pa.PostcodeAreaCode,
                pa.AreaName,
                pa.Region,
                pa.CenterLatitude,
                pa.CenterLongitude
            })
            .ToListAsync(cancellationToken);

        foreach (var area in areas)
        {
            suggestions.Add(new LocationSuggestion
            {
                DisplayText = $"{area.PostcodeAreaCode} Area",
                Description = $"{area.AreaName} ({area.PostcodeAreaCode}), {area.Region ?? "UK"}",
                LocationType = "area",
                Coordinates = new LocationCoordinates
                {
                    Latitude = area.CenterLatitude!.Value,
                    Longitude = area.CenterLongitude!.Value
                }
            });
        }
    }

    #endregion

    #region Database Lookup Helper Methods

    /// <summary>
    /// Gets coordinates from database postcodes table
    /// </summary>
    private async Task<LocationCoordinates?> GetCoordinatesFromDatabasePostcode(string normalizedPostcode, CancellationToken cancellationToken)
    {
        try
        {
            var postcode = await _context.Postcodes
                .AsNoTracking()
                .Where(p => p.PostcodeFormatted == normalizedPostcode && p.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (postcode != null)
            {
                return new LocationCoordinates
                {
                    Latitude = postcode.Latitude,
                    Longitude = postcode.Longitude,
                    City = postcode.PostcodeDistrict, // Use district as city approximation
                    County = postcode.PostcodeArea + " Area"
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up postcode in database: {Postcode}", normalizedPostcode);
            return null;
        }
    }

    /// <summary>
    /// Gets coordinates from database cities table
    /// </summary>
    private async Task<LocationCoordinates?> GetCoordinatesFromDatabaseCity(string cityName, CancellationToken cancellationToken)
    {
        try
        {
            var normalizedCityName = cityName.ToLower().Trim();

            // Search for city by name (case-insensitive)
            var city = await _context.Cities
                .AsNoTracking()
                .Include(c => c.County)
                .Where(c => c.IsActive && 
                           c.Latitude.HasValue && c.Longitude.HasValue &&
                           (c.CityName.ToLower() == normalizedCityName ||
                            (c.AlternativeName != null && c.AlternativeName.ToLower() == normalizedCityName)))
                .OrderBy(c => c.Population.HasValue ? 0 : 1) // Prioritize cities with population data
                .ThenByDescending(c => c.Population) // Then by population size
                .FirstOrDefaultAsync(cancellationToken);

            if (city != null)
            {
                return new LocationCoordinates
                {
                    Latitude = city.Latitude!.Value,
                    Longitude = city.Longitude!.Value,
                    City = city.CityName,
                    County = city.County.CountyName
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up city in database: {CityName}", cityName);
            return null;
        }
    }

    #endregion

    #region Google Places API Helper Methods

    /// <summary>
    /// Gets coordinates from Google Places API as fallback when database lookup fails
    /// </summary>
    private async Task<LocationCoordinates?> GetCoordinatesFromGooglePlacesAsync(string location, CancellationToken cancellationToken)
    {
        try
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Google Maps API key not configured, skipping Google Places fallback");
                return null;
            }

            // Use Google Places API for geocoding
            var url = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={Uri.EscapeDataString(location)}&inputtype=textquery&fields=formatted_address,geometry,name,place_id&key={apiKey}";
            
            _logger.LogDebug("Google Places API geocoding request for location: {Location}", location);
            
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                
                if (data.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates.EnumerateArray().First();
                    
                    if (firstCandidate.TryGetProperty("geometry", out var geometry) && 
                        geometry.TryGetProperty("location", out var geometryLocation))
                    {
                        var latitude = (decimal)geometryLocation.GetProperty("lat").GetDouble();
                        var longitude = (decimal)geometryLocation.GetProperty("lng").GetDouble();
                        
                        // Extract city and county information if available
                        var formattedAddress = firstCandidate.GetProperty("formatted_address").GetString();
                        var (city, county) = ExtractLocationFromFormattedAddress(formattedAddress);
                        
                        _logger.LogInformation("Successfully geocoded '{Location}' using Google Places API: {Lat}, {Lng}", 
                            location, latitude, longitude);
                        
                        return new LocationCoordinates
                        {
                            Latitude = latitude,
                            Longitude = longitude,
                            City = city,
                            County = county
                        };
                    }
                }
                else
                {
                    _logger.LogDebug("No candidates found in Google Places API response for location: {Location}", location);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Google Places API request failed: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using Google Places API for location: {Location}", location);
        }
        
        return null;
    }

    /// <summary>
    /// Extracts city and county information from Google Places formatted address
    /// </summary>
    private static (string city, string county) ExtractLocationFromFormattedAddress(string? formattedAddress)
    {
        if (string.IsNullOrEmpty(formattedAddress))
            return ("", "");

        // Split the formatted address by commas and try to extract city and county
        // Example: "Cambridge, UK" or "Cambridge, Cambridgeshire, UK" or "10 Downing Street, London SW1A 2AA, UK"
        var parts = formattedAddress.Split(',').Select(p => p.Trim()).ToArray();
        
        string city = "";
        string county = "";
        
        // Work backwards from the end, skipping country
        for (int i = parts.Length - 2; i >= 0; i--) // Skip "UK" at the end
        {
            var part = parts[i];
            
            // Skip postcode patterns
            if (UKPostcodeRegex.IsMatch(part))
                continue;
            
            // Skip street addresses (contains numbers)
            if (part.Any(char.IsDigit))
                continue;
            
            // First non-postcode, non-address part is likely the city
            if (string.IsNullOrEmpty(city))
            {
                city = part;
            }
            // Second part might be county
            else if (string.IsNullOrEmpty(county) && 
                     (part.EndsWith("shire") || part.Contains("County") || part.Length > 4))
            {
                county = part;
                break; // Found both city and county
            }
        }
        
        return (city, county);
    }

    #endregion

    #region Private Helper Methods

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    private static double ToDegrees(double radians)
    {
        return radians * 180 / Math.PI;
    }

    /// <summary>
    /// Mock coordinates for testing - replace with real geocoding service
    /// </summary>
    private static LocationCoordinates? GetMockCoordinatesForPostcode(string postcode)
    {
        // Mock data for common London postcodes
        var mockCoordinates = new Dictionary<string, (decimal lat, decimal lng, string city, string county)>
        {
            { "SW1A 1AA", (51.5014m, -0.1419m, "London", "Greater London") }, // Buckingham Palace
            { "W1K 5NT", (51.5074m, -0.1278m, "London", "Greater London") }, // Mayfair
            { "W1U 6TU", (51.5194m, -0.1553m, "London", "Greater London") }, // Baker Street
            { "W1G 9QJ", (51.5208m, -0.1475m, "London", "Greater London") }, // Harley Street
            { "W8 4PU", (51.5050m, -0.1822m, "London", "Greater London") }, // Kensington
            { "TW9 3AQ", (51.4479m, -0.2588m, "Richmond", "Greater London") }, // Richmond
            { "NW1 7JG", (51.5429m, -0.1419m, "London", "Greater London") }, // Camden
            { "SE10 8QY", (51.4816m, 0.0082m, "Greenwich", "Greater London") }, // Greenwich
            { "SW4 0AX", (51.4654m, -0.1390m, "London", "Greater London") }, // Clapham
            { "NW3 1AH", (51.5556m, -0.1657m, "London", "Greater London") }, // Hampstead
            { "E1 6AN", (51.5144m, -0.0596m, "London", "Greater London") }, // Whitechapel
            { "W11 2HL", (51.5096m, -0.1955m, "London", "Greater London") }, // Notting Hill
            { "W1K 7TN", (51.5048m, -0.1517m, "London", "Greater London") }, // Park Lane
            { "SW3 6NP", (51.4875m, -0.1687m, "London", "Greater London") }, // Chelsea
            { "SW7 2AZ", (51.4945m, -0.1763m, "London", "Greater London") }, // South Kensington
            { "N1 9AG", (51.5454m, -0.1035m, "London", "Greater London") }, // Islington
        };

        if (mockCoordinates.TryGetValue(postcode, out var coords))
        {
            return new LocationCoordinates
            {
                Latitude = coords.lat,
                Longitude = coords.lng,
                City = coords.city,
                County = coords.county
            };
        }

        // Return approximate coordinates for London area if not found
        return new LocationCoordinates
        {
            Latitude = 51.5074m,
            Longitude = -0.1278m,
            City = "London",
            County = "Greater London"
        };
    }

    /// <summary>
    /// Mock address for testing - replace with real reverse geocoding service
    /// </summary>
    private static AddressInfo? GetMockAddressForCoordinates(double latitude, double longitude)
    {
        // Very simple mock - just return London for coordinates in Greater London area
        if (latitude >= 51.3 && latitude <= 51.7 && longitude >= -0.5 && longitude <= 0.3)
        {
            return new AddressInfo
            {
                FormattedAddress = $"Approximate location near {latitude:F4}, {longitude:F4}, London, UK",
                City = "London",
                County = "Greater London",
                Country = "United Kingdom",
                CountryCode = "GB"
            };
        }

        return new AddressInfo
        {
            FormattedAddress = $"Location at {latitude:F4}, {longitude:F4}",
            Country = "United Kingdom",
            CountryCode = "GB"
        };
    }

    /// <summary>
    /// Mock geocoding results for testing
    /// </summary>
    private static List<GeocodeResult> GetMockGeocodeResults(string address, string countryCode)
    {
        var results = new List<GeocodeResult>();

        // Simple pattern matching for common locations
        if (address.ToLower().Contains("london"))
        {
            results.Add(new GeocodeResult
            {
                FormattedAddress = "London, UK",
                Coordinates = new LocationCoordinates { Latitude = 51.5074m, Longitude = -0.1278m },
                AddressInfo = new AddressInfo 
                { 
                    City = "London", 
                    County = "Greater London", 
                    Country = "United Kingdom",
                    CountryCode = "GB"
                },
                Confidence = 0.9,
                LocationType = "city"
            });
        }

        if (address.ToLower().Contains("manchester"))
        {
            results.Add(new GeocodeResult
            {
                FormattedAddress = "Manchester, UK",
                Coordinates = new LocationCoordinates { Latitude = 53.4808m, Longitude = -2.2426m },
                AddressInfo = new AddressInfo 
                { 
                    City = "Manchester", 
                    County = "Greater Manchester", 
                    Country = "United Kingdom",
                    CountryCode = "GB"
                },
                Confidence = 0.85,
                LocationType = "city"
            });
        }

        return results;
    }


    #endregion
}