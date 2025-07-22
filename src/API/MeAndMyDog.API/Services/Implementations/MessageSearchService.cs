using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for advanced message search functionality
/// </summary>
public class MessageSearchService : IMessageSearchService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessageSearchService> _logger;

    /// <summary>
    /// Initialize the message search service
    /// </summary>
    public MessageSearchService(ApplicationDbContext context, ILogger<MessageSearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<SearchMessageResponse> SearchMessagesAsync(string userId, SearchMessageRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Build base query for user's accessible messages
            var query = BuildBaseMessageQuery(userId);

            // Apply search filters
            query = ApplySearchFilters(query, request);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, request.SortBy);

            // Apply pagination
            var results = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(m => new
                {
                    m.Id,
                    m.ConversationId,
                    ConversationName = m.Conversation.Name ?? "Direct Message",
                    m.SenderId,
                    SenderName = m.Sender.DisplayName ?? m.Sender.UserName ?? "Unknown",
                    m.Content,
                    MessageType = EnumConverter.ToMessageType(m.MessageType),
                    m.SentAt,
                    m.IsEncrypted,
                    HasAttachments = m.Attachments.Any(),
                    AttachmentCount = m.Attachments.Count(),
                    Tags = m.Tags ?? ""
                })
                .ToListAsync();

            // Process results and calculate relevance scores
            var searchResults = results.Select(r => 
            {
                var result = new MessageSearchResultDto
                {
                    MessageId = r.Id,
                    ConversationId = r.ConversationId,
                    ConversationName = r.ConversationName,
                    SenderId = r.SenderId,
                    SenderName = r.SenderName,
                    Content = request.HighlightMatches ? HighlightMatches(r.Content, request.Query) : r.Content,
                    OriginalContent = r.Content,
                    MessageType = r.MessageType,
                    SentAt = r.SentAt,
                    IsEncrypted = r.IsEncrypted,
                    HasAttachments = r.HasAttachments,
                    AttachmentCount = r.AttachmentCount,
                    Tags = ParseTags(r.Tags),
                    RelevanceScore = CalculateRelevanceScore(r.Content, request.Query, request.SortBy)
                };

                // Add matched snippets
                if (!string.IsNullOrEmpty(request.Query))
                {
                    result.MatchedSnippets = ExtractMatchedSnippets(r.Content, request.Query);
                }

                return result;
            }).ToList();

            // Add context messages if requested
            if (request.IncludeContext && searchResults.Any())
            {
                await AddContextMessages(searchResults, userId);
            }

            stopwatch.Stop();

            // Record search in history
            await RecordSearchHistoryAsync(userId, request, totalCount, (int)stopwatch.ElapsedMilliseconds);

            var response = new SearchMessageResponse
            {
                Success = true,
                Results = searchResults,
                TotalCount = totalCount,
                ReturnedCount = searchResults.Count,
                Skip = request.Skip,
                Take = request.Take,
                HasMore = (request.Skip + searchResults.Count) < totalCount,
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds,
                Query = request.Query,
                FiltersApplied = CreateFiltersAppliedDto(request)
            };

            // Add suggestions for empty results
            if (totalCount == 0 && !string.IsNullOrEmpty(request.Query))
            {
                response.Suggestions = await GetSearchSuggestionsAsync(userId, request.Query, 3);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching messages for user {UserId}", userId);
            return new SearchMessageResponse
            {
                Success = false,
                Error = "An error occurred while searching messages",
                ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetSearchSuggestionsAsync(string userId, string query, int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return new List<string>();
            }

            // Get suggestions from search history
            var historySuggestions = await _context.SearchHistory
                .Where(h => h.UserId == userId && h.SearchQuery.Contains(query) && h.SearchQuery != query)
                .GroupBy(h => h.SearchQuery)
                .Select(g => new { Query = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .Select(x => x.Query)
                .ToListAsync();

            // Get suggestions from message content
            var contentSuggestions = await _context.Messages
                .Where(m => (m.SenderId == userId || m.Conversation.Participants.Any(p => p.UserId == userId)) &&
                           m.Content.Contains(query))
                .Select(m => m.Content)
                .Take(100) // Limit for processing
                .ToListAsync();

            // Extract relevant phrases from content
            var extractedSuggestions = ExtractQuerySuggestions(contentSuggestions, query, limit - historySuggestions.Count);

            var allSuggestions = historySuggestions.Concat(extractedSuggestions).Distinct().Take(limit).ToList();
            
            return allSuggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions for user {UserId}", userId);
            return new List<string>();
        }
    }

    /// <inheritdoc />
    public async Task<SavedSearchResponse> SaveSearchAsync(string userId, CreateSavedSearchRequest request)
    {
        try
        {
            // Check for duplicate name
            var existing = await _context.MessageSearches
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Name == request.Name && s.IsActive);

            if (existing != null)
            {
                return new SavedSearchResponse
                {
                    Success = false,
                    Message = "A saved search with this name already exists"
                };
            }

            var savedSearch = new MessageSearch
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = request.Name,
                Query = request.SearchCriteria.Query,
                ConversationId = request.SearchCriteria.ConversationId,
                SenderId = request.SearchCriteria.SenderId,
                MessageType = request.SearchCriteria.MessageType?.ToString(),
                DateFrom = request.SearchCriteria.DateRange == SearchDateRange.Custom ? request.SearchCriteria.DateFrom : null,
                DateTo = request.SearchCriteria.DateRange == SearchDateRange.Custom ? request.SearchCriteria.DateTo : null,
                IncludeAttachments = request.SearchCriteria.IncludeAttachments,
                IncludeVoiceMessages = request.SearchCriteria.IncludeVoiceMessages,
                IncludeEncryptedMessages = request.SearchCriteria.IncludeEncryptedMessages,
                Tags = request.SearchCriteria.Tags,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.MessageSearches.Add(savedSearch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved search created for user {UserId}: {SearchName}", userId, request.Name);

            return new SavedSearchResponse
            {
                Success = true,
                SavedSearch = MapToSavedSearchDto(savedSearch),
                Message = "Search saved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving search for user {UserId}", userId);
            return new SavedSearchResponse
            {
                Success = false,
                Message = "An error occurred while saving the search"
            };
        }
    }

    /// <inheritdoc />
    public async Task<SavedSearchResponse> UpdateSavedSearchAsync(string userId, string searchId, UpdateSavedSearchRequest request)
    {
        try
        {
            var savedSearch = await _context.MessageSearches
                .FirstOrDefaultAsync(s => s.Id == searchId && s.UserId == userId);

            if (savedSearch == null)
            {
                return new SavedSearchResponse
                {
                    Success = false,
                    Message = "Saved search not found"
                };
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Name))
            {
                // Check for duplicate name
                var existing = await _context.MessageSearches
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.Name == request.Name && 
                                            s.Id != searchId && s.IsActive);

                if (existing != null)
                {
                    return new SavedSearchResponse
                    {
                        Success = false,
                        Message = "A saved search with this name already exists"
                    };
                }

                savedSearch.Name = request.Name;
            }

            if (request.SearchCriteria != null)
            {
                savedSearch.Query = request.SearchCriteria.Query;
                savedSearch.ConversationId = request.SearchCriteria.ConversationId;
                savedSearch.SenderId = request.SearchCriteria.SenderId;
                savedSearch.MessageType = request.SearchCriteria.MessageType?.ToString();
                savedSearch.DateFrom = request.SearchCriteria.DateRange == SearchDateRange.Custom ? request.SearchCriteria.DateFrom : null;
                savedSearch.DateTo = request.SearchCriteria.DateRange == SearchDateRange.Custom ? request.SearchCriteria.DateTo : null;
                savedSearch.IncludeAttachments = request.SearchCriteria.IncludeAttachments;
                savedSearch.IncludeVoiceMessages = request.SearchCriteria.IncludeVoiceMessages;
                savedSearch.IncludeEncryptedMessages = request.SearchCriteria.IncludeEncryptedMessages;
                savedSearch.Tags = request.SearchCriteria.Tags;
            }

            if (request.IsPinned.HasValue)
            {
                savedSearch.IsPinned = request.IsPinned.Value;
            }

            savedSearch.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved search updated for user {UserId}: {SearchId}", userId, searchId);

            return new SavedSearchResponse
            {
                Success = true,
                SavedSearch = MapToSavedSearchDto(savedSearch),
                Message = "Search updated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saved search {SearchId} for user {UserId}", searchId, userId);
            return new SavedSearchResponse
            {
                Success = false,
                Message = "An error occurred while updating the search"
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteSavedSearchAsync(string userId, string searchId)
    {
        try
        {
            var savedSearch = await _context.MessageSearches
                .FirstOrDefaultAsync(s => s.Id == searchId && s.UserId == userId);

            if (savedSearch == null)
            {
                return false;
            }

            // Soft delete
            savedSearch.IsActive = false;
            savedSearch.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved search deleted for user {UserId}: {SearchId}", userId, searchId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search {SearchId} for user {UserId}", searchId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<SavedSearchDto>> GetSavedSearchesAsync(string userId, bool includeInactive = false)
    {
        try
        {
            var query = _context.MessageSearches
                .Where(s => s.UserId == userId);

            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive);
            }

            var savedSearches = await query
                .OrderByDescending(s => s.IsPinned)
                .ThenByDescending(s => s.UsageCount)
                .ThenByDescending(s => s.UpdatedAt)
                .ToListAsync();

            return savedSearches.Select(MapToSavedSearchDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved searches for user {UserId}", userId);
            return new List<SavedSearchDto>();
        }
    }

    /// <inheritdoc />
    public async Task<SavedSearchDto?> GetSavedSearchAsync(string userId, string searchId)
    {
        try
        {
            var savedSearch = await _context.MessageSearches
                .FirstOrDefaultAsync(s => s.Id == searchId && s.UserId == userId && s.IsActive);

            return savedSearch != null ? MapToSavedSearchDto(savedSearch) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved search {SearchId} for user {UserId}", searchId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<SearchMessageResponse> ExecuteSavedSearchAsync(string userId, string searchId, int skip = 0, int take = 20)
    {
        try
        {
            var savedSearch = await _context.MessageSearches
                .FirstOrDefaultAsync(s => s.Id == searchId && s.UserId == userId && s.IsActive);

            if (savedSearch == null)
            {
                return new SearchMessageResponse
                {
                    Success = false,
                    Error = "Saved search not found"
                };
            }

            // Convert saved search to request
            var request = new SearchMessageRequest
            {
                Query = savedSearch.Query,
                ConversationId = savedSearch.ConversationId,
                SenderId = savedSearch.SenderId,
                MessageType = !string.IsNullOrEmpty(savedSearch.MessageType) ? 
                    EnumConverter.ToMessageType(savedSearch.MessageType) : null,
                DateRange = (savedSearch.DateFrom.HasValue || savedSearch.DateTo.HasValue) ? 
                    SearchDateRange.Custom : SearchDateRange.AllTime,
                DateFrom = savedSearch.DateFrom,
                DateTo = savedSearch.DateTo,
                IncludeAttachments = savedSearch.IncludeAttachments,
                IncludeVoiceMessages = savedSearch.IncludeVoiceMessages,
                IncludeEncryptedMessages = savedSearch.IncludeEncryptedMessages,
                Tags = savedSearch.Tags,
                Skip = skip,
                Take = take
            };

            // Update usage statistics
            savedSearch.UsageCount++;
            savedSearch.LastUsedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return await SearchMessagesAsync(userId, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing saved search {SearchId} for user {UserId}", searchId, userId);
            return new SearchMessageResponse
            {
                Success = false,
                Error = "An error occurred while executing the saved search"
            };
        }
    }

    /// <inheritdoc />
    public async Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            var searchHistory = await _context.SearchHistory
                .Where(h => h.UserId == userId && h.CreatedAt >= fromDate && h.CreatedAt <= toDate)
                .ToListAsync();

            if (!searchHistory.Any())
            {
                return new SearchAnalyticsDto
                {
                    UserId = userId,
                    FromDate = fromDate.Value,
                    ToDate = toDate.Value
                };
            }

            var totalSearches = searchHistory.Count;
            var uniqueQueries = searchHistory.Select(h => h.SearchQuery.ToLowerInvariant()).Distinct().Count();
            var averageResults = searchHistory.Average(h => h.ResultCount);
            var averageExecutionTime = searchHistory.Average(h => h.ExecutionTimeMs);

            var hourGroups = searchHistory.GroupBy(h => h.CreatedAt.Hour);
            var mostActiveHour = hourGroups.OrderByDescending(g => g.Count()).First().Key;

            var dayGroups = searchHistory.GroupBy(h => (int)h.CreatedAt.DayOfWeek);
            var mostActiveDayOfWeek = dayGroups.OrderByDescending(g => g.Count()).First().Key;

            var interactionRate = (double)searchHistory.Count(h => h.HasInteraction) / totalSearches * 100;

            var topQueries = searchHistory
                .GroupBy(h => h.SearchQuery.ToLowerInvariant())
                .Select(g => new PopularQueryDto
                {
                    Query = g.First().SearchQuery,
                    Count = g.Count(),
                    LastUsed = g.Max(h => h.CreatedAt),
                    AverageResults = g.Average(h => h.ResultCount)
                })
                .OrderByDescending(q => q.Count)
                .Take(10)
                .ToList();

            var volumeByDay = searchHistory
                .GroupBy(h => DateOnly.FromDateTime(h.CreatedAt.Date))
                .Select(g => new SearchVolumeByDateDto
                {
                    Date = g.Key,
                    SearchCount = g.Count(),
                    UniqueQueries = g.Select(h => h.SearchQuery.ToLowerInvariant()).Distinct().Count()
                })
                .OrderBy(v => v.Date)
                .ToList();

            return new SearchAnalyticsDto
            {
                UserId = userId,
                TotalSearches = totalSearches,
                UniqueQueries = uniqueQueries,
                AverageResults = averageResults,
                AverageExecutionTime = averageExecutionTime,
                MostActiveHour = mostActiveHour,
                MostActiveDayOfWeek = mostActiveDayOfWeek,
                InteractionRate = interactionRate,
                TopQueries = topQueries,
                VolumeByDay = volumeByDay,
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search analytics for user {UserId}", userId);
            return new SearchAnalyticsDto
            {
                UserId = userId,
                FromDate = fromDate ?? DateTimeOffset.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTimeOffset.UtcNow
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<PopularQueryDto>> GetPopularQueriesAsync(string userId, int limit = 10)
    {
        try
        {
            var thirtyDaysAgo = DateTimeOffset.UtcNow.AddDays(-30);

            var popularQueries = await _context.SearchHistory
                .Where(h => h.UserId == userId && h.CreatedAt >= thirtyDaysAgo)
                .GroupBy(h => h.SearchQuery.ToLowerInvariant())
                .Select(g => new PopularQueryDto
                {
                    Query = g.First().SearchQuery,
                    Count = g.Count(),
                    LastUsed = g.Max(h => h.CreatedAt),
                    AverageResults = g.Average(h => h.ResultCount)
                })
                .OrderByDescending(q => q.Count)
                .Take(limit)
                .ToListAsync();

            return popularQueries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular queries for user {UserId}", userId);
            return new List<PopularQueryDto>();
        }
    }

    /// <inheritdoc />
    public async Task<bool> IndexMessagesAsync(List<string>? messageIds = null)
    {
        try
        {
            // This would typically integrate with a search engine like Elasticsearch
            // For now, we'll simulate indexing success
            _logger.LogInformation("Indexing messages: {MessageIds}", 
                messageIds?.Count.ToString() ?? "all messages");

            // In a real implementation, this would:
            // 1. Connect to search index (Elasticsearch, Azure Search, etc.)
            // 2. Transform message data for indexing
            // 3. Update/create index entries
            // 4. Handle failures and retries

            await Task.Delay(100); // Simulate processing time
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing messages");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<int> ClearSearchHistoryAsync(string userId, int? olderThanDays = null)
    {
        try
        {
            var query = _context.SearchHistory.Where(h => h.UserId == userId);

            if (olderThanDays.HasValue)
            {
                var cutoffDate = DateTimeOffset.UtcNow.AddDays(-olderThanDays.Value);
                query = query.Where(h => h.CreatedAt < cutoffDate);
            }

            var entriesToDelete = await query.ToListAsync();
            var count = entriesToDelete.Count;

            _context.SearchHistory.RemoveRange(entriesToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleared {Count} search history entries for user {UserId}", count, userId);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing search history for user {UserId}", userId);
            return 0;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Build base query for messages accessible to the user
    /// </summary>
    private IQueryable<Message> BuildBaseMessageQuery(string userId)
    {
        return _context.Messages
            .Include(m => m.Conversation)
            .Include(m => m.Sender)
            .Include(m => m.Attachments)
            .Where(m => m.SenderId == userId || m.Conversation.Participants.Any(p => p.UserId == userId))
            .Where(m => !m.IsDeleted);
    }

    /// <summary>
    /// Apply search filters to the query
    /// </summary>
    private IQueryable<Message> ApplySearchFilters(IQueryable<Message> query, SearchMessageRequest request)
    {
        // Text search
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerm = request.Query.Trim();
            if (request.ContentOnly)
            {
                query = query.Where(m => m.Content.Contains(searchTerm));
            }
            else
            {
                query = query.Where(m => 
                    m.Content.Contains(searchTerm) ||
                    m.Sender.DisplayName!.Contains(searchTerm) ||
                    m.Sender.UserName!.Contains(searchTerm) ||
                    m.Conversation.Name!.Contains(searchTerm));
            }
        }

        // Conversation filter
        if (!string.IsNullOrEmpty(request.ConversationId))
        {
            query = query.Where(m => m.ConversationId == request.ConversationId);
        }

        // Sender filter
        if (!string.IsNullOrEmpty(request.SenderId))
        {
            query = query.Where(m => m.SenderId == request.SenderId);
        }

        // Message type filter
        if (request.MessageType.HasValue)
        {
            var messageTypeString = EnumConverter.ToString(request.MessageType.Value);
            query = query.Where(m => m.MessageType == messageTypeString);
        }

        // Date range filter
        var (dateFrom, dateTo) = GetDateRange(request.DateRange, request.DateFrom, request.DateTo);
        if (dateFrom.HasValue)
        {
            query = query.Where(m => m.SentAt >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(m => m.SentAt <= dateTo.Value);
        }

        // Attachment filter
        if (!request.IncludeAttachments)
        {
            query = query.Where(m => !m.Attachments.Any());
        }

        // Voice message filter
        if (!request.IncludeVoiceMessages)
        {
            query = query.Where(m => m.MessageType != EnumConverter.ToString(MessageType.Audio));
        }

        // Encrypted message filter
        if (!request.IncludeEncryptedMessages)
        {
            query = query.Where(m => !m.IsEncrypted);
        }

        // Tags filter
        if (!string.IsNullOrEmpty(request.Tags))
        {
            var tags = request.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(t => t.Trim().ToLowerInvariant());
            
            foreach (var tag in tags)
            {
                query = query.Where(m => m.Tags != null && m.Tags.ToLower().Contains(tag));
            }
        }

        return query;
    }

    /// <summary>
    /// Apply sorting to the query
    /// </summary>
    private IQueryable<Message> ApplySorting(IQueryable<Message> query, SearchSortBy sortBy)
    {
        return sortBy switch
        {
            SearchSortBy.DateAsc => query.OrderBy(m => m.SentAt),
            SearchSortBy.DateDesc => query.OrderByDescending(m => m.SentAt),
            SearchSortBy.SenderName => query.OrderBy(m => m.Sender.DisplayName ?? m.Sender.UserName),
            SearchSortBy.ConversationName => query.OrderBy(m => m.Conversation.Name),
            SearchSortBy.MessageType => query.OrderBy(m => m.MessageType),
            SearchSortBy.LengthAsc => query.OrderBy(m => m.Content.Length),
            SearchSortBy.LengthDesc => query.OrderByDescending(m => m.Content.Length),
            SearchSortBy.Relevance or _ => query.OrderByDescending(m => m.SentAt) // Default to date for now
        };
    }

    /// <summary>
    /// Get date range from filter options
    /// </summary>
    private (DateTimeOffset?, DateTimeOffset?) GetDateRange(SearchDateRange dateRange, DateTimeOffset? customFrom, DateTimeOffset? customTo)
    {
        var now = DateTimeOffset.UtcNow;

        return dateRange switch
        {
            SearchDateRange.Today => (now.Date, now.Date.AddDays(1)),
            SearchDateRange.Yesterday => (now.Date.AddDays(-1), now.Date),
            SearchDateRange.LastWeek => (now.AddDays(-7), null),
            SearchDateRange.LastMonth => (now.AddDays(-30), null),
            SearchDateRange.Last3Months => (now.AddDays(-90), null),
            SearchDateRange.LastYear => (now.AddDays(-365), null),
            SearchDateRange.Custom => (customFrom, customTo),
            SearchDateRange.AllTime or _ => (null, null)
        };
    }

    /// <summary>
    /// Calculate relevance score for search results
    /// </summary>
    private double CalculateRelevanceScore(string content, string? query, SearchSortBy sortBy)
    {
        if (string.IsNullOrEmpty(query) || sortBy != SearchSortBy.Relevance)
        {
            return 0;
        }

        var score = 0.0;
        var queryLower = query.ToLowerInvariant();
        var contentLower = content.ToLowerInvariant();

        // Exact match bonus
        if (contentLower.Contains(queryLower))
        {
            score += 50;
        }

        // Word match scoring
        var queryWords = queryLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var contentWords = contentLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var queryWord in queryWords)
        {
            var matches = contentWords.Count(w => w.Contains(queryWord));
            score += matches * 10;
        }

        // Penalize very long content
        if (content.Length > 1000)
        {
            score *= 0.8;
        }

        return Math.Min(score, 100);
    }

    /// <summary>
    /// Highlight matching terms in content
    /// </summary>
    private string HighlightMatches(string content, string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return content;
        }

        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var highlightedContent = content;

        foreach (var word in queryWords)
        {
            var pattern = Regex.Escape(word);
            highlightedContent = Regex.Replace(highlightedContent, pattern, 
                match => $"<mark>{match.Value}</mark>", 
                RegexOptions.IgnoreCase);
        }

        return highlightedContent;
    }

    /// <summary>
    /// Extract matched text snippets from content
    /// </summary>
    private List<string> ExtractMatchedSnippets(string content, string query)
    {
        var snippets = new List<string>();
        
        if (string.IsNullOrEmpty(query))
        {
            return snippets;
        }

        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var word in queryWords)
        {
            var index = content.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                var start = Math.Max(0, index - 50);
                var length = Math.Min(content.Length - start, 100);
                var snippet = content.Substring(start, length);
                
                if (start > 0) snippet = "..." + snippet;
                if (start + length < content.Length) snippet = snippet + "...";
                
                snippets.Add(snippet);
            }
        }

        return snippets.Distinct().Take(3).ToList();
    }

    /// <summary>
    /// Parse tags string into list
    /// </summary>
    private List<string> ParseTags(string? tags)
    {
        if (string.IsNullOrEmpty(tags))
        {
            return new List<string>();
        }

        return tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .Where(t => !string.IsNullOrEmpty(t))
                  .ToList();
    }

    /// <summary>
    /// Add context messages to search results
    /// </summary>
    private async Task AddContextMessages(List<MessageSearchResultDto> results, string userId)
    {
        foreach (var result in results)
        {
            var contextMessages = await _context.Messages
                .Where(m => m.ConversationId == result.ConversationId &&
                           (m.SenderId == userId || m.Conversation.Participants.Any(p => p.UserId == userId)) &&
                           !m.IsDeleted)
                .Where(m => m.SentAt >= result.SentAt.AddMinutes(-5) && 
                           m.SentAt <= result.SentAt.AddMinutes(5) &&
                           m.Id != result.MessageId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageContextDto
                {
                    MessageId = m.Id,
                    SenderName = m.Sender.DisplayName ?? m.Sender.UserName ?? "Unknown",
                    Content = m.Content.Length > 100 ? m.Content.Substring(0, 100) + "..." : m.Content,
                    SentAt = m.SentAt,
                    Position = m.SentAt < result.SentAt ? -1 : 1
                })
                .Take(4)
                .ToListAsync();

            result.Context = contextMessages;
        }
    }

    /// <summary>
    /// Record search in history for analytics
    /// </summary>
    private async Task RecordSearchHistoryAsync(string userId, SearchMessageRequest request, int resultCount, int executionTimeMs)
    {
        try
        {
            var history = new SearchHistory
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                SearchQuery = request.Query ?? "",
                SearchFilters = JsonSerializer.Serialize(request),
                ResultCount = resultCount,
                ExecutionTimeMs = executionTimeMs,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.SearchHistory.Add(history);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording search history for user {UserId}", userId);
            // Don't throw - this is non-critical
        }
    }

    /// <summary>
    /// Extract query suggestions from content
    /// </summary>
    private List<string> ExtractQuerySuggestions(List<string> contentList, string query, int limit)
    {
        var suggestions = new HashSet<string>();
        var queryLower = query.ToLowerInvariant();

        foreach (var content in contentList)
        {
            var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < words.Length - 1; i++)
            {
                var phrase = $"{words[i]} {words[i + 1]}".ToLowerInvariant();
                if (phrase.Contains(queryLower) && phrase.Length >= query.Length + 2)
                {
                    suggestions.Add(phrase);
                    if (suggestions.Count >= limit) break;
                }
            }
            
            if (suggestions.Count >= limit) break;
        }

        return suggestions.Take(limit).ToList();
    }

    /// <summary>
    /// Create filters applied DTO
    /// </summary>
    private SearchFiltersAppliedDto CreateFiltersAppliedDto(SearchMessageRequest request)
    {
        return new SearchFiltersAppliedDto
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            MessageType = request.MessageType?.ToString(),
            DateRange = request.DateRange.ToString(),
            Tags = ParseTags(request.Tags),
            SortBy = request.SortBy.ToString()
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private SavedSearchDto MapToSavedSearchDto(MessageSearch search)
    {
        return new SavedSearchDto
        {
            Id = search.Id,
            UserId = search.UserId,
            Name = search.Name,
            Query = search.Query,
            ConversationId = search.ConversationId,
            SenderId = search.SenderId,
            MessageType = search.MessageType,
            DateFrom = search.DateFrom,
            DateTo = search.DateTo,
            IncludeAttachments = search.IncludeAttachments,
            IncludeVoiceMessages = search.IncludeVoiceMessages,
            IncludeEncryptedMessages = search.IncludeEncryptedMessages,
            Tags = search.Tags,
            IsPinned = search.IsPinned,
            UsageCount = search.UsageCount,
            LastUsedAt = search.LastUsedAt,
            CreatedAt = search.CreatedAt,
            UpdatedAt = search.UpdatedAt
        };
    }

    #endregion
}