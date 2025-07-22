using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for advanced message search functionality
/// </summary>
public interface IMessageSearchService
{
    /// <summary>
    /// Search messages with advanced filtering and sorting
    /// </summary>
    /// <param name="userId">User performing the search</param>
    /// <param name="request">Search criteria</param>
    /// <returns>Search results</returns>
    Task<SearchMessageResponse> SearchMessagesAsync(string userId, SearchMessageRequest request);

    /// <summary>
    /// Get search suggestions based on query
    /// </summary>
    /// <param name="userId">User requesting suggestions</param>
    /// <param name="query">Partial search query</param>
    /// <param name="limit">Maximum suggestions to return</param>
    /// <returns>List of search suggestions</returns>
    Task<List<string>> GetSearchSuggestionsAsync(string userId, string query, int limit = 10);

    /// <summary>
    /// Save a search for future use
    /// </summary>
    /// <param name="userId">User saving the search</param>
    /// <param name="request">Search to save</param>
    /// <returns>Saved search response</returns>
    Task<SavedSearchResponse> SaveSearchAsync(string userId, CreateSavedSearchRequest request);

    /// <summary>
    /// Update an existing saved search
    /// </summary>
    /// <param name="userId">User updating the search</param>
    /// <param name="searchId">ID of search to update</param>
    /// <param name="request">Updated search data</param>
    /// <returns>Updated search response</returns>
    Task<SavedSearchResponse> UpdateSavedSearchAsync(string userId, string searchId, UpdateSavedSearchRequest request);

    /// <summary>
    /// Delete a saved search
    /// </summary>
    /// <param name="userId">User deleting the search</param>
    /// <param name="searchId">ID of search to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteSavedSearchAsync(string userId, string searchId);

    /// <summary>
    /// Get saved searches for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="includeInactive">Whether to include inactive searches</param>
    /// <returns>List of saved searches</returns>
    Task<List<SavedSearchDto>> GetSavedSearchesAsync(string userId, bool includeInactive = false);

    /// <summary>
    /// Get a specific saved search by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="searchId">Search ID</param>
    /// <returns>Saved search or null if not found</returns>
    Task<SavedSearchDto?> GetSavedSearchAsync(string userId, string searchId);

    /// <summary>
    /// Execute a saved search
    /// </summary>
    /// <param name="userId">User executing the search</param>
    /// <param name="searchId">Saved search ID</param>
    /// <param name="skip">Results to skip for pagination</param>
    /// <param name="take">Results to take for pagination</param>
    /// <returns>Search results</returns>
    Task<SearchMessageResponse> ExecuteSavedSearchAsync(string userId, string searchId, int skip = 0, int take = 20);

    /// <summary>
    /// Get search analytics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Date range start</param>
    /// <param name="toDate">Date range end</param>
    /// <returns>Search analytics</returns>
    Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

    /// <summary>
    /// Get popular search queries
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="limit">Maximum queries to return</param>
    /// <returns>List of popular queries</returns>
    Task<List<PopularQueryDto>> GetPopularQueriesAsync(string userId, int limit = 10);

    /// <summary>
    /// Index messages for search (background operation)
    /// </summary>
    /// <param name="messageIds">Specific message IDs to reindex (null for all)</param>
    /// <returns>True if indexing was successful</returns>
    Task<bool> IndexMessagesAsync(List<string>? messageIds = null);

    /// <summary>
    /// Clear search history for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="olderThanDays">Clear history older than specified days (optional)</param>
    /// <returns>Number of entries cleared</returns>
    Task<int> ClearSearchHistoryAsync(string userId, int? olderThanDays = null);
}