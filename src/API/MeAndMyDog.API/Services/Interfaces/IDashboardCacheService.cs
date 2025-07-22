using MeAndMyDog.API.Models.DTOs.Dashboard;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Dashboard caching service for high-performance data retrieval
/// </summary>
public interface IDashboardCacheService
{
    /// <summary>
    /// Cache dashboard configuration for a user
    /// </summary>
    Task SetDashboardConfigAsync(string userId, DashboardConfigDto config, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get cached dashboard configuration
    /// </summary>
    Task<DashboardConfigDto?> GetDashboardConfigAsync(string userId);
    
    /// <summary>
    /// Cache user's pets data
    /// </summary>
    Task SetPetsDataAsync(string userId, List<PetSummaryDto> pets, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get cached pets data
    /// </summary>
    Task<List<PetSummaryDto>?> GetPetsDataAsync(string userId);
    
    /// <summary>
    /// Cache upcoming services
    /// </summary>
    Task SetUpcomingServicesAsync(string userId, List<UpcomingServiceDto> services, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get cached upcoming services
    /// </summary>
    Task<List<UpcomingServiceDto>?> GetUpcomingServicesAsync(string userId);
    
    /// <summary>
    /// Cache weather data by location
    /// </summary>
    Task SetWeatherDataAsync(string locationKey, WeatherDataDto weather, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get cached weather data
    /// </summary>
    Task<WeatherDataDto?> GetWeatherDataAsync(string locationKey);
    
    /// <summary>
    /// Cache dashboard analytics data
    /// </summary>
    Task SetAnalyticsDataAsync(string userId, DashboardAnalyticsDto analytics, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get cached analytics data
    /// </summary>
    Task<DashboardAnalyticsDto?> GetAnalyticsDataAsync(string userId);
    
    /// <summary>
    /// Invalidate all cached data for a user
    /// </summary>
    Task InvalidateUserCacheAsync(string userId);
    
    /// <summary>
    /// Batch invalidate cache keys
    /// </summary>
    Task InvalidateCacheKeysAsync(IEnumerable<string> keys);
    
    /// <summary>
    /// Get cache statistics
    /// </summary>
    Task<DashboardCacheStatsDto> GetCacheStatisticsAsync();
}