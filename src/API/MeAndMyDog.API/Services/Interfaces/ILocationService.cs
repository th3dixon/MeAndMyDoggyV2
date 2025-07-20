using MeAndMyDog.API.Models;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for location and geospatial operations
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Gets geographical coordinates from a UK postcode
    /// </summary>
    /// <param name="postCode">The postcode to geocode</param>
    /// <returns>Location coordinates if found, null otherwise</returns>
    Task<LocationCoordinates?> GetCoordinatesFromPostCodeAsync(string postCode);
    
    /// <summary>
    /// Converts a UK postcode to latitude and longitude coordinates
    /// </summary>
    /// <param name="postcode">UK postcode (e.g., "SW1A 1AA")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location coordinates or null if not found</returns>
    Task<LocationCoordinates?> GetCoordinatesFromPostcodeAsync(
        string postcode, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates the distance in miles between two geographical points
    /// </summary>
    /// <param name="lat1">Latitude of first point</param>
    /// <param name="lon1">Longitude of first point</param>
    /// <param name="lat2">Latitude of second point</param>
    /// <param name="lon2">Longitude of second point</param>
    /// <returns>Distance in miles</returns>
    double CalculateDistanceMiles(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
    
    /// <summary>
    /// Reverse geocodes coordinates to an address
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Address information or null if not found</returns>
    Task<AddressInfo?> GetAddressFromCoordinatesAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates the distance between two geographical points
    /// </summary>
    /// <param name="lat1">First point latitude</param>
    /// <param name="lng1">First point longitude</param>
    /// <param name="lat2">Second point latitude</param>
    /// <param name="lng2">Second point longitude</param>
    /// <param name="unit">Distance unit (miles or kilometers)</param>
    /// <returns>Distance in the specified unit</returns>
    double CalculateDistance(
        double lat1, 
        double lng1, 
        double lat2, 
        double lng2, 
        DistanceUnit unit = DistanceUnit.Miles);
    
    /// <summary>
    /// Finds all points within a specified radius of a center point
    /// </summary>
    /// <param name="centerLat">Center point latitude</param>
    /// <param name="centerLng">Center point longitude</param>
    /// <param name="radiusMiles">Radius in miles</param>
    /// <param name="points">List of points to check</param>
    /// <returns>Points within the radius with distances</returns>
    List<LocationWithDistance> FindPointsWithinRadius(
        double centerLat, 
        double centerLng, 
        double radiusMiles, 
        IEnumerable<LocationCoordinates> points);
    
    /// <summary>
    /// Validates a UK postcode format
    /// </summary>
    /// <param name="postcode">Postcode to validate</param>
    /// <returns>True if the postcode format is valid</returns>
    bool IsValidUKPostcode(string postcode);
    
    /// <summary>
    /// Normalizes a UK postcode to standard format
    /// </summary>
    /// <param name="postcode">Postcode to normalize</param>
    /// <returns>Normalized postcode or null if invalid</returns>
    string? NormalizeUKPostcode(string postcode);
    
    /// <summary>
    /// Gets the bounding box coordinates for a given center point and radius
    /// </summary>
    /// <param name="centerLat">Center latitude</param>
    /// <param name="centerLng">Center longitude</param>
    /// <param name="radiusMiles">Radius in miles</param>
    /// <returns>Bounding box coordinates</returns>
    BoundingBox GetBoundingBox(double centerLat, double centerLng, double radiusMiles);
    
    /// <summary>
    /// Geocodes a free-form address string
    /// </summary>
    /// <param name="address">Address string (e.g., "10 Downing Street, London")</param>
    /// <param name="countryCode">Country code filter (default: "GB")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of possible location matches</returns>
    Task<List<GeocodeResult>> GeocodeAddressAsync(
        string address, 
        string countryCode = "GB", 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets location suggestions for autocomplete functionality
    /// </summary>
    /// <param name="query">Partial location string</param>
    /// <param name="countryCode">Country code filter (default: "GB")</param>
    /// <param name="maxResults">Maximum number of suggestions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of location suggestions</returns>
    Task<List<LocationSuggestion>> GetLocationSuggestionsAsync(
        string query, 
        string countryCode = "GB", 
        int maxResults = 10, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Distance unit enumeration
/// </summary>
public enum DistanceUnit
{
    /// <summary>
    /// Distance measured in miles
    /// </summary>
    Miles,
    /// <summary>
    /// Distance measured in kilometers
    /// </summary>
    Kilometers
}

/// <summary>
/// Address information from reverse geocoding
/// </summary>
public class AddressInfo
{
    /// <summary>
    /// Formatted address string
    /// </summary>
    public string FormattedAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Street number and name
    /// </summary>
    public string? Street { get; set; }
    
    /// <summary>
    /// City or town
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// County or region
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Postcode
    /// </summary>
    public string? Postcode { get; set; }
    
    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Country code (e.g., "GB")
    /// </summary>
    public string? CountryCode { get; set; }
}

/// <summary>
/// Location coordinates with distance information
/// </summary>
public class LocationWithDistance : LocationCoordinates
{
    /// <summary>
    /// Distance from the reference point
    /// </summary>
    public double Distance { get; set; }
    
    /// <summary>
    /// Distance unit
    /// </summary>
    public DistanceUnit Unit { get; set; }
}

/// <summary>
/// Bounding box coordinates
/// </summary>
public class BoundingBox
{
    /// <summary>
    /// North (maximum) latitude
    /// </summary>
    public double North { get; set; }
    
    /// <summary>
    /// South (minimum) latitude
    /// </summary>
    public double South { get; set; }
    
    /// <summary>
    /// East (maximum) longitude
    /// </summary>
    public double East { get; set; }
    
    /// <summary>
    /// West (minimum) longitude
    /// </summary>
    public double West { get; set; }
}

/// <summary>
/// Geocoding result
/// </summary>
public class GeocodeResult
{
    /// <summary>
    /// Formatted address
    /// </summary>
    public string FormattedAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Location coordinates
    /// </summary>
    public LocationCoordinates Coordinates { get; set; } = new();
    
    /// <summary>
    /// Address components
    /// </summary>
    public AddressInfo AddressInfo { get; set; } = new();
    
    /// <summary>
    /// Confidence score (0-1)
    /// </summary>
    public double Confidence { get; set; }
    
    /// <summary>
    /// Location type (e.g., "postcode", "street", "city")
    /// </summary>
    public string LocationType { get; set; } = string.Empty;
}

/// <summary>
/// Location suggestion for autocomplete
/// </summary>
public class LocationSuggestion
{
    /// <summary>
    /// Display text for the suggestion
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Location coordinates (if available)
    /// </summary>
    public LocationCoordinates? Coordinates { get; set; }
    
    /// <summary>
    /// Location type
    /// </summary>
    public string LocationType { get; set; } = string.Empty;
    
    /// <summary>
    /// Postcode (if applicable)
    /// </summary>
    public string? Postcode { get; set; }
}