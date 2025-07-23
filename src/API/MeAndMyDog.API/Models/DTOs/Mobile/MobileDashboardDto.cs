namespace MeAndMyDog.API.Models.DTOs.Mobile;

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