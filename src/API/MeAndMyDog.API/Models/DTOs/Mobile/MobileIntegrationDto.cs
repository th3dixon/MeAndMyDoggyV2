namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile device registration data
/// </summary>
public class MobileDeviceRegistrationDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // iOS, Android
    public string AppVersion { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public Dictionary<string, bool> NotificationPermissions { get; set; } = new();
    public TimeZoneInfo? TimeZone { get; set; }
    public string? Language { get; set; }
}

/// <summary>
/// Device registration result
/// </summary>
public class MobileDeviceRegistrationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string DeviceRegistrationId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Mobile push notification data
/// </summary>
public class MobilePushNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = "general"; // appointment, booking, system, etc.
    public Dictionary<string, object> Data { get; set; } = new();
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public string? Sound { get; set; }
    public int Badge { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string Priority { get; set; } = "normal"; // low, normal, high
    public TimeSpan? TimeToLive { get; set; }
}

/// <summary>
/// Push notification result
/// </summary>
public class PushNotificationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string NotificationId { get; set; } = string.Empty;
    public int DevicesReached { get; set; }
    public List<string> FailedDevices { get; set; } = new();
}

/// <summary>
/// Bulk notification request
/// </summary>
public class BulkNotificationRequest
{
    public List<string> UserIds { get; set; } = new();
    public MobilePushNotificationDto Notification { get; set; } = new();
    public Dictionary<string, string>? UserSpecificData { get; set; }
    public bool AllowDuplicates { get; set; } = false;
}

/// <summary>
/// Bulk notification result
/// </summary>
public class BulkNotificationResult
{
    public bool Success { get; set; }
    public int TotalTargetUsers { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Mobile-optimized dashboard data
/// </summary>
public class MobileDashboardDto
{
    public MobileQuickStatsDto QuickStats { get; set; } = new();
    public List<MobilePetSummaryDto> Pets { get; set; } = new();
    public List<MobileUpcomingServiceDto> UpcomingServices { get; set; } = new();
    public List<MobileActivityDto> RecentActivity { get; set; } = new();
    public MobileWeatherDto? Weather { get; set; }
    public List<MobileNotificationDto> Notifications { get; set; } = new();
    public List<MobileQuickActionDto> QuickActions { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string CacheVersion { get; set; } = string.Empty;
}

/// <summary>
/// Mobile view preferences
/// </summary>
public class MobileViewPreferences
{
    public List<string> EnabledWidgets { get; set; } = new();
    public string Theme { get; set; } = "system"; // light, dark, system
    public int MaxItemsPerWidget { get; set; } = 5;
    public bool ReduceAnimations { get; set; }
    public bool HighContrast { get; set; }
    public string DataUsageMode { get; set; } = "normal"; // minimal, normal, full
}

/// <summary>
/// Mobile quick stats
/// </summary>
public class MobileQuickStatsDto
{
    public int PetCount { get; set; }
    public int UpcomingServices { get; set; }
    public int UnreadNotifications { get; set; }
    public string NextAppointment { get; set; } = string.Empty;
    public decimal MonthlySpending { get; set; }
}

/// <summary>
/// Mobile pet summary
/// </summary>
public class MobilePetSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string HealthStatus { get; set; } = "Unknown";
    public string? NextAppointment { get; set; }
    public List<string> HealthAlerts { get; set; } = new();
}

/// <summary>
/// Mobile upcoming service
/// </summary>
public class MobileUpcomingServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? PetName { get; set; }
}

/// <summary>
/// Mobile activity item
/// </summary>
public class MobileActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
}

/// <summary>
/// Mobile weather data
/// </summary>
public class MobileWeatherDto
{
    public int Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string PetTip { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

/// <summary>
/// Mobile notification
/// </summary>
public class MobileNotificationDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
}

/// <summary>
/// Mobile quick action
/// </summary>
public class MobileQuickActionDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007AFF";
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Offline sync request
/// </summary>
public class OfflineSyncRequest
{
    public DateTime LastSyncTime { get; set; }
    public List<OfflineDataChange> Changes { get; set; } = new();
    public string DeviceId { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
}

/// <summary>
/// Offline data change
/// </summary>
public class OfflineDataChange
{
    public string Id { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // create, update, delete
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Sync result
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public List<string> Conflicts { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public DateTime ServerTime { get; set; } = DateTime.UtcNow;
    public int ProcessedChanges { get; set; }
    public Dictionary<string, object> UpdatedData { get; set; } = new();
}

/// <summary>
/// Real-time update data
/// </summary>
public class MobileRealtimeUpdateDto
{
    public List<RealtimeUpdate> Updates { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool HasMoreUpdates { get; set; }
}

/// <summary>
/// Individual real-time update
/// </summary>
public class RealtimeUpdate
{
    public string Type { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Mobile analytics event
/// </summary>
public class MobileAnalyticsEventDto
{
    public string EventName { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string DeviceId { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
}

/// <summary>
/// Notification preferences
/// </summary>
public class NotificationPreferencesDto
{
    public bool AppointmentReminders { get; set; } = true;
    public bool BookingConfirmations { get; set; } = true;
    public bool HealthReminders { get; set; } = true;
    public bool ProviderMessages { get; set; } = true;
    public bool SystemAnnouncements { get; set; } = true;
    public bool MarketingOffers { get; set; } = false;
    public Dictionary<string, bool> CustomPreferences { get; set; } = new();
    public string QuietHoursStart { get; set; } = "22:00";
    public string QuietHoursEnd { get; set; } = "07:00";
    public bool WeekendNotifications { get; set; } = true;
}

/// <summary>
/// Mobile API token
/// </summary>
public class MobileApiTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public List<string> Scopes { get; set; } = new();
}

/// <summary>
/// Mobile app configuration
/// </summary>
public class MobileAppConfigDto
{
    public string MinimumVersion { get; set; } = string.Empty;
    public bool ForceUpdate { get; set; }
    public Dictionary<string, object> FeatureFlags { get; set; } = new();
    public List<string> MaintenanceWindows { get; set; } = new();
    public Dictionary<string, string> ApiEndpoints { get; set; } = new();
    public int MaxCacheAge { get; set; } = 3600;
    public Dictionary<string, int> RateLimits { get; set; } = new();
}

/// <summary>
/// Deep link processing result
/// </summary>
public class DeepLinkResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string TargetScreen { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool RequiresAuthentication { get; set; }
    public string? RedirectUrl { get; set; }
}

/// <summary>
/// Nearby service data
/// </summary>
public class NearbyServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public double Distance { get; set; }
    public decimal? Price { get; set; }
    public double Rating { get; set; }
    public bool Available { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
}