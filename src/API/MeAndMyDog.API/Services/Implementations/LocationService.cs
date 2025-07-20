using MeAndMyDog.API.Models;
using MeAndMyDog.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Implementation of location service providing geocoding and geospatial operations
/// </summary>
public class LocationService : ILocationService
{
    private readonly ILogger<LocationService> _logger;
    private readonly HttpClient _httpClient;
    
    // UK postcode regex pattern
    private static readonly Regex UKPostcodeRegex = new(
        @"^([A-Z]{1,2}[0-9][A-Z0-9]?) ?([0-9][A-Z]{2})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the LocationService
    /// </summary>
    /// <param name="logger">Logger for recording service operations</param>
    /// <param name="httpClient">HTTP client for external API calls</param>
    public LocationService(ILogger<LocationService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets geographical coordinates from a UK postcode (legacy method)
    /// </summary>
    public async Task<LocationCoordinates?> GetCoordinatesFromPostCodeAsync(string postCode)
    {
        return await GetCoordinatesFromPostcodeAsync(postCode);
    }

    /// <summary>
    /// Converts a UK postcode to latitude and longitude coordinates
    /// </summary>
    public Task<LocationCoordinates?> GetCoordinatesFromPostcodeAsync(
        string postcode, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(postcode))
                return Task.FromResult<LocationCoordinates?>(null);

            var normalizedPostcode = NormalizeUKPostcode(postcode);
            if (normalizedPostcode == null)
            {
                _logger.LogWarning("Invalid postcode format: {Postcode}", postcode);
                return Task.FromResult<LocationCoordinates?>(null);
            }

            // For this implementation, we'll use a mock service
            // In production, you would integrate with a real geocoding service like:
            // - UK Postcode API (postcodes.io)
            // - Google Geocoding API
            // - Ordnance Survey API
            
            var coordinates = GetMockCoordinatesForPostcode(normalizedPostcode);
            
            if (coordinates != null)
            {
                _logger.LogDebug("Found coordinates for postcode {Postcode}: {Lat}, {Lng}", 
                    normalizedPostcode, coordinates.Latitude, coordinates.Longitude);
            }

            return Task.FromResult(coordinates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coordinates for postcode: {Postcode}", postcode);
            return Task.FromResult<LocationCoordinates?>(null);
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
    /// Gets location suggestions for autocomplete functionality
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

            // Mock implementation - in production, use a real places autocomplete service
            var suggestions = GetMockLocationSuggestions(query, maxResults);
            
            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location suggestions for query: {Query}", query);
            return new List<LocationSuggestion>();
        }
    }

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

    /// <summary>
    /// Mock location suggestions for testing
    /// </summary>
    private static List<LocationSuggestion> GetMockLocationSuggestions(string query, int maxResults)
    {
        var suggestions = new List<LocationSuggestion>();
        var queryLower = query.ToLower();

        var mockSuggestions = new[]
        {
            ("London", "London, Greater London, UK", new LocationCoordinates { Latitude = 51.5074m, Longitude = -0.1278m }),
            ("Manchester", "Manchester, Greater Manchester, UK", new LocationCoordinates { Latitude = 53.4808m, Longitude = -2.2426m }),
            ("Birmingham", "Birmingham, West Midlands, UK", new LocationCoordinates { Latitude = 52.4862m, Longitude = -1.8904m }),
            ("Leeds", "Leeds, West Yorkshire, UK", new LocationCoordinates { Latitude = 53.8008m, Longitude = -1.5491m }),
            ("Liverpool", "Liverpool, Merseyside, UK", new LocationCoordinates { Latitude = 53.4084m, Longitude = -2.9916m }),
            ("Sheffield", "Sheffield, South Yorkshire, UK", new LocationCoordinates { Latitude = 53.3811m, Longitude = -1.4701m }),
            ("Bristol", "Bristol, England, UK", new LocationCoordinates { Latitude = 51.4545m, Longitude = -2.5879m }),
            ("Newcastle", "Newcastle upon Tyne, Tyne and Wear, UK", new LocationCoordinates { Latitude = 54.9783m, Longitude = -1.6178m }),
            ("Leicester", "Leicester, Leicestershire, UK", new LocationCoordinates { Latitude = 52.6369m, Longitude = -1.1398m }),
            ("Nottingham", "Nottingham, Nottinghamshire, UK", new LocationCoordinates { Latitude = 52.9548m, Longitude = -1.1581m })
        };

        foreach (var (name, description, coordinates) in mockSuggestions)
        {
            if (name.ToLower().StartsWith(queryLower) && suggestions.Count < maxResults)
            {
                suggestions.Add(new LocationSuggestion
                {
                    DisplayText = name,
                    Description = description,
                    Coordinates = coordinates,
                    LocationType = "city"
                });
            }
        }

        return suggestions;
    }

    #endregion
}