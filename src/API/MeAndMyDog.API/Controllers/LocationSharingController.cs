using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.DTOs.Location;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for location sharing functionality
/// </summary>
[ApiController]
[Route("api/v1/locations")]
[Authorize]
public class LocationSharingController : ControllerBase
{
    private readonly ILocationSharingService _locationService;
    private readonly ILogger<LocationSharingController> _logger;

    /// <summary>
    /// Initializes a new instance of LocationSharingController
    /// </summary>
    public LocationSharingController(
        ILocationSharingService locationService,
        ILogger<LocationSharingController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    /// <summary>
    /// Share a location in a conversation
    /// </summary>
    /// <param name="request">Location sharing details</param>
    /// <returns>Share location response</returns>
    [HttpPost("share")]
    [ProducesResponseType(typeof(ShareLocationResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ShareLocation([FromBody] ShareLocationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _locationService.ShareLocationAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Location shared successfully by user {UserId} in conversation {ConversationId}", 
                    userId, request.ConversationId);
                return CreatedAtAction(nameof(GetLocationShare), 
                    new { locationShareId = response.LocationShare!.Id }, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing location");
            return StatusCode(500, "An error occurred while sharing the location");
        }
    }

    /// <summary>
    /// Update a live location share
    /// </summary>
    /// <param name="request">Location update details</param>
    /// <returns>Update location response</returns>
    [HttpPut("live/update")]
    [ProducesResponseType(typeof(UpdateLocationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateLiveLocation([FromBody] UpdateLiveLocationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _locationService.UpdateLiveLocationAsync(userId, request);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                if (response.Error?.Contains("not found") == true)
                {
                    return NotFound(response.Error);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating live location");
            return StatusCode(500, "An error occurred while updating the live location");
        }
    }

    /// <summary>
    /// Stop live location sharing
    /// </summary>
    /// <param name="locationShareId">Location share ID to stop</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{locationShareId}/stop")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> StopLiveLocation(string locationShareId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _locationService.StopLiveLocationAsync(userId, locationShareId);
            if (success)
            {
                _logger.LogInformation("Live location sharing stopped for share {LocationShareId}", locationShareId);
                return Ok(new { Success = true, Message = "Live location sharing stopped successfully" });
            }
            else
            {
                return NotFound("Live location share not found or already stopped");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping live location {LocationShareId}", locationShareId);
            return StatusCode(500, "An error occurred while stopping live location sharing");
        }
    }

    /// <summary>
    /// Get location shares in a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="includeExpired">Whether to include expired live locations</param>
    /// <param name="limit">Maximum locations to return</param>
    /// <returns>List of location shares</returns>
    [HttpGet("conversations/{conversationId}")]
    [ProducesResponseType(typeof(List<LocationShareDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversationLocations(
        string conversationId,
        [FromQuery] bool includeExpired = false,
        [FromQuery] int limit = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate limit
            limit = Math.Min(Math.Max(1, limit), 100);

            var locations = await _locationService.GetConversationLocationsAsync(userId, conversationId, includeExpired, limit);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation locations for {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while getting conversation locations");
        }
    }

    /// <summary>
    /// Get a specific location share by ID
    /// </summary>
    /// <param name="locationShareId">Location share ID</param>
    /// <returns>Location share details</returns>
    [HttpGet("{locationShareId}")]
    [ProducesResponseType(typeof(LocationShareDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLocationShare(string locationShareId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var locationShare = await _locationService.GetLocationShareAsync(userId, locationShareId);
            if (locationShare == null)
            {
                return NotFound("Location share not found");
            }

            return Ok(locationShare);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location share {LocationShareId}", locationShareId);
            return StatusCode(500, "An error occurred while getting the location share");
        }
    }

    /// <summary>
    /// Get live location updates for a location share
    /// </summary>
    /// <param name="locationShareId">Location share ID</param>
    /// <param name="since">Only get updates since this timestamp</param>
    /// <param name="limit">Maximum updates to return</param>
    /// <returns>List of location updates</returns>
    [HttpGet("{locationShareId}/updates")]
    [ProducesResponseType(typeof(List<LocationUpdateDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLocationUpdates(
        string locationShareId,
        [FromQuery] DateTimeOffset? since = null,
        [FromQuery] int limit = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate limit
            limit = Math.Min(Math.Max(1, limit), 50);

            var updates = await _locationService.GetLocationUpdatesAsync(userId, locationShareId, since, limit);
            return Ok(updates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location updates for {LocationShareId}", locationShareId);
            return StatusCode(500, "An error occurred while getting location updates");
        }
    }

    /// <summary>
    /// Create a location bookmark
    /// </summary>
    /// <param name="request">Bookmark creation details</param>
    /// <returns>Location bookmark response</returns>
    [HttpPost("bookmarks")]
    [ProducesResponseType(typeof(LocationBookmarkResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateBookmark([FromBody] CreateLocationBookmarkRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _locationService.CreateBookmarkAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Location bookmark created by user {UserId}: {BookmarkName}", userId, request.Name);
                return CreatedAtAction(nameof(GetBookmark), 
                    new { bookmarkId = response.Bookmark!.Id }, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating location bookmark");
            return StatusCode(500, "An error occurred while creating the bookmark");
        }
    }

    /// <summary>
    /// Update a location bookmark
    /// </summary>
    /// <param name="bookmarkId">Bookmark ID to update</param>
    /// <param name="request">Updated bookmark details</param>
    /// <returns>Location bookmark response</returns>
    [HttpPut("bookmarks/{bookmarkId}")]
    [ProducesResponseType(typeof(LocationBookmarkResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateBookmark(string bookmarkId, [FromBody] UpdateLocationBookmarkRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _locationService.UpdateBookmarkAsync(userId, bookmarkId, request);

            if (response.Success)
            {
                _logger.LogInformation("Location bookmark {BookmarkId} updated by user {UserId}", bookmarkId, userId);
                return Ok(response);
            }
            else
            {
                if (response.Error?.Contains("not found") == true)
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location bookmark {BookmarkId}", bookmarkId);
            return StatusCode(500, "An error occurred while updating the bookmark");
        }
    }

    /// <summary>
    /// Delete a location bookmark
    /// </summary>
    /// <param name="bookmarkId">Bookmark ID to delete</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("bookmarks/{bookmarkId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteBookmark(string bookmarkId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _locationService.DeleteBookmarkAsync(userId, bookmarkId);
            if (success)
            {
                _logger.LogInformation("Location bookmark {BookmarkId} deleted by user {UserId}", bookmarkId, userId);
                return NoContent();
            }
            else
            {
                return NotFound("Location bookmark not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting location bookmark {BookmarkId}", bookmarkId);
            return StatusCode(500, "An error occurred while deleting the bookmark");
        }
    }

    /// <summary>
    /// Get user's location bookmarks
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="includeInactive">Whether to include inactive bookmarks</param>
    /// <returns>List of location bookmarks</returns>
    [HttpGet("bookmarks")]
    [ProducesResponseType(typeof(List<LocationBookmarkDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserBookmarks(
        [FromQuery] string? category = null,
        [FromQuery] bool includeInactive = false)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            LocationCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<LocationCategory>(category, true, out var parsed))
            {
                categoryEnum = parsed;
            }

            var bookmarks = await _locationService.GetUserBookmarksAsync(userId, categoryEnum, includeInactive);
            return Ok(bookmarks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user bookmarks");
            return StatusCode(500, "An error occurred while getting bookmarks");
        }
    }

    /// <summary>
    /// Get a specific location bookmark
    /// </summary>
    /// <param name="bookmarkId">Bookmark ID</param>
    /// <returns>Location bookmark details</returns>
    [HttpGet("bookmarks/{bookmarkId}")]
    [ProducesResponseType(typeof(LocationBookmarkDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetBookmark(string bookmarkId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bookmark = await _locationService.GetBookmarkAsync(userId, bookmarkId);
            if (bookmark == null)
            {
                return NotFound("Location bookmark not found");
            }

            return Ok(bookmark);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location bookmark {BookmarkId}", bookmarkId);
            return StatusCode(500, "An error occurred while getting the bookmark");
        }
    }

    /// <summary>
    /// Search for nearby locations and bookmarks
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>List of nearby location bookmarks</returns>
    [HttpPost("search/nearby")]
    [ProducesResponseType(typeof(List<LocationBookmarkDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchNearbyLocations([FromBody] SearchNearbyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var locations = await _locationService.SearchNearbyLocationsAsync(userId, request);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching nearby locations");
            return StatusCode(500, "An error occurred while searching nearby locations");
        }
    }

    /// <summary>
    /// Get popular locations based on usage
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="limit">Maximum locations to return</param>
    /// <returns>List of popular location bookmarks</returns>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(List<LocationBookmarkDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetPopularLocations(
        [FromQuery] string? category = null,
        [FromQuery] int limit = 10)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate limit
            limit = Math.Min(Math.Max(1, limit), 50);

            LocationCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<LocationCategory>(category, true, out var parsed))
            {
                categoryEnum = parsed;
            }

            var locations = await _locationService.GetPopularLocationsAsync(userId, categoryEnum, limit);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular locations");
            return StatusCode(500, "An error occurred while getting popular locations");
        }
    }

    /// <summary>
    /// Calculate distance between two coordinate points
    /// </summary>
    /// <param name="lat1">First latitude</param>
    /// <param name="lon1">First longitude</param>
    /// <param name="lat2">Second latitude</param>
    /// <param name="lon2">Second longitude</param>
    /// <returns>Distance in meters</returns>
    [HttpGet("distance")]
    [ProducesResponseType(typeof(DistanceResponse), 200)]
    [ProducesResponseType(400)]
    public IActionResult CalculateDistance(
        [FromQuery] double lat1,
        [FromQuery] double lon1,
        [FromQuery] double lat2,
        [FromQuery] double lon2)
    {
        try
        {
            // Validate coordinates
            if (lat1 < -90 || lat1 > 90 || lat2 < -90 || lat2 > 90)
            {
                return BadRequest("Latitude values must be between -90 and 90");
            }

            if (lon1 < -180 || lon1 > 180 || lon2 < -180 || lon2 > 180)
            {
                return BadRequest("Longitude values must be between -180 and 180");
            }

            var distance = _locationService.CalculateDistance(lat1, lon1, lat2, lon2);

            return Ok(new DistanceResponse
            {
                DistanceMeters = distance,
                DistanceKilometers = distance / 1000,
                DistanceMiles = distance / 1609.344
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating distance");
            return StatusCode(500, "An error occurred while calculating distance");
        }
    }

    /// <summary>
    /// Reverse geocode coordinates to address
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>Formatted address</returns>
    [HttpGet("geocode/reverse")]
    [ProducesResponseType(typeof(GeocodeResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ReverseGeocode([FromQuery] double latitude, [FromQuery] double longitude)
    {
        try
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90)
            {
                return BadRequest("Latitude must be between -90 and 90");
            }

            if (longitude < -180 || longitude > 180)
            {
                return BadRequest("Longitude must be between -180 and 180");
            }

            var address = await _locationService.ReverseGeocodeAsync(latitude, longitude);

            return Ok(new GeocodeResponse
            {
                Success = address != null,
                Address = address,
                Latitude = latitude,
                Longitude = longitude
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverse geocoding coordinates");
            return StatusCode(500, "An error occurred while reverse geocoding");
        }
    }

    /// <summary>
    /// Geocode address to coordinates
    /// </summary>
    /// <param name="address">Address to geocode</param>
    /// <returns>Coordinates</returns>
    [HttpGet("geocode/forward")]
    [ProducesResponseType(typeof(GeocodeResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GeocodeAddress([FromQuery] string address)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("Address is required");
            }

            var coordinates = await _locationService.GeocodeAddressAsync(address.Trim());

            return Ok(new GeocodeResponse
            {
                Success = coordinates.HasValue,
                Address = address,
                Latitude = coordinates?.Latitude,
                Longitude = coordinates?.Longitude
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address");
            return StatusCode(500, "An error occurred while geocoding the address");
        }
    }

    /// <summary>
    /// Get location sharing statistics for the current user
    /// </summary>
    /// <param name="fromDate">Statistics from date</param>
    /// <param name="toDate">Statistics to date</param>
    /// <returns>Location sharing statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(LocationSharingStatsDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetLocationSharingStats(
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Default to last 30 days if dates not provided
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            if (fromDate > toDate)
            {
                return BadRequest("From date cannot be after to date");
            }

            var stats = await _locationService.GetLocationSharingStatsAsync(userId, fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location sharing statistics");
            return StatusCode(500, "An error occurred while getting location sharing statistics");
        }
    }

    /// <summary>
    /// Clean up expired live location shares (Admin only)
    /// </summary>
    /// <returns>Number of cleaned up shares</returns>
    [HttpPost("cleanup/expired")]
    [ProducesResponseType(typeof(CleanupResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupExpiredShares()
    {
        try
        {
            var cleanedCount = await _locationService.CleanupExpiredLocationSharesAsync();

            return Ok(new CleanupResponse
            {
                Success = true,
                CleanedUpCount = cleanedCount,
                Message = $"Cleaned up {cleanedCount} expired location shares"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired location shares");
            return StatusCode(500, "An error occurred while cleaning up expired shares");
        }
    }
}