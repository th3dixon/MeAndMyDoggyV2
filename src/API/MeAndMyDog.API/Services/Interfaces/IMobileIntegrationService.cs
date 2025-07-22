using MeAndMyDog.API.Models.DTOs.Mobile;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Mobile app integration service for dashboard functionality
/// </summary>
public interface IMobileIntegrationService
{
    /// <summary>
    /// Register mobile device for push notifications
    /// </summary>
    Task<MobileDeviceRegistrationResult> RegisterDeviceAsync(string userId, MobileDeviceRegistrationDto registration);
    
    /// <summary>
    /// Send push notification to user's devices
    /// </summary>
    Task<PushNotificationResult> SendNotificationAsync(string userId, MobilePushNotificationDto notification);
    
    /// <summary>
    /// Send bulk notifications to multiple users
    /// </summary>
    Task<BulkNotificationResult> SendBulkNotificationsAsync(BulkNotificationRequest request);
    
    /// <summary>
    /// Get mobile-optimized dashboard data
    /// </summary>
    Task<MobileDashboardDto> GetMobileDashboardAsync(string userId, MobileViewPreferences? preferences = null);
    
    /// <summary>
    /// Sync offline mobile data
    /// </summary>
    Task<SyncResult> SyncOfflineDataAsync(string userId, OfflineSyncRequest syncRequest);
    
    /// <summary>
    /// Get real-time updates for mobile dashboard
    /// </summary>
    Task<MobileRealtimeUpdateDto> GetRealtimeUpdatesAsync(string userId, DateTime lastUpdateTime);
    
    /// <summary>
    /// Track mobile app analytics
    /// </summary>
    Task TrackMobileAnalyticsAsync(string userId, MobileAnalyticsEventDto analyticsEvent);
    
    /// <summary>
    /// Get user notification preferences
    /// </summary>
    Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(string userId);
    
    /// <summary>
    /// Update notification preferences
    /// </summary>
    Task<bool> UpdateNotificationPreferencesAsync(string userId, NotificationPreferencesDto preferences);
    
    /// <summary>
    /// Generate mobile API token
    /// </summary>
    Task<MobileApiTokenDto> GenerateMobileApiTokenAsync(string userId, string deviceId, TimeSpan? expiry = null);
    
    /// <summary>
    /// Refresh mobile API token
    /// </summary>
    Task<MobileApiTokenDto> RefreshMobileApiTokenAsync(string refreshToken);
    
    /// <summary>
    /// Get mobile app configuration
    /// </summary>
    Task<MobileAppConfigDto> GetMobileAppConfigAsync(string version, string platform);
    
    /// <summary>
    /// Process mobile deep link
    /// </summary>
    Task<DeepLinkResult> ProcessDeepLinkAsync(string userId, string deepLink);
    
    /// <summary>
    /// Get location-based services for mobile
    /// </summary>
    Task<List<NearbyServiceDto>> GetNearbyServicesAsync(double latitude, double longitude, int radiusKm = 5);
}