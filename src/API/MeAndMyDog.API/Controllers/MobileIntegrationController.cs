using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Models.DTOs.Mobile;
using MeAndMyDog.API.Models.DTOs.MobileIntegration;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Mobile integration controller providing comprehensive mobile app support
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MobileIntegrationController : ControllerBase
{
    private readonly IMobileIntegrationService _mobileService;
    private readonly ILogger<MobileIntegrationController> _logger;

    public MobileIntegrationController(
        IMobileIntegrationService mobileService,
        ILogger<MobileIntegrationController> logger)
    {
        _mobileService = mobileService;
        _logger = logger;
    }

    /// <summary>
    /// Register mobile device for push notifications
    /// </summary>
    [HttpPost("device/register")]
    public async Task<IActionResult> RegisterDevice([FromBody] MobileDeviceRegistrationDto registration)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mobileService.RegisterDeviceAsync(userId, registration);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering mobile device");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get mobile-optimized dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetMobileDashboard([FromQuery] string? theme = null, [FromQuery] int maxItems = 5)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var preferences = new MobileViewPreferences
            {
                Theme = theme ?? "system",
                MaxItemsPerWidget = maxItems
            };

            var dashboard = await _mobileService.GetMobileDashboardAsync(userId, preferences);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mobile dashboard");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send push notification to user's devices
    /// </summary>
    [HttpPost("notification/send")]
    [Authorize(Roles = "Admin,ServiceProvider")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        try
        {
            var result = await _mobileService.SendNotificationAsync(request.UserId, request.Notification);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send bulk notifications to multiple users
    /// </summary>
    [HttpPost("notification/bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendBulkNotifications([FromBody] BulkNotificationRequest request)
    {
        try
        {
            var result = await _mobileService.SendBulkNotificationsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification preferences
    /// </summary>
    [HttpGet("notification/preferences")]
    public async Task<IActionResult> GetNotificationPreferences()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var preferences = await _mobileService.GetNotificationPreferencesAsync(userId);
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update notification preferences
    /// </summary>
    [HttpPut("notification/preferences")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] NotificationPreferencesDto preferences)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _mobileService.UpdateNotificationPreferencesAsync(userId, preferences);
            
            if (success)
            {
                return Ok(new { success = true, message = "Preferences updated successfully" });
            }
            
            return BadRequest(new { success = false, message = "Failed to update preferences" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Sync offline data
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncOfflineData([FromBody] OfflineSyncRequest syncRequest)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mobileService.SyncOfflineDataAsync(userId, syncRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing offline data");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get real-time updates
    /// </summary>
    [HttpGet("updates")]
    public async Task<IActionResult> GetRealtimeUpdates([FromQuery] DateTime? since = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lastUpdateTime = since ?? DateTime.UtcNow.AddHours(-1);
            var updates = await _mobileService.GetRealtimeUpdatesAsync(userId, lastUpdateTime);
            return Ok(updates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving realtime updates");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Track mobile analytics event
    /// </summary>
    [HttpPost("analytics/track")]
    public async Task<IActionResult> TrackAnalyticsEvent([FromBody] MobileAnalyticsEventDto analyticsEvent)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _mobileService.TrackMobileAnalyticsAsync(userId, analyticsEvent);
            return Ok(new { success = true, message = "Analytics event tracked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking mobile analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate mobile API token
    /// </summary>
    [HttpPost("auth/token")]
    public async Task<IActionResult> GenerateApiToken([FromBody] TokenGenerationRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var expiry = request.ExpiryDays > 0 ? TimeSpan.FromDays(request.ExpiryDays) : TimeSpan.FromDays(30);
            var token = await _mobileService.GenerateMobileApiTokenAsync(userId, request.DeviceId, expiry);
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating mobile API token");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get mobile app configuration
    /// </summary>
    [HttpGet("config")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAppConfig([FromQuery] string version = "1.0.0", [FromQuery] string platform = "iOS")
    {
        try
        {
            var config = await _mobileService.GetMobileAppConfigAsync(version, platform);
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mobile app config");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Process deep link
    /// </summary>
    [HttpPost("deeplink")]
    public async Task<IActionResult> ProcessDeepLink([FromBody] DeepLinkRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _mobileService.ProcessDeepLinkAsync(userId, request.DeepLink);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing deep link");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get nearby services based on location
    /// </summary>
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyServices([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] int radius = 5)
    {
        try
        {
            var services = await _mobileService.GetNearbyServicesAsync(latitude, longitude, radius);
            return Ok(services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nearby services");
            return StatusCode(500, "Internal server error");
        }
    }
}