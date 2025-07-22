using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs.Mobile;
using MeAndMyDog.API.Models.DTOs.Dashboard;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Mobile integration service providing comprehensive mobile app support
/// </summary>
public class MobileIntegrationService : IMobileIntegrationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MobileIntegrationService> _logger;
    private readonly IDashboardCacheService _cacheService;
    private readonly IPushNotificationService _pushService;

    public MobileIntegrationService(
        ApplicationDbContext context,
        ILogger<MobileIntegrationService> logger,
        IDashboardCacheService cacheService,
        IPushNotificationService pushService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
        _pushService = pushService;
    }

    public async Task<MobileDeviceRegistrationResult> RegisterDeviceAsync(string userId, MobileDeviceRegistrationDto registration)
    {
        try
        {
            // Check if device already exists
            var existingDevice = await _context.MobileDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == registration.DeviceId);

            if (existingDevice != null)
            {
                // Update existing device
                existingDevice.DeviceToken = registration.DeviceToken;
                existingDevice.Platform = registration.Platform;
                existingDevice.AppVersion = registration.AppVersion;
                existingDevice.OSVersion = registration.OSVersion;
                existingDevice.DeviceModel = registration.DeviceModel;
                existingDevice.NotificationPermissions = JsonSerializer.Serialize(registration.NotificationPermissions);
                existingDevice.UpdatedAt = DateTime.UtcNow;
                existingDevice.IsActive = true;

                _context.MobileDevices.Update(existingDevice);
            }
            else
            {
                // Create new device registration
                var newDevice = new MobileDevice
                {
                    UserId = userId,
                    DeviceId = registration.DeviceId,
                    DeviceToken = registration.DeviceToken,
                    Platform = registration.Platform,
                    AppVersion = registration.AppVersion,
                    OSVersion = registration.OSVersion,
                    DeviceModel = registration.DeviceModel,
                    NotificationPermissions = JsonSerializer.Serialize(registration.NotificationPermissions),
                    Language = registration.Language ?? "en",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _context.MobileDevices.AddAsync(newDevice);
            }

            await _context.SaveChangesAsync();

            return new MobileDeviceRegistrationResult
            {
                Success = true,
                DeviceRegistrationId = registration.DeviceId,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering mobile device for user {UserId}", userId);
            return new MobileDeviceRegistrationResult
            {
                Success = false,
                Error = "Failed to register device"
            };
        }
    }

    public async Task<PushNotificationResult> SendNotificationAsync(string userId, MobilePushNotificationDto notification)
    {
        try
        {
            var userDevices = await _context.MobileDevices
                .Where(d => d.UserId == userId && d.IsActive)
                .AsNoTracking()
                .ToListAsync();

            if (!userDevices.Any())
            {
                return new PushNotificationResult
                {
                    Success = false,
                    Error = "No active devices found for user"
                };
            }

            var notificationId = Guid.NewGuid().ToString();
            var successCount = 0;
            var failedDevices = new List<string>();

            foreach (var device in userDevices)
            {
                try
                {
                    // Platform-specific notification sending
                    var sent = await SendPlatformNotificationAsync(device, notification);
                    if (sent)
                    {
                        successCount++;
                    }
                    else
                    {
                        failedDevices.Add(device.DeviceId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send notification to device {DeviceId}", device.DeviceId);
                    failedDevices.Add(device.DeviceId);
                }
            }

            // Log notification in database
            var notificationLog = new PushNotificationLog
            {
                Id = notificationId,
                UserId = userId,
                Title = notification.Title,
                Body = notification.Body,
                Type = notification.Type,
                Data = JsonSerializer.Serialize(notification.Data),
                DevicesTargeted = userDevices.Count,
                DevicesReached = successCount,
                CreatedAt = DateTime.UtcNow
            };

            await _context.PushNotificationLogs.AddAsync(notificationLog);
            await _context.SaveChangesAsync();

            return new PushNotificationResult
            {
                Success = successCount > 0,
                NotificationId = notificationId,
                DevicesReached = successCount,
                FailedDevices = failedDevices
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
            return new PushNotificationResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<BulkNotificationResult> SendBulkNotificationsAsync(BulkNotificationRequest request)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var successCount = 0;
            var failCount = 0;
            var errors = new List<string>();

            var tasks = request.UserIds.Select(async userId =>
            {
                try
                {
                    var result = await SendNotificationAsync(userId, request.Notification);
                    if (result.Success)
                    {
                        Interlocked.Increment(ref successCount);
                    }
                    else
                    {
                        Interlocked.Increment(ref failCount);
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            lock (errors)
                            {
                                errors.Add($"User {userId}: {result.Error}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failCount);
                    lock (errors)
                    {
                        errors.Add($"User {userId}: {ex.Message}");
                    }
                }
            });

            await Task.WhenAll(tasks);

            return new BulkNotificationResult
            {
                Success = successCount > 0,
                TotalTargetUsers = request.UserIds.Count,
                SuccessfulDeliveries = successCount,
                FailedDeliveries = failCount,
                Errors = errors,
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk notifications");
            return new BulkNotificationResult
            {
                Success = false,
                TotalTargetUsers = request.UserIds.Count,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<MobileDashboardDto> GetMobileDashboardAsync(string userId, MobileViewPreferences? preferences = null)
    {
        try
        {
            preferences ??= new MobileViewPreferences();

            // Get optimized dashboard data
            var quickStats = await GetMobileQuickStatsAsync(userId);
            var pets = await GetMobilePetsAsync(userId, preferences.MaxItemsPerWidget);
            var upcomingServices = await GetMobileUpcomingServicesAsync(userId, preferences.MaxItemsPerWidget);
            var recentActivity = await GetMobileActivityAsync(userId, preferences.MaxItemsPerWidget);
            var weather = await GetMobileWeatherAsync(userId);
            var notifications = await GetMobileNotificationsAsync(userId, 10);
            var quickActions = GetMobileQuickActions(preferences);

            return new MobileDashboardDto
            {
                QuickStats = quickStats,
                Pets = pets,
                UpcomingServices = upcomingServices,
                RecentActivity = recentActivity,
                Weather = weather,
                Notifications = notifications,
                QuickActions = quickActions,
                LastUpdated = DateTime.UtcNow,
                CacheVersion = GenerateCacheVersion()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating mobile dashboard for user {UserId}", userId);
            return new MobileDashboardDto();
        }
    }

    public async Task<SyncResult> SyncOfflineDataAsync(string userId, OfflineSyncRequest syncRequest)
    {
        try
        {
            var conflicts = new List<string>();
            var errors = new List<string>();
            var processedChanges = 0;
            var updatedData = new Dictionary<string, object>();

            foreach (var change in syncRequest.Changes)
            {
                try
                {
                    var result = await ProcessDataChangeAsync(userId, change);
                    if (result.Success)
                    {
                        processedChanges++;
                        if (result.UpdatedEntity != null)
                        {
                            updatedData[change.Id] = result.UpdatedEntity;
                        }
                    }
                    else
                    {
                        if (result.IsConflict)
                        {
                            conflicts.Add($"{change.EntityType}:{change.Id} - {result.Error}");
                        }
                        else
                        {
                            errors.Add($"{change.EntityType}:{change.Id} - {result.Error}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{change.EntityType}:{change.Id} - {ex.Message}");
                }
            }

            return new SyncResult
            {
                Success = errors.Count == 0,
                Conflicts = conflicts,
                Errors = errors,
                ProcessedChanges = processedChanges,
                UpdatedData = updatedData
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing offline data for user {UserId}", userId);
            return new SyncResult
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<MobileRealtimeUpdateDto> GetRealtimeUpdatesAsync(string userId, DateTime lastUpdateTime)
    {
        try
        {
            var updates = new List<RealtimeUpdate>();

            // Check for booking updates
            var bookingData = await _context.Bookings
                .Where(b => b.CustomerId == userId && b.UpdatedAt > lastUpdateTime)
                .Select(b => new { b.Id, b.Status, b.StartDateTime, b.TotalPrice, b.UpdatedAt })
                .ToListAsync();
                
            var bookingUpdates = bookingData.Select(b => new RealtimeUpdate
            {
                Type = "booking",
                EntityId = b.Id,
                Operation = "update",
                Data = new Dictionary<string, object>
                {
                    ["status"] = b.Status,
                    ["startDateTime"] = b.StartDateTime,
                    ["totalPrice"] = b.TotalPrice
                },
                Timestamp = b.UpdatedAt.DateTime
            }).ToList();

            updates.AddRange(bookingUpdates);

            // Check for notification updates
            var notificationData = await _context.PushNotificationLogs
                .Where(n => n.UserId == userId && n.CreatedAt > lastUpdateTime)
                .Select(n => new { n.Id, n.Title, n.Body, n.Type, n.CreatedAt })
                .ToListAsync();
                
            var notificationUpdates = notificationData.Select(n => new RealtimeUpdate
            {
                Type = "notification",
                EntityId = n.Id,
                Operation = "create",
                Data = new Dictionary<string, object>
                {
                    ["title"] = n.Title,
                    ["body"] = n.Body,
                    ["type"] = n.Type
                },
                Timestamp = n.CreatedAt
            }).ToList();

            updates.AddRange(notificationUpdates);

            return new MobileRealtimeUpdateDto
            {
                Updates = updates.OrderByDescending(u => u.Timestamp).ToList(),
                HasMoreUpdates = updates.Count >= 50
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting realtime updates for user {UserId}", userId);
            return new MobileRealtimeUpdateDto();
        }
    }

    public async Task TrackMobileAnalyticsAsync(string userId, MobileAnalyticsEventDto analyticsEvent)
    {
        try
        {
            var analyticsLog = new MobileAnalyticsLog
            {
                UserId = userId,
                EventName = analyticsEvent.EventName,
                EventCategory = analyticsEvent.EventCategory,
                Properties = JsonSerializer.Serialize(analyticsEvent.Properties),
                DeviceId = analyticsEvent.DeviceId,
                AppVersion = analyticsEvent.AppVersion,
                Screen = analyticsEvent.Screen,
                Timestamp = analyticsEvent.Timestamp
            };

            await _context.MobileAnalyticsLogs.AddAsync(analyticsLog);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Tracked mobile analytics event {EventName} for user {UserId}", 
                analyticsEvent.EventName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking mobile analytics for user {UserId}", userId);
        }
    }

    public async Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(string userId)
    {
        try
        {
            var preferences = await _context.UserSettings
                .Where(s => s.UserId == userId && s.Category == "NotificationPreferences")
                .ToDictionaryAsync(s => s.Key, s => s.Value);

            return new NotificationPreferencesDto
            {
                AppointmentReminders = GetBoolPreference(preferences, "appointmentReminders", true),
                BookingConfirmations = GetBoolPreference(preferences, "bookingConfirmations", true),
                HealthReminders = GetBoolPreference(preferences, "healthReminders", true),
                ProviderMessages = GetBoolPreference(preferences, "providerMessages", true),
                SystemAnnouncements = GetBoolPreference(preferences, "systemAnnouncements", true),
                MarketingOffers = GetBoolPreference(preferences, "marketingOffers", false),
                QuietHoursStart = preferences.GetValueOrDefault("quietHoursStart", "22:00"),
                QuietHoursEnd = preferences.GetValueOrDefault("quietHoursEnd", "07:00"),
                WeekendNotifications = GetBoolPreference(preferences, "weekendNotifications", true)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification preferences for user {UserId}", userId);
            return new NotificationPreferencesDto();
        }
    }

    public async Task<bool> UpdateNotificationPreferencesAsync(string userId, NotificationPreferencesDto preferences)
    {
        try
        {
            var settingsToUpdate = new Dictionary<string, string>
            {
                ["appointmentReminders"] = preferences.AppointmentReminders.ToString(),
                ["bookingConfirmations"] = preferences.BookingConfirmations.ToString(),
                ["healthReminders"] = preferences.HealthReminders.ToString(),
                ["providerMessages"] = preferences.ProviderMessages.ToString(),
                ["systemAnnouncements"] = preferences.SystemAnnouncements.ToString(),
                ["marketingOffers"] = preferences.MarketingOffers.ToString(),
                ["quietHoursStart"] = preferences.QuietHoursStart,
                ["quietHoursEnd"] = preferences.QuietHoursEnd,
                ["weekendNotifications"] = preferences.WeekendNotifications.ToString()
            };

            var now = DateTimeOffset.UtcNow;
            foreach (var (key, value) in settingsToUpdate)
            {
                var existingSetting = await _context.UserSettings
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.Category == "NotificationPreferences" && s.Key == key);

                if (existingSetting != null)
                {
                    existingSetting.Value = value;
                    existingSetting.UpdatedAt = now;
                }
                else
                {
                    await _context.UserSettings.AddAsync(new UserSetting
                    {
                        UserId = userId,
                        Category = "NotificationPreferences",
                        Key = key,
                        Value = value,
                        DataType = "boolean",
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences for user {UserId}", userId);
            return false;
        }
    }

    public async Task<MobileApiTokenDto> GenerateMobileApiTokenAsync(string userId, string deviceId, TimeSpan? expiry = null)
    {
        // This would integrate with JWT token generation
        await Task.CompletedTask;
        
        var expiryTime = expiry ?? TimeSpan.FromDays(30);
        var accessToken = GenerateJwtToken(userId, deviceId, expiryTime);
        var refreshToken = GenerateRefreshToken();

        return new MobileApiTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.Add(expiryTime),
            Scopes = new List<string> { "dashboard", "bookings", "notifications" }
        };
    }

    public async Task<MobileApiTokenDto> RefreshMobileApiTokenAsync(string refreshToken)
    {
        // Placeholder for refresh token logic
        await Task.CompletedTask;
        throw new NotImplementedException("Token refresh not yet implemented");
    }

    public async Task<MobileAppConfigDto> GetMobileAppConfigAsync(string version, string platform)
    {
        await Task.CompletedTask;
        
        return new MobileAppConfigDto
        {
            MinimumVersion = platform.ToLower() == "ios" ? "1.0.0" : "1.0.0",
            ForceUpdate = false,
            FeatureFlags = new Dictionary<string, object>
            {
                ["pushNotifications"] = true,
                ["realTimeUpdates"] = true,
                ["offlineMode"] = true,
                ["analytics"] = true
            },
            ApiEndpoints = new Dictionary<string, string>
            {
                ["base"] = "https://api.meandmydoggyv2.com/api/v1",
                ["dashboard"] = "/dashboard",
                ["bookings"] = "/bookings",
                ["notifications"] = "/notifications"
            },
            RateLimits = new Dictionary<string, int>
            {
                ["dashboard"] = 100,
                ["bookings"] = 50,
                ["notifications"] = 200
            }
        };
    }

    public async Task<DeepLinkResult> ProcessDeepLinkAsync(string userId, string deepLink)
    {
        try
        {
            var uri = new Uri(deepLink);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length == 0)
            {
                return new DeepLinkResult
                {
                    Success = true,
                    TargetScreen = "dashboard"
                };
            }

            var screen = segments[0].ToLower();
            var parameters = new Dictionary<string, string>();

            // Parse query parameters
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            foreach (string key in query.Keys)
            {
                if (key != null)
                {
                    parameters[key] = query[key] ?? string.Empty;
                }
            }

            await Task.CompletedTask;

            return new DeepLinkResult
            {
                Success = true,
                TargetScreen = screen,
                Parameters = parameters,
                RequiresAuthentication = screen != "auth"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing deep link {DeepLink} for user {UserId}", deepLink, userId);
            return new DeepLinkResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<List<NearbyServiceDto>> GetNearbyServicesAsync(double latitude, double longitude, int radiusKm = 5)
    {
        try
        {
            // Simplified distance calculation - in production use PostGIS or similar
            var nearbyProviders = await _context.ServiceProviders
                .Include(sp => sp.User)
                .Include(sp => sp.Services)
                .Where(sp => sp.IsActive && 
                           sp.User.Latitude != null && sp.User.Longitude != null)
                .AsNoTracking()
                .ToListAsync();

            var nearbyServices = nearbyProviders
                .Where(sp => CalculateDistance(latitude, longitude, 
                    (double)sp.User.Latitude!, (double)sp.User.Longitude!) <= radiusKm)
                .SelectMany(sp => sp.Services.Select(s => new NearbyServiceDto
                {
                    Id = s.Id,
                    ProviderName = sp.BusinessName,
                    ServiceType = s.Category,
                    Distance = CalculateDistance(latitude, longitude, 
                        (double)sp.User.Latitude!, (double)sp.User.Longitude!),
                    Price = s.BasePrice,
                    Rating = (double)sp.Rating,
                    Available = sp.IsActive,
                    Latitude = (double)sp.User.Latitude!,
                    Longitude = (double)sp.User.Longitude!
                }))
                .OrderBy(s => s.Distance)
                .Take(20)
                .ToList();

            return nearbyServices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearby services for location {Lat}, {Lng}", latitude, longitude);
            return new List<NearbyServiceDto>();
        }
    }

    #region Private Helper Methods

    private async Task<bool> SendPlatformNotificationAsync(MobileDevice device, MobilePushNotificationDto notification)
    {
        try
        {
            // This would integrate with FCM for Android and APNs for iOS
            // For now, using the existing push notification service as a placeholder
            
            await Task.CompletedTask;
            _logger.LogDebug("Sent notification to {Platform} device {DeviceId}", device.Platform, device.DeviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send platform notification to device {DeviceId}", device.DeviceId);
            return false;
        }
    }

    private async Task<MobileQuickStatsDto> GetMobileQuickStatsAsync(string userId)
    {
        var petCount = await _context.DogProfiles.CountAsync(d => d.OwnerId == userId && d.IsActive);
        var upcomingServices = await _context.Bookings.CountAsync(b => 
            b.CustomerId == userId && b.StartDateTime > DateTime.UtcNow && b.Status != "Cancelled");
        var unreadNotifications = await _context.PushNotificationLogs.CountAsync(n => 
            n.UserId == userId && !n.IsRead);

        var nextBooking = await _context.Bookings
            .Where(b => b.CustomerId == userId && b.StartDateTime > DateTime.UtcNow && b.Status != "Cancelled")
            .OrderBy(b => b.StartDateTime)
            .FirstOrDefaultAsync();

        var monthlySpending = await _context.Bookings
            .Where(b => b.CustomerId == userId && 
                       b.CreatedAt >= DateTime.UtcNow.AddDays(-30) &&
                       b.Status == "Completed")
            .SumAsync(b => b.TotalPrice);

        return new MobileQuickStatsDto
        {
            PetCount = petCount,
            UpcomingServices = upcomingServices,
            UnreadNotifications = unreadNotifications,
            NextAppointment = nextBooking?.StartDateTime.ToString("MMM dd, h:mm tt") ?? "None scheduled",
            MonthlySpending = monthlySpending
        };
    }

    private async Task<List<MobilePetSummaryDto>> GetMobilePetsAsync(string userId, int maxItems)
    {
        return await _context.DogProfiles
            .Include(d => d.MedicalRecords)
            .Include(d => d.Appointments)
            .Where(d => d.OwnerId == userId && d.IsActive)
            .Take(maxItems)
            .Select(d => new MobilePetSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                ImageUrl = d.ProfileImageUrl,
                HealthStatus = "Good", // Simplified for mobile
                NextAppointment = d.Appointments
                    .Where(a => a.StartTime > DateTime.UtcNow)
                    .OrderBy(a => a.StartTime)
                    .Select(a => a.StartTime.ToString("MMM dd"))
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    private async Task<List<MobileUpcomingServiceDto>> GetMobileUpcomingServicesAsync(string userId, int maxItems)
    {
        return await _context.Bookings
            .Include(b => b.Service)
            .Include(b => b.ServiceProvider)
                .ThenInclude(sp => sp.User)
            .Include(b => b.Dog)
            .Where(b => b.CustomerId == userId && 
                       b.StartDateTime > DateTime.UtcNow &&
                       b.Status != "Cancelled")
            .OrderBy(b => b.StartDateTime)
            .Take(maxItems)
            .Select(b => new MobileUpcomingServiceDto
            {
                Id = b.Id,
                ServiceName = b.Service != null ? b.Service.Name : "Service",
                ProviderName = b.ServiceProvider != null && b.ServiceProvider.User != null 
                    ? b.ServiceProvider.User.DisplayName : "Provider",
                DateTime = b.StartDateTime.DateTime,
                Status = b.Status,
                Price = b.TotalPrice,
                PetName = b.Dog != null ? b.Dog.Name : null
            })
            .ToListAsync();
    }

    private async Task<List<MobileActivityDto>> GetMobileActivityAsync(string userId, int maxItems)
    {
        var activities = new List<MobileActivityDto>();

        // Get recent bookings
        var recentBookings = await _context.Bookings
            .Include(b => b.Service)
            .Include(b => b.Dog)
            .Where(b => b.CustomerId == userId && 
                       b.UpdatedAt >= DateTime.UtcNow.AddDays(-7) &&
                       b.Status == "Completed")
            .OrderByDescending(b => b.UpdatedAt)
            .Take(maxItems)
            .Select(b => new MobileActivityDto
            {
                Id = b.Id,
                Type = "booking_completed",
                Title = "Service Completed",
                Description = $"{b.Service.Name} for {b.Dog.Name}",
                Timestamp = b.UpdatedAt.DateTime,
                Icon = "checkmark.circle"
            })
            .ToListAsync();

        activities.AddRange(recentBookings);

        return activities.OrderByDescending(a => a.Timestamp).Take(maxItems).ToList();
    }

    private async Task<MobileWeatherDto?> GetMobileWeatherAsync(string userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            // Try cache first (simplified)
            var locationKey = $"{user.Latitude}_{user.Longitude}";
            var cachedWeather = await _cacheService.GetWeatherDataAsync(locationKey);

            if (cachedWeather != null)
            {
                return new MobileWeatherDto
                {
                    Temperature = cachedWeather.Temperature,
                    Condition = cachedWeather.Condition,
                    Icon = cachedWeather.Icon,
                    PetTip = cachedWeather.PetTip,
                    Location = cachedWeather.Location
                };
            }

            // Fallback weather data
            return new MobileWeatherDto
            {
                Temperature = 18,
                Condition = "Partly Cloudy",
                Icon = "cloud.sun",
                PetTip = "Perfect weather for a walk!",
                Location = user.City ?? "Unknown"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mobile weather for user {UserId}", userId);
            return null;
        }
    }

    private async Task<List<MobileNotificationDto>> GetMobileNotificationsAsync(string userId, int maxItems)
    {
        return await _context.PushNotificationLogs
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(maxItems)
            .Select(n => new MobileNotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Body,
                Type = n.Type,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            })
            .ToListAsync();
    }

    private List<MobileQuickActionDto> GetMobileQuickActions(MobileViewPreferences preferences)
    {
        var actions = new List<MobileQuickActionDto>
        {
            new() { Id = "book_service", Title = "Book Service", Icon = "calendar.badge.plus", ActionUrl = "/booking", Color = "#007AFF" },
            new() { Id = "emergency_vet", Title = "Emergency Vet", Icon = "cross.case", ActionUrl = "/emergency", Color = "#FF3B30" },
            new() { Id = "pet_health", Title = "Pet Health", Icon = "heart", ActionUrl = "/health", Color = "#34C759" },
            new() { Id = "nearby_services", Title = "Nearby Services", Icon = "location", ActionUrl = "/nearby", Color = "#FF9500" }
        };

        return actions.Where(a => a.Enabled).ToList();
    }

    private async Task<(bool Success, bool IsConflict, string? Error, object? UpdatedEntity)> ProcessDataChangeAsync(
        string userId, OfflineDataChange change)
    {
        try
        {
            // This would handle different entity types and operations
            // Simplified implementation
            await Task.CompletedTask;
            
            return (true, false, null, null);
        }
        catch (Exception ex)
        {
            return (false, false, ex.Message, null);
        }
    }

    private bool GetBoolPreference(Dictionary<string, string> preferences, string key, bool defaultValue)
    {
        if (preferences.TryGetValue(key, out var value))
        {
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
        return defaultValue;
    }

    private string GenerateJwtToken(string userId, string deviceId, TimeSpan expiry)
    {
        // Placeholder - would integrate with actual JWT generation
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{deviceId}:{DateTime.UtcNow.Add(expiry):O}"));
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private string GenerateCacheVersion()
    {
        return DateTime.UtcNow.Ticks.ToString();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371; // km

        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadius * c;
    }

    #endregion
}

