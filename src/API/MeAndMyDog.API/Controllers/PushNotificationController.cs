using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing push notifications and user devices
/// </summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class PushNotificationController : ControllerBase
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<PushNotificationController> _logger;

    /// <summary>
    /// Initializes a new instance of PushNotificationController
    /// </summary>
    public PushNotificationController(
        IPushNotificationService pushNotificationService,
        ILogger<PushNotificationController> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    /// <summary>
    /// Register a device for push notifications
    /// </summary>
    /// <param name="request">Device registration details</param>
    /// <returns>Registered device information</returns>
    [HttpPost("devices/register")]
    [ProducesResponseType(typeof(NotificationDeviceDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var device = await _pushNotificationService.RegisterDeviceAsync(userId, request);

            _logger.LogInformation("Device registered successfully for user {UserId}", userId);
            return CreatedAtAction(nameof(GetUserDevices), new { }, device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device");
            return StatusCode(500, "An error occurred while registering the device");
        }
    }

    /// <summary>
    /// Unregister a device from push notifications
    /// </summary>
    /// <param name="deviceId">Device ID to unregister</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("devices/{deviceId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UnregisterDevice(string deviceId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _pushNotificationService.UnregisterDeviceAsync(userId, deviceId);
            if (!success)
            {
                return NotFound("Device not found");
            }

            _logger.LogInformation("Device {DeviceId} unregistered for user {UserId}", deviceId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering device {DeviceId}", deviceId);
            return StatusCode(500, "An error occurred while unregistering the device");
        }
    }

    /// <summary>
    /// Update device information
    /// </summary>
    /// <param name="deviceId">Device ID to update</param>
    /// <param name="request">Updated device information</param>
    /// <returns>Updated device information</returns>
    [HttpPut("devices/{deviceId}")]
    [ProducesResponseType(typeof(NotificationDeviceDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateDevice(string deviceId, [FromBody] RegisterDeviceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var device = await _pushNotificationService.UpdateDeviceAsync(userId, deviceId, request);
            if (device == null)
            {
                return NotFound("Device not found");
            }

            _logger.LogInformation("Device {DeviceId} updated for user {UserId}", deviceId, userId);
            return Ok(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {DeviceId}", deviceId);
            return StatusCode(500, "An error occurred while updating the device");
        }
    }

    /// <summary>
    /// Get all devices for the current user
    /// </summary>
    /// <returns>List of user devices</returns>
    [HttpGet("devices")]
    [ProducesResponseType(typeof(List<NotificationDeviceDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserDevices()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var devices = await _pushNotificationService.GetUserDevicesAsync(userId);
            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices for user");
            return StatusCode(500, "An error occurred while getting devices");
        }
    }

    /// <summary>
    /// Send a push notification
    /// </summary>
    /// <param name="request">Notification details</param>
    /// <returns>Notification response with delivery status</returns>
    [HttpPost("send")]
    [ProducesResponseType(typeof(NotificationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin,ServiceProvider")]
    public async Task<IActionResult> SendNotification([FromBody] SendPushNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _pushNotificationService.SendNotificationAsync(request, userId);

            if (response.Success)
            {
                _logger.LogInformation("Notification sent successfully by user {UserId}", userId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Failed to send notification: {Message}", response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return StatusCode(500, "An error occurred while sending the notification");
        }
    }

    /// <summary>
    /// Send notification to multiple users
    /// </summary>
    /// <param name="request">Notification details with target user IDs</param>
    /// <returns>Notification response with delivery status</returns>
    [HttpPost("send/users")]
    [ProducesResponseType(typeof(NotificationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendNotificationToUsers([FromBody] SendNotificationToUsersRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var notificationRequest = new SendPushNotificationRequest
            {
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

            var response = await _pushNotificationService.SendNotificationToUsersAsync(
                request.TargetUserIds, notificationRequest, userId);

            _logger.LogInformation("Notification sent to {UserCount} users by user {UserId}", 
                request.TargetUserIds.Count, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to users");
            return StatusCode(500, "An error occurred while sending the notification");
        }
    }

    /// <summary>
    /// Send broadcast notification to all users
    /// </summary>
    /// <param name="request">Broadcast notification details</param>
    /// <returns>Notification response with delivery status</returns>
    [HttpPost("send/broadcast")]
    [ProducesResponseType(typeof(NotificationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendBroadcastNotification([FromBody] SendPushNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _pushNotificationService.SendBroadcastNotificationAsync(request, userId);

            _logger.LogInformation("Broadcast notification sent by user {UserId}", userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending broadcast notification");
            return StatusCode(500, "An error occurred while sending the broadcast notification");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Notification details</returns>
    [HttpGet("{notificationId}")]
    [ProducesResponseType(typeof(PushNotificationDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetNotification(string notificationId)
    {
        try
        {
            var notification = await _pushNotificationService.GetNotificationAsync(notificationId);
            if (notification == null)
            {
                return NotFound("Notification not found");
            }

            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
            return StatusCode(500, "An error occurred while getting the notification");
        }
    }

    /// <summary>
    /// Get notifications for the current user
    /// </summary>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of user notifications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PushNotificationDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination parameters
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            var notifications = await _pushNotificationService.GetUserNotificationsAsync(userId, skip, take);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user");
            return StatusCode(500, "An error occurred while getting notifications");
        }
    }

    /// <summary>
    /// Mark notification as opened
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="request">Device ID that opened the notification</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{notificationId}/opened")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkNotificationOpened(string notificationId, [FromBody] MarkOpenedRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _pushNotificationService.MarkNotificationOpenedAsync(notificationId, request.DeviceId);
            if (!success)
            {
                return NotFound("Notification or device not found");
            }

            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as opened", notificationId);
            return StatusCode(500, "An error occurred while marking the notification as opened");
        }
    }

    /// <summary>
    /// Cancel a pending notification
    /// </summary>
    /// <param name="notificationId">Notification ID to cancel</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{notificationId}/cancel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CancelNotification(string notificationId)
    {
        try
        {
            var success = await _pushNotificationService.CancelNotificationAsync(notificationId);
            if (!success)
            {
                return NotFound("Notification not found or cannot be cancelled");
            }

            _logger.LogInformation("Notification {NotificationId} cancelled", notificationId);
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling notification {NotificationId}", notificationId);
            return StatusCode(500, "An error occurred while cancelling the notification");
        }
    }

    /// <summary>
    /// Get notification preferences for the current user
    /// </summary>
    /// <returns>List of notification preferences</returns>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(List<NotificationPreferenceDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetNotificationPreferences()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var preferences = await _pushNotificationService.GetNotificationPreferencesAsync(userId);
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification preferences for user");
            return StatusCode(500, "An error occurred while getting notification preferences");
        }
    }

    /// <summary>
    /// Update notification preferences for the current user
    /// </summary>
    /// <param name="request">Updated preferences</param>
    /// <returns>Success confirmation</returns>
    [HttpPut("preferences")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferencesRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _pushNotificationService.UpdateNotificationPreferencesAsync(userId, request);
            if (!success)
            {
                return BadRequest("Failed to update notification preferences");
            }

            _logger.LogInformation("Notification preferences updated for user {UserId}", userId);
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences for user");
            return StatusCode(500, "An error occurred while updating notification preferences");
        }
    }

    /// <summary>
    /// Get notification statistics (admin only)
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Notification statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(NotificationStatistics), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetNotificationStatistics([FromQuery] DateTimeOffset? fromDate, [FromQuery] DateTimeOffset? toDate)
    {
        try
        {
            // Default to last 30 days if dates not provided
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            if (fromDate > toDate)
            {
                return BadRequest("From date cannot be after to date");
            }

            var statistics = await _pushNotificationService.GetNotificationStatisticsAsync(fromDate.Value, toDate.Value);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification statistics");
            return StatusCode(500, "An error occurred while getting notification statistics");
        }
    }
}