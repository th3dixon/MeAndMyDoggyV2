using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for location sharing functionality
/// </summary>
public class LocationSharingService : ILocationSharingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<LocationSharingService> _logger;

    /// <summary>
    /// Initialize the location sharing service
    /// </summary>
    public LocationSharingService(
        ApplicationDbContext context,
        IMessagingService messagingService,
        ILogger<LocationSharingService> logger)
    {
        _context = context;
        _messagingService = messagingService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ShareLocationResponse> ShareLocationAsync(string userId, ShareLocationRequest request)
    {
        try
        {
            // Validate user access to conversation
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId &&
                                        c.Participants.Any(p => p.UserId == userId));

            if (conversation == null)
            {
                return new ShareLocationResponse
                {
                    Success = false,
                    Error = "Conversation not found or access denied"
                };
            }

            // If sharing from bookmark, get bookmark details
            LocationBookmark? bookmark = null;
            if (!string.IsNullOrEmpty(request.BookmarkId))
            {
                bookmark = await _context.LocationBookmarks
                    .FirstOrDefaultAsync(b => b.Id == request.BookmarkId && b.UserId == userId && b.IsActive);

                if (bookmark == null)
                {
                    return new ShareLocationResponse
                    {
                        Success = false,
                        Error = "Location bookmark not found"
                    };
                }

                // Update bookmark usage
                bookmark.UsageCount++;
                bookmark.LastUsedAt = DateTimeOffset.UtcNow;
            }

            // Create message first
            var messageContent = request.MessageContent ?? "📍 Shared location";
            var messageRequest = new SendMessageRequest
            {
                ConversationId = request.ConversationId,
                Content = messageContent,
                MessageType = MessageType.Location
            };

            var message = await _messagingService.SendMessageAsync(
                messageRequest.ConversationId, 
                userId, 
                messageRequest.Content, 
                messageRequest.MessageType);
            var messageResponse = new { Success = true, Message = message };
            if (!messageResponse.Success || messageResponse.Message == null)
            {
                return new ShareLocationResponse
                {
                    Success = false,
                    Error = "Failed to create message for location share"
                };
            }

            // Create location share
            var locationShare = new LocationShare
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = messageResponse.Message.Id,
                UserId = userId,
                ConversationId = request.ConversationId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Accuracy = request.Accuracy,
                Altitude = request.Altitude,
                Label = request.Label,
                LocationType = EnumConverter.ToString(request.LocationType),
                IsLive = request.EnableLiveSharing,
                LiveExpiresAt = request.EnableLiveSharing && request.LiveSharingDurationMinutes.HasValue
                    ? DateTimeOffset.UtcNow.AddMinutes(request.LiveSharingDurationMinutes.Value)
                    : null,
                LiveUpdateIntervalSeconds = request.EnableLiveSharing ? request.LiveUpdateIntervalSeconds : null,
                IsActive = true,
                Visibility = EnumConverter.ToString(request.Visibility),
                SharedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Try to reverse geocode for address
            try
            {
                locationShare.Address = await ReverseGeocodeAsync(request.Latitude, request.Longitude);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to reverse geocode location for share {LocationShareId}", locationShare.Id);
            }

            // If using bookmark, copy some details
            if (bookmark != null)
            {
                locationShare.PlaceName = bookmark.PlaceName;
                locationShare.Address = locationShare.Address ?? bookmark.Address;
                locationShare.Label = locationShare.Label ?? bookmark.Name;
            }

            _context.LocationShares.Add(locationShare);
            await _context.SaveChangesAsync();

            var locationShareDto = await MapToLocationShareDto(locationShare);

            _logger.LogInformation("Location shared by user {UserId} in conversation {ConversationId}", userId, request.ConversationId);

            return new ShareLocationResponse
            {
                Success = true,
                LocationShare = locationShareDto,
                Message = messageResponse.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing location for user {UserId}", userId);
            return new ShareLocationResponse
            {
                Success = false,
                Error = "An error occurred while sharing the location"
            };
        }
    }

    /// <inheritdoc />
    public async Task<UpdateLocationResponse> UpdateLiveLocationAsync(string userId, UpdateLiveLocationRequest request)
    {
        try
        {
            var locationShare = await _context.LocationShares
                .FirstOrDefaultAsync(ls => ls.Id == request.LocationShareId &&
                                          ls.UserId == userId &&
                                          ls.IsLive &&
                                          ls.IsActive);

            if (locationShare == null)
            {
                return new UpdateLocationResponse
                {
                    Success = false,
                    Error = "Live location share not found or not active"
                };
            }

            // Check if live sharing has expired
            if (locationShare.LiveExpiresAt.HasValue && locationShare.LiveExpiresAt < DateTimeOffset.UtcNow)
            {
                locationShare.IsActive = false;
                await _context.SaveChangesAsync();

                return new UpdateLocationResponse
                {
                    Success = false,
                    Error = "Live location sharing has expired",
                    IsLiveActive = false
                };
            }

            // Create location update
            var locationUpdate = new LocationUpdate
            {
                Id = Guid.NewGuid().ToString(),
                LocationShareId = request.LocationShareId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Accuracy = request.Accuracy,
                Altitude = request.Altitude,
                Speed = request.Speed,
                Bearing = request.Bearing,
                BatteryLevel = request.BatteryLevel,
                LocationSource = request.LocationSource,
                CapturedAt = request.CapturedAt ?? DateTimeOffset.UtcNow,
                ReceivedAt = DateTimeOffset.UtcNow
            };

            // Update the main location share with latest coordinates
            locationShare.Latitude = request.Latitude;
            locationShare.Longitude = request.Longitude;
            locationShare.Accuracy = request.Accuracy;
            locationShare.Altitude = request.Altitude;
            locationShare.UpdatedAt = DateTimeOffset.UtcNow;

            _context.LocationUpdates.Add(locationUpdate);
            await _context.SaveChangesAsync();

            var locationUpdateDto = MapToLocationUpdateDto(locationUpdate);

            _logger.LogDebug("Live location updated for share {LocationShareId}", request.LocationShareId);

            return new UpdateLocationResponse
            {
                Success = true,
                LocationUpdate = locationUpdateDto,
                IsLiveActive = true,
                LiveExpiresAt = locationShare.LiveExpiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating live location for user {UserId}", userId);
            return new UpdateLocationResponse
            {
                Success = false,
                Error = "An error occurred while updating the live location"
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> StopLiveLocationAsync(string userId, string locationShareId)
    {
        try
        {
            var locationShare = await _context.LocationShares
                .FirstOrDefaultAsync(ls => ls.Id == locationShareId &&
                                          ls.UserId == userId &&
                                          ls.IsLive);

            if (locationShare == null)
            {
                return false;
            }

            locationShare.IsActive = false;
            locationShare.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Live location sharing stopped for share {LocationShareId}", locationShareId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping live location {LocationShareId} for user {UserId}", locationShareId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<LocationShareDto>> GetConversationLocationsAsync(string userId, string conversationId, bool includeExpired = false, int limit = 50)
    {
        try
        {
            // Verify user access to conversation
            var hasAccess = await _context.Conversations
                .Where(c => c.Id == conversationId)
                .SelectMany(c => c.Participants)
                .AnyAsync(p => p.UserId == userId);

            if (!hasAccess)
            {
                return new List<LocationShareDto>();
            }

            var query = _context.LocationShares
                .Include(ls => ls.User)
                .Include(ls => ls.LocationUpdates.OrderByDescending(lu => lu.ReceivedAt).Take(3))
                .Where(ls => ls.ConversationId == conversationId);

            if (!includeExpired)
            {
                query = query.Where(ls => ls.IsActive &&
                                         (!ls.LiveExpiresAt.HasValue || ls.LiveExpiresAt > DateTimeOffset.UtcNow));
            }

            var locationShares = await query
                .OrderByDescending(ls => ls.SharedAt)
                .Take(limit)
                .ToListAsync();

            var locationShareDtos = new List<LocationShareDto>();
            foreach (var locationShare in locationShares)
            {
                locationShareDtos.Add(await MapToLocationShareDto(locationShare));
            }

            return locationShareDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation locations for user {UserId}", userId);
            return new List<LocationShareDto>();
        }
    }

    /// <inheritdoc />
    public async Task<LocationShareDto?> GetLocationShareAsync(string userId, string locationShareId)
    {
        try
        {
            var locationShare = await _context.LocationShares
                .Include(ls => ls.User)
                .Include(ls => ls.Conversation)
                .ThenInclude(c => c.Participants)
                .Include(ls => ls.LocationUpdates.OrderByDescending(lu => lu.ReceivedAt).Take(5))
                .FirstOrDefaultAsync(ls => ls.Id == locationShareId);

            if (locationShare == null)
            {
                return null;
            }

            // Check user access
            var hasAccess = locationShare.UserId == userId ||
                           locationShare.Conversation.Participants.Any(p => p.UserId == userId);

            if (!hasAccess)
            {
                return null;
            }

            return await MapToLocationShareDto(locationShare);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location share {LocationShareId} for user {UserId}", locationShareId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<LocationUpdateDto>> GetLocationUpdatesAsync(string userId, string locationShareId, DateTimeOffset? since = null, int limit = 20)
    {
        try
        {
            // Verify user access to location share
            var locationShare = await _context.LocationShares
                .Include(ls => ls.Conversation)
                .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(ls => ls.Id == locationShareId);

            if (locationShare == null)
            {
                return new List<LocationUpdateDto>();
            }

            var hasAccess = locationShare.UserId == userId ||
                           locationShare.Conversation.Participants.Any(p => p.UserId == userId);

            if (!hasAccess)
            {
                return new List<LocationUpdateDto>();
            }

            var query = _context.LocationUpdates
                .Where(lu => lu.LocationShareId == locationShareId);

            if (since.HasValue)
            {
                query = query.Where(lu => lu.ReceivedAt > since);
            }

            var updates = await query
                .OrderByDescending(lu => lu.ReceivedAt)
                .Take(limit)
                .ToListAsync();

            return updates.Select(MapToLocationUpdateDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location updates for share {LocationShareId}", locationShareId);
            return new List<LocationUpdateDto>();
        }
    }

    /// <inheritdoc />
    public async Task<LocationBookmarkResponse> CreateBookmarkAsync(string userId, CreateLocationBookmarkRequest request)
    {
        try
        {
            // Check for duplicate name
            var existingBookmark = await _context.LocationBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Name == request.Name && b.IsActive);

            if (existingBookmark != null)
            {
                return new LocationBookmarkResponse
                {
                    Success = false,
                    Error = "A bookmark with this name already exists"
                };
            }

            var bookmark = new LocationBookmark
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                PlaceName = request.PlaceName,
                Category = EnumConverter.ToString(request.Category),
                Icon = request.Icon,
                Color = request.Color,
                IsPrivate = request.IsPrivate,
                IsActive = true,
                UsageCount = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Try to reverse geocode if address not provided
            if (string.IsNullOrEmpty(bookmark.Address))
            {
                try
                {
                    bookmark.Address = await ReverseGeocodeAsync(request.Latitude, request.Longitude);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to reverse geocode bookmark location");
                }
            }

            _context.LocationBookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            var bookmarkDto = MapToLocationBookmarkDto(bookmark);

            _logger.LogInformation("Location bookmark created by user {UserId}: {BookmarkName}", userId, request.Name);

            return new LocationBookmarkResponse
            {
                Success = true,
                Bookmark = bookmarkDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating location bookmark for user {UserId}", userId);
            return new LocationBookmarkResponse
            {
                Success = false,
                Error = "An error occurred while creating the bookmark"
            };
        }
    }

    /// <inheritdoc />
    public async Task<LocationBookmarkResponse> UpdateBookmarkAsync(string userId, string bookmarkId, UpdateLocationBookmarkRequest request)
    {
        try
        {
            var bookmark = await _context.LocationBookmarks
                .FirstOrDefaultAsync(b => b.Id == bookmarkId && b.UserId == userId);

            if (bookmark == null)
            {
                return new LocationBookmarkResponse
                {
                    Success = false,
                    Error = "Location bookmark not found"
                };
            }

            // Check for duplicate name if name is being changed
            if (!string.IsNullOrEmpty(request.Name) && request.Name != bookmark.Name)
            {
                var existingBookmark = await _context.LocationBookmarks
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.Name == request.Name && 
                                            b.Id != bookmarkId && b.IsActive);

                if (existingBookmark != null)
                {
                    return new LocationBookmarkResponse
                    {
                        Success = false,
                        Error = "A bookmark with this name already exists"
                    };
                }

                bookmark.Name = request.Name;
            }

            // Update other fields if provided
            if (!string.IsNullOrEmpty(request.Description))
                bookmark.Description = request.Description;
            
            if (request.Latitude.HasValue)
                bookmark.Latitude = request.Latitude.Value;
            
            if (request.Longitude.HasValue)
                bookmark.Longitude = request.Longitude.Value;
            
            if (!string.IsNullOrEmpty(request.Address))
                bookmark.Address = request.Address;
            
            if (!string.IsNullOrEmpty(request.PlaceName))
                bookmark.PlaceName = request.PlaceName;
            
            if (request.Category.HasValue)
                bookmark.Category = EnumConverter.ToString(request.Category.Value);
            
            if (!string.IsNullOrEmpty(request.Icon))
                bookmark.Icon = request.Icon;
            
            if (!string.IsNullOrEmpty(request.Color))
                bookmark.Color = request.Color;
            
            if (request.IsPrivate.HasValue)
                bookmark.IsPrivate = request.IsPrivate.Value;

            bookmark.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var bookmarkDto = MapToLocationBookmarkDto(bookmark);

            _logger.LogInformation("Location bookmark {BookmarkId} updated by user {UserId}", bookmarkId, userId);

            return new LocationBookmarkResponse
            {
                Success = true,
                Bookmark = bookmarkDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location bookmark {BookmarkId} for user {UserId}", bookmarkId, userId);
            return new LocationBookmarkResponse
            {
                Success = false,
                Error = "An error occurred while updating the bookmark"
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBookmarkAsync(string userId, string bookmarkId)
    {
        try
        {
            var bookmark = await _context.LocationBookmarks
                .FirstOrDefaultAsync(b => b.Id == bookmarkId && b.UserId == userId);

            if (bookmark == null)
            {
                return false;
            }

            // Soft delete
            bookmark.IsActive = false;
            bookmark.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Location bookmark {BookmarkId} deleted by user {UserId}", bookmarkId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting location bookmark {BookmarkId} for user {UserId}", bookmarkId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<LocationBookmarkDto>> GetUserBookmarksAsync(string userId, LocationCategory? category = null, bool includeInactive = false)
    {
        try
        {
            var query = _context.LocationBookmarks
                .Where(b => b.UserId == userId);

            if (!includeInactive)
            {
                query = query.Where(b => b.IsActive);
            }

            if (category.HasValue)
            {
                var categoryString = EnumConverter.ToString(category.Value);
                query = query.Where(b => b.Category == categoryString);
            }

            var bookmarks = await query
                .OrderByDescending(b => b.UsageCount)
                .ThenByDescending(b => b.UpdatedAt)
                .ToListAsync();

            return bookmarks.Select(MapToLocationBookmarkDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookmarks for user {UserId}", userId);
            return new List<LocationBookmarkDto>();
        }
    }

    /// <inheritdoc />
    public async Task<LocationBookmarkDto?> GetBookmarkAsync(string userId, string bookmarkId)
    {
        try
        {
            var bookmark = await _context.LocationBookmarks
                .FirstOrDefaultAsync(b => b.Id == bookmarkId && b.UserId == userId && b.IsActive);

            return bookmark != null ? MapToLocationBookmarkDto(bookmark) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookmark {BookmarkId} for user {UserId}", bookmarkId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<LocationBookmarkDto>> SearchNearbyLocationsAsync(string userId, SearchNearbyRequest request)
    {
        try
        {
            var query = _context.LocationBookmarks.Where(b => b.IsActive);

            // Filter by privacy setting
            if (request.IncludePrivate && request.IncludePublic)
            {
                query = query.Where(b => b.UserId == userId || !b.IsPrivate);
            }
            else if (request.IncludePrivate)
            {
                query = query.Where(b => b.UserId == userId);
            }
            else if (request.IncludePublic)
            {
                query = query.Where(b => !b.IsPrivate);
            }

            // Category filter
            if (request.Category.HasValue)
            {
                var categoryString = EnumConverter.ToString(request.Category.Value);
                query = query.Where(b => b.Category == categoryString);
            }

            // Text search
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(b => b.Name.Contains(request.Query) ||
                                        (b.Description != null && b.Description.Contains(request.Query)) ||
                                        (b.Address != null && b.Address.Contains(request.Query)) ||
                                        (b.PlaceName != null && b.PlaceName.Contains(request.Query)));
            }

            var bookmarks = await query.ToListAsync();

            // Calculate distances and filter by radius
            var nearbyBookmarks = bookmarks
                .Select(b =>
                {
                    var distance = CalculateDistance(request.Latitude, request.Longitude, b.Latitude, b.Longitude);
                    var dto = MapToLocationBookmarkDto(b);
                    dto.DistanceFromUser = distance;
                    return dto;
                })
                .Where(dto => dto.DistanceFromUser <= request.RadiusMeters)
                .OrderBy(dto => dto.DistanceFromUser)
                .Take(request.Limit)
                .ToList();

            return nearbyBookmarks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching nearby locations for user {UserId}", userId);
            return new List<LocationBookmarkDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<LocationBookmarkDto>> GetPopularLocationsAsync(string userId, LocationCategory? category = null, int limit = 10)
    {
        try
        {
            var query = _context.LocationBookmarks
                .Where(b => (b.UserId == userId || !b.IsPrivate) && b.IsActive);

            if (category.HasValue)
            {
                var categoryString = EnumConverter.ToString(category.Value);
                query = query.Where(b => b.Category == categoryString);
            }

            var bookmarks = await query
                .OrderByDescending(b => b.UsageCount)
                .ThenByDescending(b => b.LastUsedAt)
                .Take(limit)
                .ToListAsync();

            return bookmarks.Select(MapToLocationBookmarkDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular locations for user {UserId}", userId);
            return new List<LocationBookmarkDto>();
        }
    }

    /// <inheritdoc />
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula for calculating distance between two points on Earth
        const double R = 6371000; // Earth's radius in meters

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = R * c;

        return distance;
    }

    /// <inheritdoc />
    public async Task<string?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        // This would typically integrate with a geocoding service like Google Maps, MapBox, or OpenStreetMap
        // For now, we'll return a placeholder implementation
        try
        {
            await Task.Delay(100); // Simulate API call
            return $"Location near {latitude:F4}, {longitude:F4}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverse geocoding coordinates {Lat}, {Lon}", latitude, longitude);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<(double Latitude, double Longitude)?> GeocodeAddressAsync(string address)
    {
        // This would typically integrate with a geocoding service
        // For now, we'll return a placeholder implementation
        try
        {
            await Task.Delay(100); // Simulate API call
            
            // Return null to indicate address not found
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", address);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredLocationSharesAsync()
    {
        try
        {
            var expiredShares = await _context.LocationShares
                .Where(ls => ls.IsLive && ls.IsActive && 
                           ls.LiveExpiresAt.HasValue && 
                           ls.LiveExpiresAt < DateTimeOffset.UtcNow)
                .ToListAsync();

            foreach (var share in expiredShares)
            {
                share.IsActive = false;
                share.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (expiredShares.Any())
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} expired location shares", expiredShares.Count);
            }

            return expiredShares.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired location shares");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<LocationSharingStatsDto> GetLocationSharingStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            var locationShares = await _context.LocationShares
                .Where(ls => ls.UserId == userId && ls.SharedAt >= fromDate && ls.SharedAt <= toDate)
                .ToListAsync();

            var bookmarks = await _context.LocationBookmarks
                .Where(b => b.UserId == userId && b.IsActive)
                .ToListAsync();

            var activeLiveShares = await _context.LocationShares
                .CountAsync(ls => ls.UserId == userId && ls.IsLive && ls.IsActive &&
                                 (!ls.LiveExpiresAt.HasValue || ls.LiveExpiresAt > DateTimeOffset.UtcNow));

            var stats = new LocationSharingStatsDto
            {
                UserId = userId,
                TotalLocationsShared = locationShares.Count,
                StaticShares = locationShares.Count(ls => EnumConverter.ToLocationShareType(ls.LocationType) == LocationShareType.Static),
                LiveShares = locationShares.Count(ls => EnumConverter.ToLocationShareType(ls.LocationType) == LocationShareType.Live),
                BookmarkShares = locationShares.Count(ls => EnumConverter.ToLocationShareType(ls.LocationType) == LocationShareType.Bookmark),
                TotalBookmarks = bookmarks.Count,
                ActiveLiveShares = activeLiveShares,
                ConversationsWithLocations = locationShares.Select(ls => ls.ConversationId).Distinct().Count(),
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };

            // Calculate average live sharing duration
            var liveShares = locationShares.Where(ls => ls.IsLive && ls.LiveExpiresAt.HasValue).ToList();
            if (liveShares.Any())
            {
                var durations = liveShares.Select(ls => (ls.LiveExpiresAt!.Value - ls.SharedAt).TotalMinutes);
                stats.AverageLiveSharingDuration = durations.Average();
            }

            // Most active sharing hour
            if (locationShares.Any())
            {
                stats.MostActiveSharingHour = locationShares
                    .GroupBy(ls => ls.SharedAt.Hour)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
            }

            // Daily stats
            stats.DailyStats = locationShares
                .GroupBy(ls => DateOnly.FromDateTime(ls.SharedAt.Date))
                .Select(g => new LocationSharingDayStats
                {
                    Date = g.Key,
                    LocationsShared = g.Count(),
                    LiveSharesStarted = g.Count(ls => ls.IsLive),
                    TotalLiveSharingMinutes = (int)g.Where(ls => ls.IsLive && ls.LiveExpiresAt.HasValue)
                                                   .Sum(ls => (ls.LiveExpiresAt!.Value - ls.SharedAt).TotalMinutes)
                })
                .OrderBy(ds => ds.Date)
                .ToList();

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location sharing stats for user {UserId}", userId);
            return new LocationSharingStatsDto
            {
                UserId = userId,
                FromDate = fromDate ?? DateTimeOffset.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTimeOffset.UtcNow
            };
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Convert degrees to radians
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private async Task<LocationShareDto> MapToLocationShareDto(LocationShare locationShare)
    {
        return new LocationShareDto
        {
            Id = locationShare.Id,
            MessageId = locationShare.MessageId,
            UserId = locationShare.UserId,
            UserName = locationShare.User?.DisplayName ?? locationShare.User?.UserName ?? "Unknown",
            ConversationId = locationShare.ConversationId,
            Latitude = locationShare.Latitude,
            Longitude = locationShare.Longitude,
            Accuracy = locationShare.Accuracy,
            Altitude = locationShare.Altitude,
            Address = locationShare.Address,
            PlaceName = locationShare.PlaceName,
            Label = locationShare.Label,
            LocationType = EnumConverter.ToLocationShareType(locationShare.LocationType),
            IsLive = locationShare.IsLive,
            LiveExpiresAt = locationShare.LiveExpiresAt,
            LiveUpdateIntervalSeconds = locationShare.LiveUpdateIntervalSeconds,
            Visibility = EnumConverter.ToLocationVisibility(locationShare.Visibility),
            IsActive = locationShare.IsActive,
            SharedAt = locationShare.SharedAt,
            UpdatedAt = locationShare.UpdatedAt,
            RecentUpdates = locationShare.LocationUpdates?.Select(MapToLocationUpdateDto).ToList() ?? new List<LocationUpdateDto>()
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static LocationUpdateDto MapToLocationUpdateDto(LocationUpdate locationUpdate)
    {
        return new LocationUpdateDto
        {
            Id = locationUpdate.Id,
            LocationShareId = locationUpdate.LocationShareId,
            Latitude = locationUpdate.Latitude,
            Longitude = locationUpdate.Longitude,
            Accuracy = locationUpdate.Accuracy,
            Altitude = locationUpdate.Altitude,
            Speed = locationUpdate.Speed,
            Bearing = locationUpdate.Bearing,
            BatteryLevel = locationUpdate.BatteryLevel,
            LocationSource = locationUpdate.LocationSource,
            CapturedAt = locationUpdate.CapturedAt,
            ReceivedAt = locationUpdate.ReceivedAt
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static LocationBookmarkDto MapToLocationBookmarkDto(LocationBookmark bookmark)
    {
        return new LocationBookmarkDto
        {
            Id = bookmark.Id,
            UserId = bookmark.UserId,
            Name = bookmark.Name,
            Description = bookmark.Description,
            Latitude = bookmark.Latitude,
            Longitude = bookmark.Longitude,
            Address = bookmark.Address,
            PlaceName = bookmark.PlaceName,
            Category = EnumConverter.ToLocationCategory(bookmark.Category),
            Icon = bookmark.Icon,
            Color = bookmark.Color,
            IsPrivate = bookmark.IsPrivate,
            UsageCount = bookmark.UsageCount,
            LastUsedAt = bookmark.LastUsedAt,
            CreatedAt = bookmark.CreatedAt,
            UpdatedAt = bookmark.UpdatedAt
        };
    }

    #endregion
}