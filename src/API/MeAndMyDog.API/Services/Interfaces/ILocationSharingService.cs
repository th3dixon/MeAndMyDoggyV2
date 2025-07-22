using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for location sharing functionality
/// </summary>
public interface ILocationSharingService
{
    /// <summary>
    /// Share a location in a conversation
    /// </summary>
    /// <param name="userId">User sharing the location</param>
    /// <param name="request">Location sharing details</param>
    /// <returns>Share location response</returns>
    Task<ShareLocationResponse> ShareLocationAsync(string userId, ShareLocationRequest request);

    /// <summary>
    /// Update a live location share
    /// </summary>
    /// <param name="userId">User updating the location</param>
    /// <param name="request">Location update details</param>
    /// <returns>Update location response</returns>
    Task<UpdateLocationResponse> UpdateLiveLocationAsync(string userId, UpdateLiveLocationRequest request);

    /// <summary>
    /// Stop live location sharing
    /// </summary>
    /// <param name="userId">User stopping the sharing</param>
    /// <param name="locationShareId">Location share to stop</param>
    /// <returns>True if stopped successfully</returns>
    Task<bool> StopLiveLocationAsync(string userId, string locationShareId);

    /// <summary>
    /// Get location shares in a conversation
    /// </summary>
    /// <param name="userId">User requesting the locations</param>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="includeExpired">Whether to include expired live locations</param>
    /// <param name="limit">Maximum locations to return</param>
    /// <returns>List of location shares</returns>
    Task<List<LocationShareDto>> GetConversationLocationsAsync(string userId, string conversationId, bool includeExpired = false, int limit = 50);

    /// <summary>
    /// Get a specific location share by ID
    /// </summary>
    /// <param name="userId">User requesting the location</param>
    /// <param name="locationShareId">Location share ID</param>
    /// <returns>Location share details or null if not found</returns>
    Task<LocationShareDto?> GetLocationShareAsync(string userId, string locationShareId);

    /// <summary>
    /// Get live location updates for a location share
    /// </summary>
    /// <param name="userId">User requesting the updates</param>
    /// <param name="locationShareId">Location share ID</param>
    /// <param name="since">Only get updates since this timestamp</param>
    /// <param name="limit">Maximum updates to return</param>
    /// <returns>List of location updates</returns>
    Task<List<LocationUpdateDto>> GetLocationUpdatesAsync(string userId, string locationShareId, DateTimeOffset? since = null, int limit = 20);

    /// <summary>
    /// Create a location bookmark
    /// </summary>
    /// <param name="userId">User creating the bookmark</param>
    /// <param name="request">Bookmark creation details</param>
    /// <returns>Location bookmark response</returns>
    Task<LocationBookmarkResponse> CreateBookmarkAsync(string userId, CreateLocationBookmarkRequest request);

    /// <summary>
    /// Update a location bookmark
    /// </summary>
    /// <param name="userId">User updating the bookmark</param>
    /// <param name="bookmarkId">Bookmark ID to update</param>
    /// <param name="request">Updated bookmark details</param>
    /// <returns>Location bookmark response</returns>
    Task<LocationBookmarkResponse> UpdateBookmarkAsync(string userId, string bookmarkId, UpdateLocationBookmarkRequest request);

    /// <summary>
    /// Delete a location bookmark
    /// </summary>
    /// <param name="userId">User deleting the bookmark</param>
    /// <param name="bookmarkId">Bookmark ID to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteBookmarkAsync(string userId, string bookmarkId);

    /// <summary>
    /// Get user's location bookmarks
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="includeInactive">Whether to include inactive bookmarks</param>
    /// <returns>List of location bookmarks</returns>
    Task<List<LocationBookmarkDto>> GetUserBookmarksAsync(string userId, LocationCategory? category = null, bool includeInactive = false);

    /// <summary>
    /// Get a specific location bookmark
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="bookmarkId">Bookmark ID</param>
    /// <returns>Location bookmark or null if not found</returns>
    Task<LocationBookmarkDto?> GetBookmarkAsync(string userId, string bookmarkId);

    /// <summary>
    /// Search for nearby locations and bookmarks
    /// </summary>
    /// <param name="userId">User performing the search</param>
    /// <param name="request">Search criteria</param>
    /// <returns>List of nearby location bookmarks</returns>
    Task<List<LocationBookmarkDto>> SearchNearbyLocationsAsync(string userId, SearchNearbyRequest request);

    /// <summary>
    /// Get popular locations based on usage
    /// </summary>
    /// <param name="userId">User requesting popular locations</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="limit">Maximum locations to return</param>
    /// <returns>List of popular location bookmarks</returns>
    Task<List<LocationBookmarkDto>> GetPopularLocationsAsync(string userId, LocationCategory? category = null, int limit = 10);

    /// <summary>
    /// Calculate distance between two coordinate points
    /// </summary>
    /// <param name="lat1">First latitude</param>
    /// <param name="lon1">First longitude</param>
    /// <param name="lat2">Second latitude</param>
    /// <param name="lon2">Second longitude</param>
    /// <returns>Distance in meters</returns>
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);

    /// <summary>
    /// Reverse geocode coordinates to address
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>Formatted address or null if not found</returns>
    Task<string?> ReverseGeocodeAsync(double latitude, double longitude);

    /// <summary>
    /// Geocode address to coordinates
    /// </summary>
    /// <param name="address">Address to geocode</param>
    /// <returns>Coordinates or null if not found</returns>
    Task<(double Latitude, double Longitude)?> GeocodeAddressAsync(string address);

    /// <summary>
    /// Clean up expired live location shares
    /// </summary>
    /// <returns>Number of expired shares cleaned up</returns>
    Task<int> CleanupExpiredLocationSharesAsync();

    /// <summary>
    /// Get location sharing statistics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Statistics from date</param>
    /// <param name="toDate">Statistics to date</param>
    /// <returns>Location sharing statistics</returns>
    Task<LocationSharingStatsDto> GetLocationSharingStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
}