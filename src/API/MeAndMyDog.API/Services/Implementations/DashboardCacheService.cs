using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MeAndMyDog.API.Models.DTOs.Dashboard;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// High-performance dashboard caching service using Redis/In-Memory cache
/// </summary>
public class DashboardCacheService : IDashboardCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DashboardCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    // Cache key prefixes for organization
    private const string CONFIG_PREFIX = "dashboard:config:";
    private const string PETS_PREFIX = "dashboard:pets:";
    private const string SERVICES_PREFIX = "dashboard:services:";
    private const string WEATHER_PREFIX = "dashboard:weather:";
    private const string ANALYTICS_PREFIX = "dashboard:analytics:";
    
    // Default cache durations
    private static readonly TimeSpan DefaultConfigExpiry = TimeSpan.FromHours(24);
    private static readonly TimeSpan DefaultPetsExpiry = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan DefaultServicesExpiry = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan DefaultWeatherExpiry = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan DefaultAnalyticsExpiry = TimeSpan.FromHours(6);

    public DashboardCacheService(
        IDistributedCache cache, 
        ILogger<DashboardCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task SetDashboardConfigAsync(string userId, DashboardConfigDto config, TimeSpan? expiry = null)
    {
        try
        {
            var key = CONFIG_PREFIX + userId;
            var json = JsonSerializer.Serialize(config, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultConfigExpiry
            };
            
            await _cache.SetStringAsync(key, json, options);
            _logger.LogDebug("Cached dashboard config for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching dashboard config for user {UserId}", userId);
        }
    }

    public async Task<DashboardConfigDto?> GetDashboardConfigAsync(string userId)
    {
        try
        {
            var key = CONFIG_PREFIX + userId;
            var json = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Dashboard config cache miss for user {UserId}", userId);
                return null;
            }
            
            var config = JsonSerializer.Deserialize<DashboardConfigDto>(json, _jsonOptions);
            _logger.LogDebug("Dashboard config cache hit for user {UserId}", userId);
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard config cache for user {UserId}", userId);
            return null;
        }
    }

    public async Task SetPetsDataAsync(string userId, List<PetSummaryDto> pets, TimeSpan? expiry = null)
    {
        try
        {
            var key = PETS_PREFIX + userId;
            var json = JsonSerializer.Serialize(pets, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultPetsExpiry
            };
            
            await _cache.SetStringAsync(key, json, options);
            _logger.LogDebug("Cached pets data for user {UserId}, {Count} pets", userId, pets.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching pets data for user {UserId}", userId);
        }
    }

    public async Task<List<PetSummaryDto>?> GetPetsDataAsync(string userId)
    {
        try
        {
            var key = PETS_PREFIX + userId;
            var json = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Pets data cache miss for user {UserId}", userId);
                return null;
            }
            
            var pets = JsonSerializer.Deserialize<List<PetSummaryDto>>(json, _jsonOptions);
            _logger.LogDebug("Pets data cache hit for user {UserId}", userId);
            return pets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pets data cache for user {UserId}", userId);
            return null;
        }
    }

    public async Task SetUpcomingServicesAsync(string userId, List<UpcomingServiceDto> services, TimeSpan? expiry = null)
    {
        try
        {
            var key = SERVICES_PREFIX + userId;
            var json = JsonSerializer.Serialize(services, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultServicesExpiry
            };
            
            await _cache.SetStringAsync(key, json, options);
            _logger.LogDebug("Cached upcoming services for user {UserId}, {Count} services", userId, services.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching upcoming services for user {UserId}", userId);
        }
    }

    public async Task<List<UpcomingServiceDto>?> GetUpcomingServicesAsync(string userId)
    {
        try
        {
            var key = SERVICES_PREFIX + userId;
            var json = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Upcoming services cache miss for user {UserId}", userId);
                return null;
            }
            
            var services = JsonSerializer.Deserialize<List<UpcomingServiceDto>>(json, _jsonOptions);
            _logger.LogDebug("Upcoming services cache hit for user {UserId}", userId);
            return services;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming services cache for user {UserId}", userId);
            return null;
        }
    }

    public async Task SetWeatherDataAsync(string locationKey, WeatherDataDto weather, TimeSpan? expiry = null)
    {
        try
        {
            var key = WEATHER_PREFIX + locationKey;
            var json = JsonSerializer.Serialize(weather, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultWeatherExpiry
            };
            
            await _cache.SetStringAsync(key, json, options);
            _logger.LogDebug("Cached weather data for location {LocationKey}", locationKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching weather data for location {LocationKey}", locationKey);
        }
    }

    public async Task<WeatherDataDto?> GetWeatherDataAsync(string locationKey)
    {
        try
        {
            var key = WEATHER_PREFIX + locationKey;
            var json = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Weather data cache miss for location {LocationKey}", locationKey);
                return null;
            }
            
            var weather = JsonSerializer.Deserialize<WeatherDataDto>(json, _jsonOptions);
            _logger.LogDebug("Weather data cache hit for location {LocationKey}", locationKey);
            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather data cache for location {LocationKey}", locationKey);
            return null;
        }
    }

    public async Task SetAnalyticsDataAsync(string userId, DashboardAnalyticsDto analytics, TimeSpan? expiry = null)
    {
        try
        {
            var key = ANALYTICS_PREFIX + userId;
            var json = JsonSerializer.Serialize(analytics, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultAnalyticsExpiry
            };
            
            await _cache.SetStringAsync(key, json, options);
            _logger.LogDebug("Cached analytics data for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching analytics data for user {UserId}", userId);
        }
    }

    public async Task<DashboardAnalyticsDto?> GetAnalyticsDataAsync(string userId)
    {
        try
        {
            var key = ANALYTICS_PREFIX + userId;
            var json = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Analytics data cache miss for user {UserId}", userId);
                return null;
            }
            
            var analytics = JsonSerializer.Deserialize<DashboardAnalyticsDto>(json, _jsonOptions);
            _logger.LogDebug("Analytics data cache hit for user {UserId}", userId);
            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics data cache for user {UserId}", userId);
            return null;
        }
    }

    public async Task InvalidateUserCacheAsync(string userId)
    {
        try
        {
            var keys = new[]
            {
                CONFIG_PREFIX + userId,
                PETS_PREFIX + userId,
                SERVICES_PREFIX + userId,
                ANALYTICS_PREFIX + userId
            };

            await InvalidateCacheKeysAsync(keys);
            _logger.LogDebug("Invalidated all cache for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache for user {UserId}", userId);
        }
    }

    public async Task InvalidateCacheKeysAsync(IEnumerable<string> keys)
    {
        try
        {
            var tasks = keys.Select(key => _cache.RemoveAsync(key));
            await Task.WhenAll(tasks);
            _logger.LogDebug("Invalidated {Count} cache keys", keys.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache keys");
        }
    }

    public async Task<DashboardCacheStatsDto> GetCacheStatisticsAsync()
    {
        // Note: This is a simplified implementation
        // For production, you'd want to use Redis INFO commands or memory cache statistics
        try
        {
            await Task.CompletedTask;
            
            return new DashboardCacheStatsDto
            {
                TotalKeys = 0,
                HitCount = 0,
                MissCount = 0,
                HitRatio = 0.0,
                KeysByType = new Dictionary<string, long>
                {
                    ["config"] = 0,
                    ["pets"] = 0,
                    ["services"] = 0,
                    ["weather"] = 0,
                    ["analytics"] = 0
                },
                MemoryUsageBytes = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache statistics");
            return new DashboardCacheStatsDto();
        }
    }
}