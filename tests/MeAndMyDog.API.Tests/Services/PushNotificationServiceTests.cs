using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Implementations;
using Moq;
using Xunit;

namespace MeAndMyDog.API.Tests.Services;

/// <summary>
/// Unit tests for PushNotificationService
/// </summary>
public class PushNotificationServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PushNotificationService _pushNotificationService;
    private readonly Mock<ILogger<PushNotificationService>> _mockLogger;

    public PushNotificationServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<PushNotificationService>>();
        _pushNotificationService = new PushNotificationService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var device = new NotificationDevice
        {
            Id = "test-device-1",
            UserId = "user1",
            DeviceToken = "test-device-token-123",
            Platform = "iOS",
            DeviceType = "Mobile",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.NotificationDevices.Add(device);
        _context.SaveChanges();
    }

    [Fact]
    public async Task RegisterDeviceAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            DeviceToken = "new-device-token-456",
            Platform = DevicePlatform.Android,
            DeviceType = DeviceType.Mobile,
            DeviceName = "Samsung Galaxy S21",
            AppVersion = "1.0.0"
        };

        // Act
        var result = await _pushNotificationService.RegisterDeviceAsync("user2", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Device);
        Assert.Equal("user2", result.Device.UserId);
        Assert.Equal("new-device-token-456", result.Device.DeviceToken);
        Assert.Equal(DevicePlatform.Android, result.Device.Platform);

        // Verify device was saved to database
        var savedDevice = await _context.NotificationDevices
            .FirstOrDefaultAsync(d => d.Id == result.Device.Id);
        Assert.NotNull(savedDevice);
        Assert.Equal("user2", savedDevice.UserId);
    }

    [Fact]
    public async Task RegisterDeviceAsync_ExistingToken_UpdatesDevice()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            DeviceToken = "test-device-token-123", // Existing token
            Platform = DevicePlatform.iOS,
            DeviceType = DeviceType.Mobile,
            DeviceName = "iPhone 13 Pro",
            AppVersion = "2.0.0"
        };

        // Act
        var result = await _pushNotificationService.RegisterDeviceAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Device);
        Assert.Equal("iPhone 13 Pro", result.Device.DeviceName);
        Assert.Equal("2.0.0", result.Device.AppVersion);

        // Verify device was updated, not duplicated
        var deviceCount = await _context.NotificationDevices
            .CountAsync(d => d.DeviceToken == "test-device-token-123");
        Assert.Equal(1, deviceCount);
    }

    [Fact]
    public async Task SendNotificationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new SendNotificationRequest
        {
            UserId = "user1",
            Title = "New Message",
            Body = "You have a new message from John",
            Type = NotificationType.Message,
            Data = new Dictionary<string, object>
            {
                ["conversationId"] = "conv-123",
                ["messageId"] = "msg-456"
            }
        };

        // Act
        var result = await _pushNotificationService.SendNotificationAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Notification);
        Assert.Equal("user1", result.Notification.UserId);
        Assert.Equal("New Message", result.Notification.Title);
        Assert.Equal(NotificationType.Message, result.Notification.Type);

        // Verify notification was saved to database
        var savedNotification = await _context.PushNotifications
            .FirstOrDefaultAsync(n => n.Id == result.Notification.Id);
        Assert.NotNull(savedNotification);
        Assert.Equal("New Message", savedNotification.Title);
    }

    [Fact]
    public async Task SendNotificationAsync_UserWithoutDevices_ReturnsFailed()
    {
        // Arrange
        var request = new SendNotificationRequest
        {
            UserId = "user-without-devices",
            Title = "Test Notification",
            Body = "Test message",
            Type = NotificationType.Message
        };

        // Act
        var result = await _pushNotificationService.SendNotificationAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No active devices", result.Message);
    }

    [Fact]
    public async Task SendBulkNotificationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        // Add another device for testing bulk notifications
        var device2 = new NotificationDevice
        {
            Id = "test-device-2",
            UserId = "user2",
            DeviceToken = "test-device-token-789",
            Platform = "Android",
            DeviceType = "Mobile",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.NotificationDevices.Add(device2);
        await _context.SaveChangesAsync();

        var request = new SendBulkNotificationRequest
        {
            UserIds = new List<string> { "user1", "user2" },
            Title = "System Announcement",
            Body = "Maintenance scheduled for tonight",
            Type = NotificationType.System,
            ScheduledFor = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        // Act
        var result = await _pushNotificationService.SendBulkNotificationAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.TotalRecipients);
        Assert.Equal(2, result.SuccessfulDeliveries);
        Assert.Empty(result.FailedDeliveries);

        // Verify notifications were saved
        var notifications = await _context.PushNotifications
            .Where(n => n.Title == "System Announcement")
            .ToListAsync();
        Assert.Equal(2, notifications.Count);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ValidUser_ReturnsNotifications()
    {
        // Arrange
        await CreateTestNotification("user1", "Test Notification 1");
        await CreateTestNotification("user1", "Test Notification 2");
        await CreateTestNotification("user2", "Test Notification 3");

        // Act
        var result = await _pushNotificationService.GetUserNotificationsAsync("user1", 1, 10);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Notifications.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Notifications, n => Assert.Equal("user1", n.UserId));
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_ValidNotification_ReturnsSuccess()
    {
        // Arrange
        var notification = await CreateTestNotification("user1", "Test Notification");

        // Act
        var result = await _pushNotificationService.MarkNotificationAsReadAsync("user1", notification.Id);

        // Assert
        Assert.True(result);

        // Verify notification was marked as read
        var updatedNotification = await _context.PushNotifications
            .FirstOrDefaultAsync(n => n.Id == notification.Id);
        Assert.NotNull(updatedNotification);
        Assert.True(updatedNotification.IsRead);
        Assert.NotNull(updatedNotification.ReadAt);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WrongUser_ReturnsFalse()
    {
        // Arrange
        var notification = await CreateTestNotification("user1", "Test Notification");

        // Act
        var result = await _pushNotificationService.MarkNotificationAsReadAsync("user2", notification.Id);

        // Assert
        Assert.False(result);

        // Verify notification was not marked as read
        var unchangedNotification = await _context.PushNotifications
            .FirstOrDefaultAsync(n => n.Id == notification.Id);
        Assert.NotNull(unchangedNotification);
        Assert.False(unchangedNotification.IsRead);
    }

    [Fact]
    public async Task DeleteNotificationAsync_ValidNotification_ReturnsSuccess()
    {
        // Arrange
        var notification = await CreateTestNotification("user1", "Test Notification");

        // Act
        var result = await _pushNotificationService.DeleteNotificationAsync("user1", notification.Id);

        // Assert
        Assert.True(result);

        // Verify notification was marked as deleted
        var deletedNotification = await _context.PushNotifications
            .FirstOrDefaultAsync(n => n.Id == notification.Id);
        Assert.NotNull(deletedNotification);
        Assert.True(deletedNotification.IsDeleted);
        Assert.NotNull(deletedNotification.DeletedAt);
    }

    [Fact]
    public async Task UpdateDeviceSettingsAsync_ValidDevice_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateDeviceSettingsRequest
        {
            NotificationsEnabled = false,
            SoundEnabled = false,
            VibrationEnabled = true,
            QuietHoursStart = TimeOnly.Parse("22:00"),
            QuietHoursEnd = TimeOnly.Parse("08:00")
        };

        // Act
        var result = await _pushNotificationService.UpdateDeviceSettingsAsync("user1", "test-device-1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Device);
        Assert.False(result.Device.NotificationsEnabled);
        Assert.False(result.Device.SoundEnabled);
        Assert.True(result.Device.VibrationEnabled);

        // Verify settings were updated in database
        var updatedDevice = await _context.NotificationDevices
            .FirstOrDefaultAsync(d => d.Id == "test-device-1");
        Assert.NotNull(updatedDevice);
        Assert.False(updatedDevice.NotificationsEnabled);
    }

    [Fact]
    public async Task UnregisterDeviceAsync_ValidDevice_ReturnsSuccess()
    {
        // Act
        var result = await _pushNotificationService.UnregisterDeviceAsync("user1", "test-device-1");

        // Assert
        Assert.True(result);

        // Verify device was marked as inactive
        var device = await _context.NotificationDevices
            .FirstOrDefaultAsync(d => d.Id == "test-device-1");
        Assert.NotNull(device);
        Assert.False(device.IsActive);
        Assert.NotNull(device.UnregisteredAt);
    }

    [Fact]
    public async Task GetUserDevicesAsync_ValidUser_ReturnsDevices()
    {
        // Arrange
        var device2 = new NotificationDevice
        {
            Id = "test-device-2",
            UserId = "user1",
            DeviceToken = "test-device-token-456",
            Platform = "Android",
            DeviceType = "Tablet",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.NotificationDevices.Add(device2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _pushNotificationService.GetUserDevicesAsync("user1");

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal("user1", d.UserId));
        Assert.All(result, d => Assert.True(d.IsActive));
    }

    [Fact]
    public async Task GetNotificationStatisticsAsync_WithData_ReturnsStats()
    {
        // Arrange
        await CreateTestNotification("user1", "Test 1", NotificationType.Message, true);
        await CreateTestNotification("user1", "Test 2", NotificationType.System, false);
        await CreateTestNotification("user1", "Test 3", NotificationType.Message, true);

        // Act
        var result = await _pushNotificationService.GetNotificationStatisticsAsync("user1", TimeSpan.FromDays(7));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalNotifications >= 3);
        Assert.True(result.ReadNotifications >= 2);
        Assert.True(result.UnreadNotifications >= 1);
        Assert.NotEmpty(result.NotificationsByType);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SendNotificationAsync_EmptyTitle_ReturnsFailed(string title)
    {
        // Arrange
        var request = new SendNotificationRequest
        {
            UserId = "user1",
            Title = title,
            Body = "Test message",
            Type = NotificationType.Message
        };

        // Act
        var result = await _pushNotificationService.SendNotificationAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("title", result.Message.ToLower());
    }

    [Fact]
    public async Task ScheduleNotificationAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ScheduleNotificationRequest
        {
            UserId = "user1",
            Title = "Scheduled Reminder",
            Body = "This is your scheduled reminder",
            Type = NotificationType.Reminder,
            ScheduledFor = DateTimeOffset.UtcNow.AddHours(1),
            Data = new Dictionary<string, object> { ["reminderId"] = "reminder-123" }
        };

        // Act
        var result = await _pushNotificationService.ScheduleNotificationAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ScheduledNotification);
        Assert.Equal("Scheduled Reminder", result.ScheduledNotification.Title);
        Assert.False(result.ScheduledNotification.IsSent);

        // Verify scheduled notification was saved
        var savedNotification = await _context.ScheduledNotifications
            .FirstOrDefaultAsync(n => n.Id == result.ScheduledNotification.Id);
        Assert.NotNull(savedNotification);
        Assert.False(savedNotification.IsSent);
    }

    private async Task<PushNotification> CreateTestNotification(string userId, string title, NotificationType type = NotificationType.Message, bool isRead = false)
    {
        var notification = new PushNotification
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Title = title,
            Body = "Test notification body",
            Type = type.ToString(),
            IsRead = isRead,
            ReadAt = isRead ? DateTimeOffset.UtcNow : null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.PushNotifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}