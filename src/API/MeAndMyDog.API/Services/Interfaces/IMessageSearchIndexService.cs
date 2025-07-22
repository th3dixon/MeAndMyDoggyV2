using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for message search indexing
/// </summary>
public interface IMessageSearchIndexService
{
    /// <summary>
    /// Index a message for search
    /// </summary>
    /// <param name="message">Message to index</param>
    /// <returns>True if successfully indexed</returns>
    Task<bool> IndexMessageAsync(Message message);

    /// <summary>
    /// Update message index when message is edited
    /// </summary>
    /// <param name="message">Updated message</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> UpdateMessageIndexAsync(Message message);

    /// <summary>
    /// Remove message from search index
    /// </summary>
    /// <param name="messageId">Message ID to remove</param>
    /// <returns>True if successfully removed</returns>
    Task<bool> RemoveMessageFromIndexAsync(string messageId);

    /// <summary>
    /// Rebuild search index for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <returns>Number of messages indexed</returns>
    Task<int> RebuildConversationIndexAsync(string conversationId);

    /// <summary>
    /// Rebuild entire search index
    /// </summary>
    /// <returns>Number of messages indexed</returns>
    Task<int> RebuildFullIndexAsync();

    /// <summary>
    /// Search messages using the index
    /// </summary>
    /// <param name="userId">User performing the search</param>
    /// <param name="request">Search request</param>
    /// <returns>Search results</returns>
    Task<MessageSearchResponse> SearchMessagesAsync(string userId, MessageSearchRequest request);

    /// <summary>
    /// Get search suggestions based on query
    /// </summary>
    /// <param name="userId">User requesting suggestions</param>
    /// <param name="partialQuery">Partial search query</param>
    /// <param name="conversationIds">Optional conversation filter</param>
    /// <param name="limit">Maximum number of suggestions</param>
    /// <returns>Search suggestions</returns>
    Task<List<string>> GetSearchSuggestionsAsync(string userId, string partialQuery, List<string>? conversationIds = null, int limit = 10);

    /// <summary>
    /// Get popular search terms
    /// </summary>
    /// <param name="userId">User requesting terms</param>
    /// <param name="language">Language filter</param>
    /// <param name="limit">Maximum number of terms</param>
    /// <returns>Popular search terms</returns>
    Task<List<PopularSearchTermDto>> GetPopularSearchTermsAsync(string userId, string? language = null, int limit = 20);

    /// <summary>
    /// Log a search query for analytics
    /// </summary>
    /// <param name="userId">User performing search</param>
    /// <param name="query">Search query</param>
    /// <param name="conversationIds">Conversations searched</param>
    /// <param name="resultCount">Number of results</param>
    /// <param name="executionTimeMs">Search execution time</param>
    /// <param name="ipAddress">User's IP address</param>
    /// <param name="userAgent">User agent string</param>
    /// <returns>Search log ID</returns>
    Task<string> LogSearchQueryAsync(string userId, string query, List<string>? conversationIds, int resultCount, long executionTimeMs, string? ipAddress = null, string? userAgent = null);

    /// <summary>
    /// Update search query with user feedback
    /// </summary>
    /// <param name="searchLogId">Search log ID</param>
    /// <param name="foundResult">Whether user found what they wanted</param>
    /// <param name="clickedResultIndex">Which result was clicked</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> UpdateSearchFeedbackAsync(string searchLogId, bool foundResult, int? clickedResultIndex = null);

    /// <summary>
    /// Get search analytics for admin users
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Search analytics</returns>
    Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate);

    /// <summary>
    /// Optimize search index (remove stale entries, update statistics)
    /// </summary>
    /// <returns>Number of entries processed</returns>
    Task<int> OptimizeSearchIndexAsync();

    /// <summary>
    /// Get index statistics
    /// </summary>
    /// <returns>Index statistics</returns>
    Task<SearchIndexStatsDto> GetIndexStatisticsAsync();

    /// <summary>
    /// Extract keywords from text for indexing
    /// </summary>
    /// <param name="text">Text to analyze</param>
    /// <param name="language">Text language</param>
    /// <returns>Extracted keywords</returns>
    Task<List<string>> ExtractKeywordsAsync(string text, string language = "en");

    /// <summary>
    /// Detect language of text
    /// </summary>
    /// <param name="text">Text to analyze</param>
    /// <returns>Detected language code</returns>
    Task<string> DetectLanguageAsync(string text);

    /// <summary>
    /// Pre-process text for search indexing
    /// </summary>
    /// <param name="text">Original text</param>
    /// <param name="language">Text language</param>
    /// <returns>Processed text optimized for search</returns>
    Task<string> PreprocessTextForSearchAsync(string text, string language = "en");
}