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
/// Unit tests for AdvancedSecurityService
/// </summary>
public class AdvancedSecurityServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AdvancedSecurityService _securityService;
    private readonly Mock<ILogger<AdvancedSecurityService>> _mockLogger;

    public AdvancedSecurityServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<AdvancedSecurityService>>();
        _securityService = new AdvancedSecurityService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var message = new Message
        {
            Id = "test-message-1",
            ConversationId = "test-conversation-1",
            SenderId = "user1",
            Content = "Test message content",
            MessageType = "Text",
            SentAt = DateTimeOffset.UtcNow
        };

        _context.Messages.Add(message);
        _context.SaveChanges();
    }

    [Fact]
    public async Task ConfigureSelfDestructAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ConfigureSelfDestructRequest
        {
            MessageId = "test-message-1",
            DestructMode = SelfDestructMode.Timer,
            TimerSeconds = 300,
            MaxViews = null
        };

        // Act
        var result = await _securityService.ConfigureSelfDestructAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.SelfDestruct);
        Assert.Equal("test-message-1", result.SelfDestruct.MessageId);
        Assert.Equal(SelfDestructMode.Timer, result.SelfDestruct.DestructMode);
        Assert.Equal(300, result.SelfDestruct.TimerSeconds);

        // Verify self-destruct was saved to database
        var savedSelfDestruct = await _context.SelfDestructMessages
            .FirstOrDefaultAsync(s => s.MessageId == "test-message-1");
        Assert.NotNull(savedSelfDestruct);
        Assert.Equal("Timer", savedSelfDestruct.DestructMode);
    }

    [Fact]
    public async Task ConfigureSelfDestructAsync_ViewBasedMode_ReturnsSuccess()
    {
        // Arrange
        var request = new ConfigureSelfDestructRequest
        {
            MessageId = "test-message-1",
            DestructMode = SelfDestructMode.ViewBased,
            TimerSeconds = 0,
            MaxViews = 3
        };

        // Act
        var result = await _securityService.ConfigureSelfDestructAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(SelfDestructMode.ViewBased, result.SelfDestruct.DestructMode);
        Assert.Equal(3, result.SelfDestruct.MaxViews);
    }

    [Fact]
    public async Task TriggerSelfDestructAsync_ValidMessage_ReturnsSuccess()
    {
        // Arrange
        await ConfigureSelfDestructForMessage("test-message-1");

        // Act
        var result = await _securityService.TriggerSelfDestructAsync("user1", "test-message-1", "Manual trigger");

        // Assert
        Assert.True(result);

        // Verify message was destroyed
        var selfDestruct = await _context.SelfDestructMessages
            .FirstOrDefaultAsync(s => s.MessageId == "test-message-1");
        Assert.NotNull(selfDestruct);
        Assert.True(selfDestruct.IsDestroyed);
        Assert.NotNull(selfDestruct.DestroyedAt);
    }

    [Fact]
    public async Task TrackMessageViewAsync_ValidRequest_ReturnsTrackingInfo()
    {
        // Arrange
        await ConfigureSelfDestructForMessage("test-message-1", SelfDestructMode.ViewBased, maxViews: 3);
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "192.168.1.1",
            UserAgent = "Test Browser",
            DeviceType = "Desktop"
        };

        // Act
        var result = await _securityService.TrackMessageViewAsync("user2", "test-message-1", 5000, clientInfo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user2", result.UserId);
        Assert.Equal("test-message-1", result.MessageId);
        Assert.Equal(5000, result.ViewDurationMs);

        // Verify view tracking was saved
        var viewTracking = await _context.MessageViewTrackings
            .FirstOrDefaultAsync(v => v.MessageId == "test-message-1" && v.UserId == "user2");
        Assert.NotNull(viewTracking);
    }

    [Fact]
    public async Task TrackMessageViewAsync_ExceedsMaxViews_TriggersDestruction()
    {
        // Arrange
        await ConfigureSelfDestructForMessage("test-message-1", SelfDestructMode.ViewBased, maxViews: 2);
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "192.168.1.1",
            UserAgent = "Test Browser",
            DeviceType = "Desktop"
        };

        // First two views
        await _securityService.TrackMessageViewAsync("user2", "test-message-1", 1000, clientInfo);
        await _securityService.TrackMessageViewAsync("user3", "test-message-1", 1000, clientInfo);

        // Act - Third view should trigger destruction
        var result = await _securityService.TrackMessageViewAsync("user4", "test-message-1", 1000, clientInfo);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TriggeredDestruction);

        // Verify message was destroyed
        var selfDestruct = await _context.SelfDestructMessages
            .FirstOrDefaultAsync(s => s.MessageId == "test-message-1");
        Assert.NotNull(selfDestruct);
        Assert.True(selfDestruct.IsDestroyed);
    }

    [Fact]
    public async Task ConfigureMessageSecurityAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ConfigureMessageSecurityRequest
        {
            MessageId = "test-message-1",
            SecurityLevel = SecurityLevel.High,
            RequiresAuthentication = true,
            BlockScreenshots = true,
            GeographicRestrictions = new List<string> { "US", "CA" },
            AccessExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var result = await _securityService.ConfigureMessageSecurityAsync("user1", request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Security);
        Assert.Equal("test-message-1", result.Security.MessageId);
        Assert.Equal(SecurityLevel.High, result.Security.SecurityLevel);
        Assert.True(result.Security.RequiresAuthentication);
        Assert.True(result.Security.BlockScreenshots);

        // Verify security configuration was saved
        var security = await _context.MessageSecurities
            .FirstOrDefaultAsync(s => s.MessageId == "test-message-1");
        Assert.NotNull(security);
        Assert.Equal("High", security.SecurityLevel);
    }

    [Fact]
    public async Task ValidateMessageAccessAsync_ValidAccess_ReturnsGranted()
    {
        // Arrange
        await ConfigureMessageSecurity("test-message-1", SecurityLevel.Medium, requiresAuth: false);
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "192.168.1.1",
            UserAgent = "Test Browser",
            DeviceType = "Desktop",
            Location = "US"
        };

        // Act
        var result = await _securityService.ValidateMessageAccessAsync("user2", "test-message-1", clientInfo);

        // Assert
        Assert.True(result.AccessGranted);
        Assert.Null(result.DenialReason);
        Assert.False(result.RequiresVerification);
        Assert.True(result.RiskScore >= 0 && result.RiskScore <= 100);
    }

    [Fact]
    public async Task ValidateMessageAccessAsync_HighRiskAccess_RequiresVerification()
    {
        // Arrange
        await ConfigureMessageSecurity("test-message-1", SecurityLevel.Critical, requiresAuth: true);
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "1.2.3.4", // Suspicious IP
            UserAgent = "Unknown Browser",
            DeviceType = "Unknown",
            Location = "CN" // Restricted location
        };

        // Act
        var result = await _securityService.ValidateMessageAccessAsync("user2", "test-message-1", clientInfo);

        // Assert
        Assert.False(result.AccessGranted);
        Assert.True(result.RequiresVerification);
        Assert.NotNull(result.DenialReason);
        Assert.True(result.RiskScore > 50);
    }

    [Fact]
    public async Task CreateSecurityIncidentAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateSecurityIncidentRequest
        {
            IncidentType = SecurityIncidentType.UnauthorizedAccess,
            Severity = IncidentSeverity.High,
            Description = "Suspicious access attempt detected",
            AffectedUserId = "user1",
            RelatedMessageId = "test-message-1",
            IpAddress = "1.2.3.4",
            UserAgent = "Suspicious Browser"
        };

        // Act
        var result = await _securityService.CreateSecurityIncidentAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Incident);
        Assert.Equal(SecurityIncidentType.UnauthorizedAccess, result.Incident.IncidentType);
        Assert.Equal(IncidentSeverity.High, result.Incident.Severity);
        Assert.Equal("user1", result.Incident.AffectedUserId);

        // Verify incident was saved to database
        var incident = await _context.SecurityIncidents
            .FirstOrDefaultAsync(i => i.Id == result.Incident.Id);
        Assert.NotNull(incident);
        Assert.Equal("UnauthorizedAccess", incident.IncidentType);
    }

    [Fact]
    public async Task LogMessageAccessAsync_ValidRequest_ReturnsAccessLog()
    {
        // Arrange
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "192.168.1.1",
            UserAgent = "Test Browser",
            DeviceType = "Desktop"
        };

        // Act
        var result = await _securityService.LogMessageAccessAsync("user2", "test-message-1", "view", clientInfo, true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user2", result.UserId);
        Assert.Equal("test-message-1", result.MessageId);
        Assert.Equal("view", result.AccessType);
        Assert.True(result.AccessGranted);
        Assert.Equal("192.168.1.1", result.IpAddress);

        // Verify access log was saved
        var accessLog = await _context.MessageAccessLogs
            .FirstOrDefaultAsync(l => l.MessageId == "test-message-1" && l.UserId == "user2");
        Assert.NotNull(accessLog);
        Assert.True(accessLog.AccessGranted);
    }

    [Fact]
    public async Task CalculateAccessRiskScoreAsync_LowRiskAccess_ReturnsLowScore()
    {
        // Arrange
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
            DeviceType = "Desktop",
            Location = "US"
        };

        // Act
        var riskScore = await _securityService.CalculateAccessRiskScoreAsync("user2", "test-message-1", clientInfo);

        // Assert
        Assert.True(riskScore >= 0 && riskScore <= 100);
        Assert.True(riskScore < 30); // Should be low risk for normal access
    }

    [Fact]
    public async Task CalculateAccessRiskScoreAsync_HighRiskAccess_ReturnsHighScore()
    {
        // Arrange
        var clientInfo = new ClientInformationDto
        {
            IpAddress = "1.2.3.4", // Suspicious IP
            UserAgent = "curl/7.68.0", // Automated tool
            DeviceType = "Unknown",
            Location = "TOR" // Tor network
        };

        // Act
        var riskScore = await _securityService.CalculateAccessRiskScoreAsync("user2", "test-message-1", clientInfo);

        // Assert
        Assert.True(riskScore >= 0 && riskScore <= 100);
        Assert.True(riskScore > 70); // Should be high risk
    }

    [Fact]
    public async Task GenerateWatermarkAsync_ValidRequest_ReturnsWatermark()
    {
        // Arrange
        var customText = "CONFIDENTIAL - user2 - " + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm");

        // Act
        var result = await _securityService.GenerateWatermarkAsync("user2", "test-message-1", customText);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("user2", result.Text);
        Assert.Equal("bottom-right", result.Position);
        Assert.True(result.Opacity > 0 && result.Opacity <= 1);
        Assert.NotEmpty(result.Color);
    }

    [Fact]
    public async Task GetSecurityStatsAsync_WithIncidents_ReturnsStats()
    {
        // Arrange
        await CreateTestSecurityIncident("user1", SecurityIncidentType.UnauthorizedAccess, IncidentSeverity.High);
        await CreateTestSecurityIncident("user2", SecurityIncidentType.SuspiciousActivity, IncidentSeverity.Medium);
        await CreateTestSecurityIncident("user1", SecurityIncidentType.DataBreach, IncidentSeverity.Critical);

        var fromDate = DateTimeOffset.UtcNow.AddDays(-7);
        var toDate = DateTimeOffset.UtcNow;

        // Act
        var result = await _securityService.GetSecurityStatsAsync("admin-user", fromDate, toDate);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalIncidents >= 3);
        Assert.True(result.CriticalIncidents >= 1);
        Assert.Equal(fromDate.Date, result.PeriodStart.Date);
        Assert.Equal(toDate.Date, result.PeriodEnd.Date);
    }

    [Fact]
    public async Task ResolveSecurityIncidentAsync_ValidIncident_ReturnsSuccess()
    {
        // Arrange
        var incident = await CreateTestSecurityIncident("user1", SecurityIncidentType.UnauthorizedAccess, IncidentSeverity.High);

        // Act
        var result = await _securityService.ResolveSecurityIncidentAsync("admin-user", incident.Id, "False alarm - legitimate access");

        // Assert
        Assert.True(result);

        // Verify incident was resolved
        var resolvedIncident = await _context.SecurityIncidents
            .FirstOrDefaultAsync(i => i.Id == incident.Id);
        Assert.NotNull(resolvedIncident);
        Assert.Equal("Resolved", resolvedIncident.Status);
        Assert.NotNull(resolvedIncident.ResolvedAt);
        Assert.Equal("admin-user", resolvedIncident.ResolvedBy);
    }

    [Fact]
    public async Task AnalyzeUserSecurityAsync_ValidUser_ReturnsAnalysis()
    {
        // Arrange
        await CreateTestSecurityIncident("user1", SecurityIncidentType.SuspiciousActivity, IncidentSeverity.Medium);
        await LogTestMessageAccess("user1", "test-message-1", false, "Geographic restriction");

        // Act
        var result = await _securityService.AnalyzeUserSecurityAsync("user1", TimeSpan.FromDays(30));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1", result.UserId);
        Assert.True(result.SecurityIncidentCount >= 1);
        Assert.True(result.OverallRiskScore >= 0 && result.OverallRiskScore <= 100);
        Assert.NotEmpty(result.RiskLevel);
        Assert.Equal(TimeSpan.FromDays(30), result.AnalysisWindow);
    }

    private async Task ConfigureSelfDestructForMessage(string messageId, SelfDestructMode mode = SelfDestructMode.Timer, int timerSeconds = 300, int? maxViews = null)
    {
        var selfDestruct = new SelfDestructMessage
        {
            Id = Guid.NewGuid().ToString(),
            MessageId = messageId,
            DestructMode = mode.ToString(),
            TimerSeconds = timerSeconds,
            MaxViews = maxViews,
            ViewCount = 0,
            IsDestroyed = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.SelfDestructMessages.Add(selfDestruct);
        await _context.SaveChangesAsync();
    }

    private async Task ConfigureMessageSecurity(string messageId, SecurityLevel level, bool requiresAuth = false)
    {
        var security = new MessageSecurity
        {
            Id = Guid.NewGuid().ToString(),
            MessageId = messageId,
            SecurityLevel = level.ToString(),
            RequiresAuthentication = requiresAuth,
            BlockScreenshots = level >= SecurityLevel.High,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.MessageSecurities.Add(security);
        await _context.SaveChangesAsync();
    }

    private async Task<SecurityIncident> CreateTestSecurityIncident(string userId, SecurityIncidentType type, IncidentSeverity severity)
    {
        var incident = new SecurityIncident
        {
            Id = Guid.NewGuid().ToString(),
            IncidentType = type.ToString(),
            Severity = severity.ToString(),
            Status = "Open",
            AffectedUserId = userId,
            Description = $"Test {type} incident",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.SecurityIncidents.Add(incident);
        await _context.SaveChangesAsync();
        return incident;
    }

    private async Task LogTestMessageAccess(string userId, string messageId, bool granted, string? denialReason = null)
    {
        var accessLog = new MessageAccessLog
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            MessageId = messageId,
            AccessType = "view",
            AccessGranted = granted,
            DenialReason = denialReason,
            IpAddress = "192.168.1.1",
            UserAgent = "Test Browser",
            AccessedAt = DateTimeOffset.UtcNow
        };

        _context.MessageAccessLogs.Add(accessLog);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}