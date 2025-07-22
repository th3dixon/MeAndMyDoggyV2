using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for push notification operations
/// </summary>
public class PushNotificationService : IPushNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PushNotificationService> _logger;

    /// <summary>
    /// Initialize the push notification service
    /// </summary>
    public PushNotificationService(ApplicationDbContext context, ILogger<PushNotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<NotificationDeviceDto> RegisterDeviceAsync(string userId, RegisterDeviceRequest request)
    {
        try
        {
            // Check if device with same token already exists for this user
            var existingDevice = await _context.NotificationDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceToken == request.DeviceToken);

            if (existingDevice != null)
            {
                // Update existing device
                existingDevice.Platform = request.Platform;
                existingDevice.DeviceName = request.DeviceName;
                existingDevice.AppVersion = request.AppVersion;
                existingDevice.OsVersion = request.OsVersion;
                existingDevice.Language = request.Language;
                existingDevice.TimeZone = request.TimeZone;
                existingDevice.IsActive = true;
                existingDevice.NotificationsEnabled = true;
                existingDevice.LastSeenAt = DateTimeOffset.UtcNow;
                existingDevice.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                return MapToDeviceDto(existingDevice);
            }

            // Create new device
            var device = new NotificationDevice
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                DeviceToken = request.DeviceToken,
                Platform = request.Platform,
                DeviceName = request.DeviceName,
                AppVersion = request.AppVersion,
                OsVersion = request.OsVersion,
                Language = request.Language ?? "en",
                TimeZone = request.TimeZone,
                IsActive = true,
                NotificationsEnabled = true,
                LastSeenAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.NotificationDevices.Add(device);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} registered successfully for user {UserId}", 
                device.Id, userId);

            return MapToDeviceDto(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UnregisterDeviceAsync(string userId, string deviceId)
    {
        try
        {
            var device = await _context.NotificationDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);

            if (device == null)
            {
                return false;
            }

            device.IsActive = false;
            device.NotificationsEnabled = false;
            device.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} unregistered for user {UserId}", deviceId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering device {DeviceId} for user {UserId}", deviceId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NotificationDeviceDto?> UpdateDeviceAsync(string userId, string deviceId, RegisterDeviceRequest request)
    {
        try
        {
            var device = await _context.NotificationDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);

            if (device == null)
            {
                return null;
            }

            device.DeviceToken = request.DeviceToken;
            device.Platform = request.Platform;
            device.DeviceName = request.DeviceName;
            device.AppVersion = request.AppVersion;
            device.OsVersion = request.OsVersion;
            device.Language = request.Language ?? "en";
            device.TimeZone = request.TimeZone;
            device.LastSeenAt = DateTimeOffset.UtcNow;
            device.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} updated for user {UserId}", deviceId, userId);
            return MapToDeviceDto(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {DeviceId} for user {UserId}", deviceId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<NotificationDeviceDto>> GetUserDevicesAsync(string userId)
    {
        try
        {
            var devices = await _context.NotificationDevices
                .Where(d => d.UserId == userId && d.IsActive)
                .OrderByDescending(d => d.LastSeenAt)
                .ToListAsync();

            return devices.Select(MapToDeviceDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> SendNotificationAsync(SendPushNotificationRequest request, string? createdBy = null)
    {
        try
        {
            // Create notification entity
            var notification = new PushNotification
            {
                Id = Guid.NewGuid().ToString(),
                TargetUserId = request.TargetUserId,
                TargetDeviceId = request.TargetDeviceId,
                NotificationType = EnumConverter.ToString(
                    Enum.TryParse<NotificationType>(request.NotificationType, true, out var notifType) 
                    ? notifType : NotificationType.General),
                Title = request.Title,
                Body = request.Body,
                IconUrl = request.IconUrl,
                ImageUrl = request.ImageUrl,
                Sound = request.Sound ?? "default",
                Badge = request.Badge,
                CustomData = request.CustomData != null ? JsonSerializer.Serialize(request.CustomData) : null,
                ActionUrl = request.ActionUrl,
                Priority = request.Priority,
                ScheduledAt = request.ScheduledAt,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = createdBy ?? "system",
                Status = request.ScheduledAt.HasValue ? 
                    EnumConverter.ToString(NotificationStatus.Scheduled) : 
                    EnumConverter.ToString(NotificationStatus.Pending)
            };

            _context.PushNotifications.Add(notification);
            await _context.SaveChangesAsync();

            var response = new NotificationResponse
            {
                Success = true,
                Message = "Notification created successfully",
                NotificationId = notification.Id
            };

            // If not scheduled, send immediately
            if (!request.ScheduledAt.HasValue)
            {
                await ProcessNotificationAsync(notification);
                
                // Get delivery statistics
                var deliveries = await _context.NotificationDeliveries
                    .Where(d => d.NotificationId == notification.Id)
                    .ToListAsync();

                response.DevicesTargeted = deliveries.Count;
                response.DeliveriesSuccessful = deliveries.Count(d => 
                    d.Status == EnumConverter.ToString(DeliveryStatus.Delivered));
                response.DeliveriesFailed = deliveries.Count(d => 
                    d.Status == EnumConverter.ToString(DeliveryStatus.Failed));
            }

            _logger.LogInformation("Notification {NotificationId} created successfully", notification.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return new NotificationResponse
            {
                Success = false,
                Message = $"Failed to send notification: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> SendNotificationToUsersAsync(List<string> userIds, 
        SendPushNotificationRequest request, string? createdBy = null)
    {
        try
        {
            var responses = new List<NotificationResponse>();
            
            foreach (var userId in userIds)
            {
                var userRequest = new SendPushNotificationRequest
                {
                    TargetUserId = userId,
                    NotificationType = request.NotificationType,
                    Title = request.Title,
                    Body = request.Body,
                    IconUrl = request.IconUrl,
                    ImageUrl = request.ImageUrl,
                    Sound = request.Sound,
                    Badge = request.Badge,
                    CustomData = request.CustomData,
                    ActionUrl = request.ActionUrl,
                    Priority = request.Priority,
                    ScheduledAt = request.ScheduledAt
                };

                var response = await SendNotificationAsync(userRequest, createdBy);
                responses.Add(response);
            }

            var successCount = responses.Count(r => r.Success);
            var totalDevices = responses.Sum(r => r.DevicesTargeted);
            var totalSuccessful = responses.Sum(r => r.DeliveriesSuccessful);
            var totalFailed = responses.Sum(r => r.DeliveriesFailed);

            return new NotificationResponse
            {
                Success = successCount == userIds.Count,
                Message = $"Sent notifications to {successCount}/{userIds.Count} users",
                DevicesTargeted = totalDevices,
                DeliveriesSuccessful = totalSuccessful,
                DeliveriesFailed = totalFailed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications to users");
            return new NotificationResponse
            {
                Success = false,
                Message = $"Failed to send notifications: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> SendBroadcastNotificationAsync(SendPushNotificationRequest request, 
        string? createdBy = null)
    {
        try
        {
            // Get all active users with devices
            var activeUserIds = await _context.NotificationDevices
                .Where(d => d.IsActive && d.NotificationsEnabled)
                .Select(d => d.UserId)
                .Distinct()
                .ToListAsync();

            return await SendNotificationToUsersAsync(activeUserIds, request, createdBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending broadcast notification");
            return new NotificationResponse
            {
                Success = false,
                Message = $"Failed to send broadcast notification: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<PushNotificationDto?> GetNotificationAsync(string notificationId)
    {
        try
        {
            var notification = await _context.PushNotifications
                .Include(n => n.NotificationDeliveries)
                    .ThenInclude(d => d.Device)
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            return notification != null ? MapToNotificationDto(notification) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<PushNotificationDto>> GetUserNotificationsAsync(string userId, int skip = 0, int take = 50)
    {
        try
        {
            var notifications = await _context.PushNotifications
                .Include(n => n.NotificationDeliveries)
                    .ThenInclude(d => d.Device)
                .Where(n => n.TargetUserId == userId || n.TargetUserId == null)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return notifications.Select(MapToNotificationDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MarkNotificationOpenedAsync(string notificationId, string deviceId)
    {
        try
        {
            var delivery = await _context.NotificationDeliveries
                .FirstOrDefaultAsync(d => d.NotificationId == notificationId && d.DeviceId == deviceId);

            if (delivery == null)
            {
                return false;
            }

            if (delivery.OpenedAt == null)
            {
                delivery.OpenedAt = DateTimeOffset.UtcNow;
                delivery.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogDebug("Notification {NotificationId} marked as opened for device {DeviceId}", 
                    notificationId, deviceId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as opened", notificationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CancelNotificationAsync(string notificationId)
    {
        try
        {
            var notification = await _context.PushNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return false;
            }

            var currentStatus = EnumConverter.ToNotificationStatus(notification.Status);
            if (currentStatus == NotificationStatus.Pending || currentStatus == NotificationStatus.Scheduled)
            {
                notification.Status = EnumConverter.ToString(NotificationStatus.Cancelled);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification {NotificationId} cancelled", notificationId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling notification {NotificationId}", notificationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<NotificationPreferenceDto>> GetNotificationPreferencesAsync(string userId)
    {
        try
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return preferences.Select(MapToPreferenceDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification preferences for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateNotificationPreferencesAsync(string userId, UpdateNotificationPreferencesRequest request)
    {
        try
        {
            foreach (var preferenceUpdate in request.Preferences)
            {
                var existingPreference = await _context.NotificationPreferences
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == preferenceUpdate.NotificationType);

                if (existingPreference != null)
                {
                    // Update existing preference
                    existingPreference.IsEnabled = preferenceUpdate.IsEnabled;
                    existingPreference.PushEnabled = preferenceUpdate.PushEnabled;
                    existingPreference.EmailEnabled = preferenceUpdate.EmailEnabled;
                    existingPreference.SmsEnabled = preferenceUpdate.SmsEnabled;
                    existingPreference.InAppEnabled = preferenceUpdate.InAppEnabled;
                    existingPreference.CustomSound = preferenceUpdate.CustomSound;
                    existingPreference.QuietHoursStart = preferenceUpdate.QuietHours?.Start;
                    existingPreference.QuietHoursEnd = preferenceUpdate.QuietHours?.End;
                    existingPreference.TimeZone = preferenceUpdate.QuietHours?.TimeZone;
                    existingPreference.QuietHoursDays = preferenceUpdate.QuietHours?.Days != null 
                        ? string.Join(",", preferenceUpdate.QuietHours.Days) : null;
                    existingPreference.MinInterval = preferenceUpdate.MinInterval;
                    existingPreference.MaxPerDay = preferenceUpdate.MaxPerDay;
                    existingPreference.MinPriority = preferenceUpdate.MinPriority;
                    existingPreference.UpdatedAt = DateTimeOffset.UtcNow;
                }
                else
                {
                    // Create new preference
                    var newPreference = new NotificationPreference
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        NotificationType = preferenceUpdate.NotificationType,
                        IsEnabled = preferenceUpdate.IsEnabled,
                        PushEnabled = preferenceUpdate.PushEnabled,
                        EmailEnabled = preferenceUpdate.EmailEnabled,
                        SmsEnabled = preferenceUpdate.SmsEnabled,
                        InAppEnabled = preferenceUpdate.InAppEnabled,
                        CustomSound = preferenceUpdate.CustomSound,
                        QuietHoursStart = preferenceUpdate.QuietHours?.Start,
                        QuietHoursEnd = preferenceUpdate.QuietHours?.End,
                        TimeZone = preferenceUpdate.QuietHours?.TimeZone,
                        QuietHoursDays = preferenceUpdate.QuietHours?.Days != null 
                            ? string.Join(",", preferenceUpdate.QuietHours.Days) : null,
                        MinInterval = preferenceUpdate.MinInterval,
                        MaxPerDay = preferenceUpdate.MaxPerDay,
                        MinPriority = preferenceUpdate.MinPriority,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    _context.NotificationPreferences.Add(newPreference);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification preferences updated for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsNotificationAllowedAsync(string userId, NotificationType notificationType)
    {
        try
        {
            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && 
                    p.NotificationType == EnumConverter.ToString(notificationType));

            if (preference == null)
            {
                // Default to allowed if no preference is set
                return true;
            }

            return preference.IsEnabled && preference.PushEnabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking notification permission for user {UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<int> ProcessScheduledNotificationsAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var scheduledNotifications = await _context.PushNotifications
                .Where(n => n.Status == EnumConverter.ToString(NotificationStatus.Scheduled) &&
                           n.ScheduledAt.HasValue && n.ScheduledAt <= now)
                .ToListAsync();

            var processedCount = 0;
            foreach (var notification in scheduledNotifications)
            {
                await ProcessNotificationAsync(notification);
                processedCount++;
            }

            if (processedCount > 0)
            {
                _logger.LogInformation("Processed {Count} scheduled notifications", processedCount);
            }

            return processedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scheduled notifications");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<int> RetryFailedNotificationsAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var failedDeliveries = await _context.NotificationDeliveries
                .Where(d => d.Status == EnumConverter.ToString(DeliveryStatus.Failed) &&
                           d.NextRetryAt.HasValue && d.NextRetryAt <= now &&
                           d.RetryCount < 3) // Max 3 retries
                .ToListAsync();

            var retriedCount = 0;
            foreach (var delivery in failedDeliveries)
            {
                await RetryNotificationDeliveryAsync(delivery);
                retriedCount++;
            }

            if (retriedCount > 0)
            {
                _logger.LogInformation("Retried {Count} failed notifications", retriedCount);
            }

            return retriedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying failed notifications");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredDataAsync()
    {
        try
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddDays(-30); // Keep data for 30 days
            var cleanupCount = 0;

            // Clean up old notifications
            var oldNotifications = await _context.PushNotifications
                .Where(n => n.CreatedAt < cutoffDate)
                .ToListAsync();

            if (oldNotifications.Any())
            {
                _context.PushNotifications.RemoveRange(oldNotifications);
                cleanupCount += oldNotifications.Count;
            }

            // Clean up inactive devices (not seen for 90 days)
            var deviceCutoffDate = DateTimeOffset.UtcNow.AddDays(-90);
            var inactiveDevices = await _context.NotificationDevices
                .Where(d => d.LastSeenAt < deviceCutoffDate)
                .ToListAsync();

            if (inactiveDevices.Any())
            {
                _context.NotificationDevices.RemoveRange(inactiveDevices);
                cleanupCount += inactiveDevices.Count;
            }

            await _context.SaveChangesAsync();

            if (cleanupCount > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired records", cleanupCount);
            }

            return cleanupCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired data");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<NotificationStatistics> GetNotificationStatisticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        try
        {
            var notifications = await _context.PushNotifications
                .Include(n => n.NotificationDeliveries)
                .Where(n => n.CreatedAt >= fromDate && n.CreatedAt <= toDate)
                .ToListAsync();

            var stats = new NotificationStatistics
            {
                TotalSent = notifications.Count,
                TotalDelivered = notifications.Sum(n => n.NotificationDeliveries.Count(d => 
                    d.Status == EnumConverter.ToString(DeliveryStatus.Delivered))),
                TotalOpened = notifications.Sum(n => n.NotificationDeliveries.Count(d => d.OpenedAt.HasValue)),
                TotalFailed = notifications.Sum(n => n.NotificationDeliveries.Count(d => 
                    d.Status == EnumConverter.ToString(DeliveryStatus.Failed)))
            };

            stats.DeliveryRate = stats.TotalSent > 0 ? (decimal)stats.TotalDelivered / stats.TotalSent * 100 : 0;
            stats.OpenRate = stats.TotalDelivered > 0 ? (decimal)stats.TotalOpened / stats.TotalDelivered * 100 : 0;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification statistics");
            throw;
        }
    }

    /// <summary>
    /// Process a notification by sending it to target devices
    /// </summary>
    private async Task ProcessNotificationAsync(PushNotification notification)
    {
        try
        {
            List<NotificationDevice> targetDevices;

            if (!string.IsNullOrEmpty(notification.TargetDeviceId))
            {
                // Send to specific device
                var device = await _context.NotificationDevices
                    .FirstOrDefaultAsync(d => d.Id == notification.TargetDeviceId && d.IsActive && d.NotificationsEnabled);
                
                targetDevices = device != null ? new List<NotificationDevice> { device } : new List<NotificationDevice>();
            }
            else if (!string.IsNullOrEmpty(notification.TargetUserId))
            {
                // Send to all user's devices
                targetDevices = await _context.NotificationDevices
                    .Where(d => d.UserId == notification.TargetUserId && d.IsActive && d.NotificationsEnabled)
                    .ToListAsync();
            }
            else
            {
                // Broadcast to all devices
                targetDevices = await _context.NotificationDevices
                    .Where(d => d.IsActive && d.NotificationsEnabled)
                    .ToListAsync();
            }

            // Check notification preferences for each target device
            if (!string.IsNullOrEmpty(notification.TargetUserId))
            {
                var notificationType = EnumConverter.ToNotificationType(notification.NotificationType);
                var isAllowed = await IsNotificationAllowedAsync(notification.TargetUserId, notificationType);
                if (!isAllowed)
                {
                    _logger.LogDebug("Notification {NotificationId} blocked by user preferences", notification.Id);
                    notification.Status = EnumConverter.ToString(NotificationStatus.Cancelled);
                    await _context.SaveChangesAsync();
                    return;
                }
            }

            // Create delivery records for each target device
            foreach (var device in targetDevices)
            {
                var delivery = new NotificationDelivery
                {
                    Id = Guid.NewGuid().ToString(),
                    NotificationId = notification.Id,
                    DeviceId = device.Id,
                    Provider = GetPushProviderForPlatform(device.Platform),
                    Status = EnumConverter.ToString(DeliveryStatus.Pending),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                _context.NotificationDeliveries.Add(delivery);

                // Simulate sending to push service (in real implementation, integrate with FCM/APNs/etc.)
                await SimulatePushDeliveryAsync(delivery, notification, device);
            }

            notification.Status = EnumConverter.ToString(NotificationStatus.Sent);
            notification.SentAt = DateTimeOffset.UtcNow;
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification {NotificationId}", notification.Id);
            
            notification.Status = EnumConverter.ToString(NotificationStatus.Failed);
            notification.ErrorMessage = ex.Message;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Simulate push notification delivery (replace with real implementation)
    /// </summary>
    private async Task SimulatePushDeliveryAsync(NotificationDelivery delivery, PushNotification notification, NotificationDevice device)
    {
        try
        {
            // In a real implementation, this would call FCM, APNs, or other push services
            // For now, we'll simulate a successful delivery
            
            delivery.Status = EnumConverter.ToString(DeliveryStatus.Delivered);
            delivery.AttemptedAt = DateTimeOffset.UtcNow;
            delivery.DeliveredAt = DateTimeOffset.UtcNow;
            delivery.ProviderMessageId = Guid.NewGuid().ToString();
            delivery.HttpStatusCode = 200;
            delivery.ProviderResponse = "Success";
            delivery.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogDebug("Push notification {NotificationId} delivered to device {DeviceId}", 
                notification.Id, device.Id);
        }
        catch (Exception ex)
        {
            delivery.Status = EnumConverter.ToString(DeliveryStatus.Failed);
            delivery.ErrorCode = "SEND_FAILED";
            delivery.ErrorMessage = ex.Message;
            delivery.RetryCount = 0;
            delivery.NextRetryAt = DateTimeOffset.UtcNow.AddMinutes(5); // Retry in 5 minutes
            delivery.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogWarning("Failed to deliver push notification {NotificationId} to device {DeviceId}: {Error}", 
                notification.Id, device.Id, ex.Message);
        }
    }

    /// <summary>
    /// Retry a failed notification delivery
    /// </summary>
    private async Task RetryNotificationDeliveryAsync(NotificationDelivery delivery)
    {
        try
        {
            var notification = await _context.PushNotifications
                .FirstOrDefaultAsync(n => n.Id == delivery.NotificationId);
            
            var device = await _context.NotificationDevices
                .FirstOrDefaultAsync(d => d.Id == delivery.DeviceId);

            if (notification != null && device != null)
            {
                delivery.RetryCount++;
                delivery.Status = EnumConverter.ToString(DeliveryStatus.Retrying);
                delivery.UpdatedAt = DateTimeOffset.UtcNow;

                await SimulatePushDeliveryAsync(delivery, notification, device);
            }
        }
        catch (Exception ex)
        {
            delivery.RetryCount++;
            delivery.Status = EnumConverter.ToString(DeliveryStatus.Failed);
            delivery.ErrorMessage = ex.Message;
            delivery.NextRetryAt = delivery.RetryCount < 3 ? 
                DateTimeOffset.UtcNow.AddMinutes(Math.Pow(2, delivery.RetryCount) * 5) : // Exponential backoff
                null;
            delivery.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogWarning("Retry {RetryCount} failed for notification delivery {DeliveryId}: {Error}", 
                delivery.RetryCount, delivery.Id, ex.Message);
        }
    }

    /// <summary>
    /// Get push service provider based on platform
    /// </summary>
    private static string GetPushProviderForPlatform(string platform)
    {
        return platform.ToLowerInvariant() switch
        {
            "ios" or "macos" => "APNs",
            "android" => "FCM",
            "web" => "WebPush",
            "windows" => "WNS",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static NotificationDeviceDto MapToDeviceDto(NotificationDevice device)
    {
        return new NotificationDeviceDto
        {
            Id = device.Id,
            UserId = device.UserId,
            DeviceToken = device.DeviceToken,
            Platform = device.Platform,
            DeviceName = device.DeviceName,
            AppVersion = device.AppVersion,
            OsVersion = device.OsVersion,
            Language = device.Language,
            TimeZone = device.TimeZone,
            IsActive = device.IsActive,
            NotificationsEnabled = device.NotificationsEnabled,
            LastSeenAt = device.LastSeenAt,
            CreatedAt = device.CreatedAt
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static PushNotificationDto MapToNotificationDto(PushNotification notification)
    {
        Dictionary<string, object>? customData = null;
        if (!string.IsNullOrEmpty(notification.CustomData))
        {
            try
            {
                customData = JsonSerializer.Deserialize<Dictionary<string, object>>(notification.CustomData);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }

        return new PushNotificationDto
        {
            Id = notification.Id,
            TargetUserId = notification.TargetUserId,
            TargetDeviceId = notification.TargetDeviceId,
            NotificationType = notification.NotificationType,
            Title = notification.Title,
            Body = notification.Body,
            IconUrl = notification.IconUrl,
            ImageUrl = notification.ImageUrl,
            Sound = notification.Sound,
            Badge = notification.Badge,
            CustomData = customData,
            ActionUrl = notification.ActionUrl,
            Priority = notification.Priority,
            ScheduledAt = notification.ScheduledAt,
            CreatedAt = notification.CreatedAt,
            Status = notification.Status,
            SentAt = notification.SentAt,
            Deliveries = notification.NotificationDeliveries?.Select(d => new NotificationDeliveryDto
            {
                Id = d.Id,
                DeviceId = d.DeviceId,
                Provider = d.Provider,
                ProviderMessageId = d.ProviderMessageId,
                Status = d.Status,
                AttemptedAt = d.AttemptedAt,
                DeliveredAt = d.DeliveredAt,
                OpenedAt = d.OpenedAt,
                ErrorMessage = d.ErrorMessage,
                RetryCount = d.RetryCount
            }).ToList() ?? new List<NotificationDeliveryDto>()
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static NotificationPreferenceDto MapToPreferenceDto(NotificationPreference preference)
    {
        QuietHoursDto? quietHours = null;
        if (!string.IsNullOrEmpty(preference.QuietHoursStart) || !string.IsNullOrEmpty(preference.QuietHoursEnd))
        {
            quietHours = new QuietHoursDto
            {
                Start = preference.QuietHoursStart,
                End = preference.QuietHoursEnd,
                TimeZone = preference.TimeZone,
                Days = !string.IsNullOrEmpty(preference.QuietHoursDays) 
                    ? preference.QuietHoursDays.Split(',').ToList() 
                    : null
            };
        }

        return new NotificationPreferenceDto
        {
            Id = preference.Id,
            UserId = preference.UserId,
            NotificationType = preference.NotificationType,
            IsEnabled = preference.IsEnabled,
            PushEnabled = preference.PushEnabled,
            EmailEnabled = preference.EmailEnabled,
            SmsEnabled = preference.SmsEnabled,
            InAppEnabled = preference.InAppEnabled,
            CustomSound = preference.CustomSound,
            QuietHours = quietHours,
            MinInterval = preference.MinInterval,
            MaxPerDay = preference.MaxPerDay,
            MinPriority = preference.MinPriority ?? "normal",
            UpdatedAt = preference.UpdatedAt
        };
    }
}