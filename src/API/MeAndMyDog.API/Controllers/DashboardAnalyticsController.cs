using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Models.DTOs.Dashboard;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Dashboard analytics controller providing advanced insights and usage tracking
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardAnalyticsController : ControllerBase
{
    private readonly IDashboardAnalyticsService _analyticsService;
    private readonly IDashboardCacheService _cacheService;
    private readonly ILogger<DashboardAnalyticsController> _logger;

    public DashboardAnalyticsController(
        IDashboardAnalyticsService analyticsService,
        IDashboardCacheService cacheService,
        ILogger<DashboardAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive user analytics
    /// </summary>
    [HttpGet("user")]
    public async Task<IActionResult> GetUserAnalytics([FromQuery] int days = 30)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var analytics = await _analyticsService.GetUserAnalyticsAsync(userId, days);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get user behavior insights
    /// </summary>
    [HttpGet("user/behavior")]
    public async Task<IActionResult> GetUserBehaviorInsights()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var insights = await _analyticsService.GetUserBehaviorInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user behavior insights");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate personalized recommendations
    /// </summary>
    [HttpGet("user/recommendations")]
    public async Task<IActionResult> GetPersonalizedRecommendations()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var recommendations = await _analyticsService.GenerateRecommendationsAsync(userId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating personalized recommendations");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get automated insights and anomaly detection
    /// </summary>
    [HttpGet("user/insights")]
    public async Task<IActionResult> GetAutomatedInsights()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var insights = await _analyticsService.GenerateAutomatedInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating automated insights");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track user session for analytics
    /// </summary>
    [HttpPost("track/session")]
    public async Task<IActionResult> TrackSession([FromBody] SessionTrackingRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _analyticsService.TrackSessionAsync(userId, request.Duration, request.Actions);
            return Ok(new { success = true, message = "Session tracked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking user session");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track widget usage
    /// </summary>
    [HttpPost("track/widget")]
    public async Task<IActionResult> TrackWidgetUsage([FromBody] WidgetTrackingRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _analyticsService.TrackWidgetUsageAsync(userId, request.WidgetType, request.Action, request.Metadata);
            return Ok(new { success = true, message = "Widget usage tracked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking widget usage");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track feature usage
    /// </summary>
    [HttpPost("track/feature")]
    public async Task<IActionResult> TrackFeatureUsage([FromBody] FeatureTrackingRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _analyticsService.TrackFeatureUsageAsync(userId, request.FeatureName, request.Context, request.Success);
            return Ok(new { success = true, message = "Feature usage tracked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking feature usage");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track booking funnel progression
    /// </summary>
    [HttpPost("track/booking-funnel")]
    public async Task<IActionResult> TrackBookingFunnel([FromBody] BookingFunnelTrackingRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _analyticsService.TrackBookingFunnelAsync(userId, request.ProviderId, request.Step, request.Completed, request.Metadata);
            return Ok(new { success = true, message = "Booking funnel tracked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking booking funnel");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get dashboard performance metrics (Admin only)
    /// </summary>
    [HttpGet("performance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPerformanceMetrics([FromQuery] int days = 7)
    {
        try
        {
            var metrics = await _analyticsService.GetPerformanceMetricsAsync(days);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance metrics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get system-wide analytics (Admin only)
    /// </summary>
    [HttpGet("system")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSystemAnalytics([FromQuery] int days = 7)
    {
        try
        {
            var analytics = await _analyticsService.GetSystemAnalyticsAsync(days);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get cache statistics (Admin only)
    /// </summary>
    [HttpGet("cache-stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCacheStatistics()
    {
        try
        {
            var stats = await _cacheService.GetCacheStatisticsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache statistics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clear user cache (for debugging)
    /// </summary>
    [HttpDelete("cache/user")]
    public async Task<IActionResult> ClearUserCache()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _cacheService.InvalidateUserCacheAsync(userId);
            return Ok(new { success = true, message = "User cache cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing user cache");
            return StatusCode(500, "Internal server error");
        }
    }
}

#region Request DTOs

/// <summary>
/// Session tracking request
/// </summary>
public class SessionTrackingRequest
{
    public TimeSpan Duration { get; set; }
    public Dictionary<string, int> Actions { get; set; } = new();
}

/// <summary>
/// Widget tracking request
/// </summary>
public class WidgetTrackingRequest
{
    public string WidgetType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Feature tracking request
/// </summary>
public class FeatureTrackingRequest
{
    public string FeatureName { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}

/// <summary>
/// Booking funnel tracking request
/// </summary>
public class BookingFunnelTrackingRequest
{
    public string ProviderId { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

#endregion