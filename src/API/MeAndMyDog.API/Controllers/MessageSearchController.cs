using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.DTOs.MessageSearch;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for advanced message search functionality
/// </summary>
[ApiController]
[Route("api/v1/search")]
[Authorize]
public class MessageSearchController : ControllerBase
{
    private readonly IMessageSearchService _searchService;
    private readonly ILogger<MessageSearchController> _logger;

    /// <summary>
    /// Initializes a new instance of MessageSearchController
    /// </summary>
    public MessageSearchController(
        IMessageSearchService searchService,
        ILogger<MessageSearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search messages with advanced filtering and sorting
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Search results</returns>
    [HttpPost("messages")]
    [ProducesResponseType(typeof(SearchMessageResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchMessages([FromBody] SearchMessageRequest request)
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

            var response = await _searchService.SearchMessagesAsync(userId, request);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching messages");
            return StatusCode(500, "An error occurred while searching messages");
        }
    }

    /// <summary>
    /// Get search suggestions based on partial query
    /// </summary>
    /// <param name="query">Partial search query</param>
    /// <param name="limit">Maximum suggestions to return</param>
    /// <returns>List of search suggestions</returns>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(List<string>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetSearchSuggestions([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Query must be at least 2 characters long");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate limit
            limit = Math.Min(Math.Max(1, limit), 20);

            var suggestions = await _searchService.GetSearchSuggestionsAsync(userId, query, limit);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions");
            return StatusCode(500, "An error occurred while getting suggestions");
        }
    }

    /// <summary>
    /// Save a search for future use
    /// </summary>
    /// <param name="request">Search to save</param>
    /// <returns>Saved search response</returns>
    [HttpPost("saved")]
    [ProducesResponseType(typeof(SavedSearchResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SaveSearch([FromBody] CreateSavedSearchRequest request)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Search name is required");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _searchService.SaveSearchAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Search saved successfully for user {UserId}: {SearchName}", userId, request.Name);
                return CreatedAtAction(nameof(GetSavedSearch), new { searchId = response.SavedSearch!.Id }, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving search");
            return StatusCode(500, "An error occurred while saving the search");
        }
    }

    /// <summary>
    /// Update an existing saved search
    /// </summary>
    /// <param name="searchId">ID of search to update</param>
    /// <param name="request">Updated search data</param>
    /// <returns>Updated search response</returns>
    [HttpPut("saved/{searchId}")]
    [ProducesResponseType(typeof(SavedSearchResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateSavedSearch(string searchId, [FromBody] UpdateSavedSearchRequest request)
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

            var response = await _searchService.UpdateSavedSearchAsync(userId, searchId, request);

            if (response.Success)
            {
                _logger.LogInformation("Saved search {SearchId} updated successfully", searchId);
                return Ok(response);
            }
            else
            {
                if (response.Message?.Contains("not found") == true)
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saved search {SearchId}", searchId);
            return StatusCode(500, "An error occurred while updating the saved search");
        }
    }

    /// <summary>
    /// Delete a saved search
    /// </summary>
    /// <param name="searchId">ID of search to delete</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("saved/{searchId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteSavedSearch(string searchId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _searchService.DeleteSavedSearchAsync(userId, searchId);
            if (success)
            {
                _logger.LogInformation("Saved search {SearchId} deleted successfully", searchId);
                return NoContent();
            }
            else
            {
                return NotFound("Saved search not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search {SearchId}", searchId);
            return StatusCode(500, "An error occurred while deleting the saved search");
        }
    }

    /// <summary>
    /// Get all saved searches for the current user
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive searches</param>
    /// <returns>List of saved searches</returns>
    [HttpGet("saved")]
    [ProducesResponseType(typeof(List<SavedSearchDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetSavedSearches([FromQuery] bool includeInactive = false)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var savedSearches = await _searchService.GetSavedSearchesAsync(userId, includeInactive);
            return Ok(savedSearches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved searches");
            return StatusCode(500, "An error occurred while getting saved searches");
        }
    }

    /// <summary>
    /// Get a specific saved search by ID
    /// </summary>
    /// <param name="searchId">Search ID</param>
    /// <returns>Saved search details</returns>
    [HttpGet("saved/{searchId}")]
    [ProducesResponseType(typeof(SavedSearchDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSavedSearch(string searchId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var savedSearch = await _searchService.GetSavedSearchAsync(userId, searchId);
            if (savedSearch == null)
            {
                return NotFound("Saved search not found");
            }

            return Ok(savedSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved search {SearchId}", searchId);
            return StatusCode(500, "An error occurred while getting the saved search");
        }
    }

    /// <summary>
    /// Execute a saved search
    /// </summary>
    /// <param name="searchId">Saved search ID</param>
    /// <param name="skip">Results to skip for pagination</param>
    /// <param name="take">Results to take for pagination</param>
    /// <returns>Search results</returns>
    [HttpPost("saved/{searchId}/execute")]
    [ProducesResponseType(typeof(SearchMessageResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExecuteSavedSearch(
        string searchId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            var response = await _searchService.ExecuteSavedSearchAsync(userId, searchId, skip, take);

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
            _logger.LogError(ex, "Error executing saved search {SearchId}", searchId);
            return StatusCode(500, "An error occurred while executing the saved search");
        }
    }

    /// <summary>
    /// Get search analytics for the current user
    /// </summary>
    /// <param name="fromDate">Start date for analytics</param>
    /// <param name="toDate">End date for analytics</param>
    /// <returns>Search analytics</returns>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(SearchAnalyticsDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetSearchAnalytics(
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

            var analytics = await _searchService.GetSearchAnalyticsAsync(userId, fromDate, toDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search analytics");
            return StatusCode(500, "An error occurred while getting search analytics");
        }
    }

    /// <summary>
    /// Get popular search queries for the current user
    /// </summary>
    /// <param name="limit">Maximum queries to return</param>
    /// <returns>List of popular queries</returns>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(List<PopularQueryDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetPopularQueries([FromQuery] int limit = 10)
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

            var popularQueries = await _searchService.GetPopularQueriesAsync(userId, limit);
            return Ok(popularQueries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular queries");
            return StatusCode(500, "An error occurred while getting popular queries");
        }
    }

    /// <summary>
    /// Clear search history for the current user
    /// </summary>
    /// <param name="olderThanDays">Clear history older than specified days (optional)</param>
    /// <returns>Number of entries cleared</returns>
    [HttpDelete("history")]
    [ProducesResponseType(typeof(ClearHistoryResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ClearSearchHistory([FromQuery] int? olderThanDays = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var clearedCount = await _searchService.ClearSearchHistoryAsync(userId, olderThanDays);

            _logger.LogInformation("Cleared {Count} search history entries for user {UserId}", clearedCount, userId);

            return Ok(new ClearHistoryResponse
            {
                Success = true,
                ClearedCount = clearedCount,
                Message = $"Cleared {clearedCount} search history entries"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing search history");
            return StatusCode(500, "An error occurred while clearing search history");
        }
    }

    /// <summary>
    /// Trigger message indexing (Admin only)
    /// </summary>
    /// <param name="messageIds">Specific message IDs to reindex (optional)</param>
    /// <returns>Indexing status</returns>
    [HttpPost("index")]
    [ProducesResponseType(typeof(IndexingResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> IndexMessages([FromBody] List<string>? messageIds = null)
    {
        try
        {
            var success = await _searchService.IndexMessagesAsync(messageIds);

            return Ok(new IndexingResponse
            {
                Success = success,
                Message = success ? "Indexing completed successfully" : "Indexing failed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing messages");
            return StatusCode(500, "An error occurred while indexing messages");
        }
    }
}