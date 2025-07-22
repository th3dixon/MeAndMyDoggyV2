using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for push notification operations
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Register a device for push notifications
    /// </summary>
    /// <param name="userId">User ID owning the device</param>
    /// <param name="request">Device registration details</param>
    /// <returns>Registered device information</returns>
    Task<NotificationDeviceDto> RegisterDeviceAsync(string userId, RegisterDeviceRequest request);

    /// <summary>
    /// Unregister a device from push notifications
    /// </summary>
    /// <param name="userId">User ID owning the device</param>
    /// <param name="deviceId">Device ID to unregister</param>
    /// <returns>True if successfully unregistered</returns>
    Task<bool> UnregisterDeviceAsync(string userId, string deviceId);

    /// <summary>
    /// Update device information
    /// </summary>
    /// <param name="userId">User ID owning the device</param>
    /// <param name="deviceId">Device ID to update</param>
    /// <param name="request">Updated device information</param>
    /// <returns>Updated device information</returns>
    Task<NotificationDeviceDto?> UpdateDeviceAsync(string userId, string deviceId, RegisterDeviceRequest request);

    /// <summary>
    /// Get all devices for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user devices</returns>
    Task<List<NotificationDeviceDto>> GetUserDevicesAsync(string userId);

    /// <summary>
    /// Send a push notification
    /// </summary>
    /// <param name="request">Notification details</param>
    /// <param name="createdBy">Who is creating the notification</param>
    /// <returns>Notification response with delivery status</returns>
    Task<NotificationResponse> SendNotificationAsync(SendPushNotificationRequest request, string? createdBy = null);

    /// <summary>
    /// Send a notification to specific users
    /// </summary>
    /// <param name="userIds">List of user IDs to notify</param>
    /// <param name="request">Notification details</param>
    /// <param name="createdBy">Who is creating the notification</param>
    /// <returns>Notification response with delivery status</returns>
    Task<NotificationResponse> SendNotificationToUsersAsync(List<string> userIds, SendPushNotificationRequest request, string? createdBy = null);

    /// <summary>
    /// Send a broadcast notification to all active users
    /// </summary>
    /// <param name="request">Notification details</param>
    /// <param name="createdBy">Who is creating the notification</param>
    /// <returns>Notification response with delivery status</returns>
    Task<NotificationResponse> SendBroadcastNotificationAsync(SendPushNotificationRequest request, string? createdBy = null);

    /// <summary>
    /// Get notification by ID
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Notification details or null if not found</returns>
    Task<PushNotificationDto?> GetNotificationAsync(string notificationId);

    /// <summary>
    /// Get notifications for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of user notifications</returns>
    Task<List<PushNotificationDto>> GetUserNotificationsAsync(string userId, int skip = 0, int take = 50);

    /// <summary>
    /// Mark notification as opened
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="deviceId">Device ID that opened the notification</param>
    /// <returns>True if successfully marked as opened</returns>
    Task<bool> MarkNotificationOpenedAsync(string notificationId, string deviceId);

    /// <summary>
    /// Cancel a pending notification
    /// </summary>
    /// <param name="notificationId">Notification ID to cancel</param>
    /// <returns>True if successfully cancelled</returns>
    Task<bool> CancelNotificationAsync(string notificationId);

    /// <summary>
    /// Get notification preferences for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of notification preferences</returns>
    Task<List<NotificationPreferenceDto>> GetNotificationPreferencesAsync(string userId);

    /// <summary>
    /// Update notification preferences for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Updated preferences</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> UpdateNotificationPreferencesAsync(string userId, UpdateNotificationPreferencesRequest request);

    /// <summary>
    /// Check if user allows notifications of a specific type
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="notificationType">Type of notification</param>
    /// <returns>True if notifications are allowed</returns>
    Task<bool> IsNotificationAllowedAsync(string userId, NotificationType notificationType);

    /// <summary>
    /// Process scheduled notifications (called by background service)
    /// </summary>
    /// <returns>Number of notifications processed</returns>
    Task<int> ProcessScheduledNotificationsAsync();

    /// <summary>
    /// Retry failed notifications (called by background service)
    /// </summary>
    /// <returns>Number of notifications retried</returns>
    Task<int> RetryFailedNotificationsAsync();

    /// <summary>
    /// Clean up expired notifications and devices
    /// </summary>
    /// <returns>Number of items cleaned up</returns>
    Task<int> CleanupExpiredDataAsync();

    /// <summary>
    /// Get notification statistics for admin dashboard
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Notification statistics</returns>
    Task<NotificationStatistics> GetNotificationStatisticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate);
}