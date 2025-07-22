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
/// Unit tests for CalendarService
/// </summary>
public class CalendarServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CalendarService _calendarService;
    private readonly Mock<ILogger<CalendarService>> _mockLogger;

    public CalendarServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<CalendarService>>();
        _calendarService = new CalendarService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add test users would normally come from Identity system
        // For testing, we'll assume users exist
    }

    [Fact]
    public async Task CreateAppointmentAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            Title = "Dog Grooming Appointment",
            Description = "Monthly grooming session",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            Location = "Pet Grooming Salon",
            AppointmentType = AppointmentType.Service,
            ParticipantEmails = new List<string> { "user2@example.com" }
        };

        // Act
        var result = await _calendarService.CreateAppointmentAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Appointment);
        Assert.Equal("Dog Grooming Appointment", result.Appointment.Title);
        Assert.Equal("user1", result.Appointment.CreatedBy);
        Assert.Equal(AppointmentType.Service, result.Appointment.AppointmentType);

        // Verify appointment was saved to database
        var savedAppointment = await _context.CalendarAppointments
            .Include(a => a.Participants)
            .FirstOrDefaultAsync(a => a.Id == result.Appointment.Id);
        Assert.NotNull(savedAppointment);
        Assert.Equal("Dog Grooming Appointment", savedAppointment.Title);
        Assert.Single(savedAppointment.Participants); // Creator + 1 participant
    }

    [Fact]
    public async Task CreateAppointmentAsync_RecurringAppointment_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            Title = "Weekly Dog Walk",
            Description = "Regular dog walking service",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            Location = "Local Park",
            AppointmentType = AppointmentType.Service,
            IsRecurring = true,
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
            RecurrenceEndDate = DateTimeOffset.UtcNow.AddMonths(3)
        };

        // Act
        var result = await _calendarService.CreateAppointmentAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Appointment.IsRecurring);
        Assert.Equal("FREQ=WEEKLY;BYDAY=MO,WE,FR", result.Appointment.RecurrencePattern);
        Assert.NotNull(result.Appointment.RecurrenceEndDate);
    }

    [Fact]
    public async Task CreateAppointmentAsync_InvalidTimeRange_ReturnsFailed()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            Title = "Invalid Appointment",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(-1), // End before start
            AppointmentType = AppointmentType.Service
        };

        // Act
        var result = await _calendarService.CreateAppointmentAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("End time must be after start time", result.Errors);
    }

    [Fact]
    public async Task CreateAppointmentAsync_PastDate_ReturnsFailed()
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            Title = "Past Appointment",
            StartTime = DateTimeOffset.UtcNow.AddDays(-1), // In the past
            EndTime = DateTimeOffset.UtcNow.AddDays(-1).AddHours(1),
            AppointmentType = AppointmentType.Service
        };

        // Act
        var result = await _calendarService.CreateAppointmentAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("cannot be in the past", result.Errors);
    }

    [Fact]
    public async Task UpdateAppointmentAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var request = new UpdateAppointmentRequest
        {
            Title = "Updated Dog Grooming",
            Description = "Updated description",
            Location = "New Pet Salon",
            StartTime = appointment.StartTime.AddHours(1),
            EndTime = appointment.EndTime.AddHours(1)
        };

        // Act
        var result = await _calendarService.UpdateAppointmentAsync("user1", appointment.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Updated Dog Grooming", result.Appointment.Title);
        Assert.Equal("Updated description", result.Appointment.Description);
        Assert.Equal("New Pet Salon", result.Appointment.Location);

        // Verify appointment was updated in database
        var updatedAppointment = await _context.CalendarAppointments
            .FirstOrDefaultAsync(a => a.Id == appointment.Id);
        Assert.NotNull(updatedAppointment);
        Assert.Equal("Updated Dog Grooming", updatedAppointment.Title);
    }

    [Fact]
    public async Task UpdateAppointmentAsync_UserNotCreator_ReturnsFailed()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var request = new UpdateAppointmentRequest
        {
            Title = "Unauthorized Update"
        };

        // Act
        var result = await _calendarService.UpdateAppointmentAsync("user2", appointment.Id, request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not authorized", result.Errors);
    }

    [Fact]
    public async Task GetAppointmentAsync_ValidAppointment_ReturnsAppointment()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");

        // Act
        var result = await _calendarService.GetAppointmentAsync("user1", appointment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(appointment.Id, result.Id);
        Assert.Equal(appointment.Title, result.Title);
        Assert.Equal(appointment.CreatedBy, result.CreatedBy);
    }

    [Fact]
    public async Task GetAppointmentAsync_UserNotParticipant_ReturnsNull()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");

        // Act
        var result = await _calendarService.GetAppointmentAsync("user3", appointment.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserAppointmentsAsync_ValidRequest_ReturnsAppointments()
    {
        // Arrange
        await CreateTestAppointment("user1", "Appointment 1");
        await CreateTestAppointment("user1", "Appointment 2");
        await CreateTestAppointment("user2", "Appointment 3");

        var fromDate = DateTimeOffset.UtcNow;
        var toDate = DateTimeOffset.UtcNow.AddDays(30);

        // Act
        var result = await _calendarService.GetUserAppointmentsAsync("user1", fromDate, toDate);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count >= 2);
        Assert.All(result, a => Assert.Equal("user1", a.CreatedBy));
    }

    [Fact]
    public async Task DeleteAppointmentAsync_ValidAppointment_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");

        // Act
        var result = await _calendarService.DeleteAppointmentAsync("user1", appointment.Id);

        // Assert
        Assert.True(result);

        // Verify appointment was marked as deleted
        var deletedAppointment = await _context.CalendarAppointments
            .FirstOrDefaultAsync(a => a.Id == appointment.Id);
        Assert.NotNull(deletedAppointment);
        Assert.True(deletedAppointment.IsDeleted);
        Assert.NotNull(deletedAppointment.DeletedAt);
    }

    [Fact]
    public async Task AddAppointmentParticipantAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var request = new AddParticipantRequest
        {
            Email = "newuser@example.com",
            Role = ParticipantRole.Attendee
        };

        // Act
        var result = await _calendarService.AddAppointmentParticipantAsync("user1", appointment.Id, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Participant);
        Assert.Equal("newuser@example.com", result.Participant.Email);
        Assert.Equal(ParticipantRole.Attendee, result.Participant.Role);

        // Verify participant was added to database
        var participant = await _context.AppointmentParticipants
            .FirstOrDefaultAsync(p => p.AppointmentId == appointment.Id && p.Email == "newuser@example.com");
        Assert.NotNull(participant);
    }

    [Fact]
    public async Task RemoveAppointmentParticipantAsync_ValidParticipant_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var addRequest = new AddParticipantRequest
        {
            Email = "participant@example.com",
            Role = ParticipantRole.Attendee
        };
        var addResult = await _calendarService.AddAppointmentParticipantAsync("user1", appointment.Id, addRequest);

        // Act
        var result = await _calendarService.RemoveAppointmentParticipantAsync("user1", appointment.Id, addResult.Participant.Id);

        // Assert
        Assert.True(result);

        // Verify participant was removed from database
        var participant = await _context.AppointmentParticipants
            .FirstOrDefaultAsync(p => p.Id == addResult.Participant.Id);
        Assert.Null(participant);
    }

    [Fact]
    public async Task CreateReminderAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var request = new CreateReminderRequest
        {
            AppointmentId = appointment.Id,
            ReminderTime = appointment.StartTime.AddHours(-1),
            ReminderType = ReminderType.Email,
            Message = "Don't forget your dog grooming appointment!"
        };

        // Act
        var result = await _calendarService.CreateReminderAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Reminder);
        Assert.Equal(appointment.Id, result.Reminder.AppointmentId);
        Assert.Equal(ReminderType.Email, result.Reminder.ReminderType);

        // Verify reminder was saved to database
        var reminder = await _context.AppointmentReminders
            .FirstOrDefaultAsync(r => r.Id == result.Reminder.Id);
        Assert.NotNull(reminder);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_AvailableTime_ReturnsAvailable()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(10);
        var endTime = startTime.AddHours(1);

        // Act
        var result = await _calendarService.CheckAvailabilityAsync("user1", startTime, endTime);

        // Assert
        Assert.True(result.IsAvailable);
        Assert.Empty(result.ConflictingAppointments);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_ConflictingAppointment_ReturnsUnavailable()
    {
        // Arrange
        var existingAppointment = await CreateTestAppointment("user1");
        var startTime = existingAppointment.StartTime.AddMinutes(30); // Overlaps with existing
        var endTime = startTime.AddHours(1);

        // Act
        var result = await _calendarService.CheckAvailabilityAsync("user1", startTime, endTime);

        // Assert
        Assert.False(result.IsAvailable);
        Assert.NotEmpty(result.ConflictingAppointments);
        Assert.Contains(result.ConflictingAppointments, a => a.Id == existingAppointment.Id);
    }

    [Fact]
    public async Task GetRecurringInstancesAsync_RecurringAppointment_ReturnsInstances()
    {
        // Arrange
        var appointment = await CreateTestRecurringAppointment("user1");
        var fromDate = DateTimeOffset.UtcNow;
        var toDate = DateTimeOffset.UtcNow.AddDays(30);

        // Act
        var result = await _calendarService.GetRecurringInstancesAsync("user1", appointment.Id, fromDate, toDate);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Instances);
        Assert.All(result.Instances, i => Assert.Equal(appointment.Id, i.ParentAppointmentId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateAppointmentAsync_EmptyTitle_ReturnsFailed(string title)
    {
        // Arrange
        var request = new CreateAppointmentRequest
        {
            Title = title,
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            AppointmentType = AppointmentType.Service
        };

        // Act
        var result = await _calendarService.CreateAppointmentAsync("user1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Title is required", result.Errors);
    }

    [Fact]
    public async Task SyncWithExternalCalendarAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var appointment = await CreateTestAppointment("user1");
        var request = new SyncCalendarRequest
        {
            AppointmentId = appointment.Id,
            ExternalCalendarType = ExternalCalendarType.Google,
            AccessToken = "fake-access-token"
        };

        // Act
        var result = await _calendarService.SyncWithExternalCalendarAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.SyncResult);
        Assert.Equal(appointment.Id, result.SyncResult.AppointmentId);

        // Verify calendar integration was saved
        var integration = await _context.CalendarIntegrations
            .FirstOrDefaultAsync(i => i.AppointmentId == appointment.Id);
        Assert.NotNull(integration);
        Assert.Equal("Google", integration.ExternalCalendarType);
    }

    private async Task<CalendarAppointment> CreateTestAppointment(string userId, string title = "Test Appointment")
    {
        var appointment = new CalendarAppointment
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Description = "Test appointment description",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            Location = "Test Location",
            AppointmentType = "Service",
            CreatedBy = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.CalendarAppointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    private async Task<CalendarAppointment> CreateTestRecurringAppointment(string userId)
    {
        var appointment = new CalendarAppointment
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Recurring Test Appointment",
            Description = "Weekly recurring appointment",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
            Location = "Test Location",
            AppointmentType = "Service",
            IsRecurring = true,
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO",
            RecurrenceEndDate = DateTimeOffset.UtcNow.AddMonths(3),
            CreatedBy = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.CalendarAppointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}