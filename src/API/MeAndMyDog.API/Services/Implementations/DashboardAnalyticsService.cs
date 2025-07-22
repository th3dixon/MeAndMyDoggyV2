using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs.Dashboard;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Services.Interfaces;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Advanced analytics service providing deep insights into dashboard usage and user behavior
/// </summary>
public class DashboardAnalyticsService : IDashboardAnalyticsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardAnalyticsService> _logger;
    private readonly IDashboardCacheService _cacheService;

    public DashboardAnalyticsService(
        ApplicationDbContext context,
        ILogger<DashboardAnalyticsService> logger,
        IDashboardCacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task TrackSessionAsync(string userId, TimeSpan sessionDuration, Dictionary<string, int> actionsPerformed)
    {
        try
        {
            var sessionLog = new UserSessionLog
            {
                UserId = userId,
                SessionStart = DateTime.UtcNow.Subtract(sessionDuration),
                SessionEnd = DateTime.UtcNow,
                Duration = sessionDuration,
                Actions = JsonSerializer.Serialize(actionsPerformed),
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserSessionLogs.AddAsync(sessionLog);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Tracked session for user {UserId}: {Duration}s", userId, sessionDuration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking session for user {UserId}", userId);
        }
    }

    public async Task TrackWidgetUsageAsync(string userId, string widgetType, string action, Dictionary<string, object>? metadata = null)
    {
        try
        {
            var widgetUsage = new WidgetUsageLog
            {
                UserId = userId,
                WidgetType = widgetType,
                Action = action,
                Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
                Timestamp = DateTime.UtcNow
            };

            await _context.WidgetUsageLogs.AddAsync(widgetUsage);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Tracked widget usage for user {UserId}: {Widget} - {Action}", userId, widgetType, action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking widget usage for user {UserId}", userId);
        }
    }

    public async Task TrackFeatureUsageAsync(string userId, string featureName, string context, bool success = true)
    {
        try
        {
            var featureUsage = new FeatureUsageLog
            {
                UserId = userId,
                FeatureName = featureName,
                Context = context,
                Success = success,
                Timestamp = DateTime.UtcNow
            };

            await _context.FeatureUsageLogs.AddAsync(featureUsage);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Tracked feature usage for user {UserId}: {Feature} in {Context}", userId, featureName, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking feature usage for user {UserId}", userId);
        }
    }

    public async Task<DashboardAnalyticsDto> GetUserAnalyticsAsync(string userId, int daysPeriod = 30)
    {
        try
        {
            // Try cache first
            var cachedAnalytics = await _cacheService.GetAnalyticsDataAsync(userId);
            if (cachedAnalytics != null)
            {
                _logger.LogDebug("Analytics served from cache for user {UserId}", userId);
                return cachedAnalytics;
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-daysPeriod);

            // Get session data
            var sessions = await _context.UserSessionLogs
                .Where(s => s.UserId == userId && s.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            // Get widget usage
            var widgetUsage = await _context.WidgetUsageLogs
                .Where(w => w.UserId == userId && w.Timestamp >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            // Get feature usage
            var featureUsage = await _context.FeatureUsageLogs
                .Where(f => f.UserId == userId && f.Timestamp >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            // Get booking data
            var bookings = await _context.Bookings
                .Where(b => b.CustomerId == userId && b.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            var analytics = new DashboardAnalyticsDto
            {
                TotalLogins = sessions.Count,
                WeeklyLogins = sessions.Count(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                WidgetUsage = widgetUsage.GroupBy(w => w.WidgetType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                MostUsedFeatures = featureUsage.GroupBy(f => f.FeatureName)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList(),
                AverageSessionDuration = sessions.Any() 
                    ? TimeSpan.FromTicks((long)sessions.Average(s => s.Duration.Ticks))
                    : TimeSpan.Zero,
                ServiceUsageByCategory = bookings.GroupBy(b => b.Service?.Category ?? "Unknown")
                    .ToDictionary(g => g.Key, g => (double)g.Count()),
                MonthlySpending = bookings.Sum(b => b.TotalPrice),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                UsageTrends = GenerateUsageTrends(sessions, daysPeriod)
            };

            // Cache the analytics
            await _cacheService.SetAnalyticsDataAsync(userId, analytics);

            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user analytics for {UserId}", userId);
            return new DashboardAnalyticsDto();
        }
    }

    public async Task<SystemAnalyticsDto> GetSystemAnalyticsAsync(int daysPeriod = 7)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysPeriod);

            var totalUsers = await _context.Users.CountAsync();
            var newUsers = await _context.Users.CountAsync(u => u.CreatedAt >= cutoffDate);

            var widgetPopularity = await _context.WidgetUsageLogs
                .Where(w => w.Timestamp >= cutoffDate)
                .GroupBy(w => w.WidgetType)
                .Select(g => new { Widget = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Widget, x => x.Count);

            var sessionData = await _context.UserSessionLogs
                .Where(s => s.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            var bookings = await _context.Bookings
                .Where(b => b.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            return new SystemAnalyticsDto
            {
                TotalActiveUsers = totalUsers,
                NewUsersThisPeriod = newUsers,
                WidgetPopularity = widgetPopularity,
                AverageSessionDuration = sessionData.Any() 
                    ? TimeSpan.FromTicks((long)sessionData.Average(s => s.Duration.Ticks))
                    : TimeSpan.Zero,
                TotalBookingsCompleted = bookings.Count(b => b.Status == "Completed"),
                TotalRevenue = bookings.Sum(b => b.TotalPrice),
                CustomerSatisfactionScore = await CalculateCustomerSatisfactionScore(daysPeriod)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system analytics");
            return new SystemAnalyticsDto();
        }
    }

    public async Task<UserBehaviorInsightsDto> GetUserBehaviorInsightsAsync(string userId)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var widgetUsage = await _context.WidgetUsageLogs
                .Where(w => w.UserId == userId && w.Timestamp >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            var sessions = await _context.UserSessionLogs
                .Where(s => s.UserId == userId && s.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Service)
                .Where(b => b.CustomerId == userId && b.CreatedAt >= cutoffDate)
                .AsNoTracking()
                .ToListAsync();

            var preferredWidgets = widgetUsage
                .GroupBy(w => w.WidgetType)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var servicePreferences = bookings
                .GroupBy(b => b.Service?.Category ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            var peakHours = sessions
                .GroupBy(s => s.SessionStart.Hour)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key.ToString("00") + ":00")
                .ToList();

            return new UserBehaviorInsightsDto
            {
                UserId = userId,
                PreferredWidgets = preferredWidgets,
                ServicePreferences = servicePreferences,
                AverageSessionDuration = sessions.Any() 
                    ? TimeSpan.FromTicks((long)sessions.Average(s => s.Duration.Ticks))
                    : TimeSpan.Zero,
                PeakUsageHours = peakHours,
                EngagementScore = CalculateEngagementScore(widgetUsage, sessions, bookings),
                UserSegment = DetermineUserSegment(bookings, sessions)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user behavior insights for {UserId}", userId);
            return new UserBehaviorInsightsDto { UserId = userId };
        }
    }

    public async Task<DashboardPerformanceMetricsDto> GetPerformanceMetricsAsync(int daysPeriod = 7)
    {
        try
        {
            // This would typically integrate with APM tools like Application Insights
            // For now, providing simulated performance data
            
            return new DashboardPerformanceMetricsDto
            {
                AverageLoadTimes = new Dictionary<string, double>
                {
                    ["dashboard"] = 1.2,
                    ["pets"] = 0.8,
                    ["services"] = 1.5,
                    ["weather"] = 0.5
                },
                CacheHitRatios = new Dictionary<string, double>
                {
                    ["dashboard_config"] = 0.85,
                    ["pets_data"] = 0.72,
                    ["weather_data"] = 0.95,
                    ["analytics"] = 0.68
                },
                SystemUptime = 99.9,
                TotalErrors = await GetErrorCountAsync(daysPeriod)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating performance metrics");
            return new DashboardPerformanceMetricsDto();
        }
    }

    public async Task<List<PersonalizedRecommendationDto>> GenerateRecommendationsAsync(string userId)
    {
        try
        {
            var recommendations = new List<PersonalizedRecommendationDto>();

            // Analyze user behavior to generate recommendations
            var recentBookings = await _context.Bookings
                .Include(b => b.Service)
                .Where(b => b.CustomerId == userId && b.CreatedAt >= DateTime.UtcNow.AddDays(-60))
                .AsNoTracking()
                .ToListAsync();

            var userPets = await _context.DogProfiles
                .Include(d => d.MedicalRecords)
                .Where(d => d.OwnerId == userId && d.IsActive)
                .AsNoTracking()
                .ToListAsync();

            // Service recommendations based on usage patterns
            if (recentBookings.Any(b => b.Service?.Category == "grooming"))
            {
                recommendations.Add(new PersonalizedRecommendationDto
                {
                    Type = "service",
                    Title = "Premium Grooming Services",
                    Description = "Based on your grooming history, you might enjoy our premium spa services",
                    RelevanceScore = 0.85,
                    ReasoningText = "You've booked 3 grooming sessions in the past 2 months",
                    ActionUrl = "/search?service=grooming&premium=true",
                    ActionText = "Explore Premium Services",
                    ExpiresAt = DateTime.UtcNow.AddDays(14)
                });
            }

            // Health recommendations based on pet data
            foreach (var pet in userPets)
            {
                var lastCheckup = pet.MedicalRecords
                    ?.Where(mr => mr.RecordType == "Checkup")
                    .OrderByDescending(mr => mr.EventDate)
                    .FirstOrDefault();

                if (lastCheckup == null || lastCheckup.EventDate < DateTime.UtcNow.AddMonths(-6))
                {
                    recommendations.Add(new PersonalizedRecommendationDto
                    {
                        Type = "health",
                        Title = $"Health Checkup for {pet.Name}",
                        Description = $"{pet.Name} is due for a health checkup",
                        RelevanceScore = 0.95,
                        ReasoningText = lastCheckup == null ? "No checkup recorded" : "Last checkup was over 6 months ago",
                        ActionUrl = "/search?service=veterinary",
                        ActionText = "Book Checkup",
                        ExpiresAt = DateTime.UtcNow.AddDays(30)
                    });
                }
            }

            return recommendations.OrderByDescending(r => r.RelevanceScore).Take(5).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations for user {UserId}", userId);
            return new List<PersonalizedRecommendationDto>();
        }
    }

    public async Task TrackBookingFunnelAsync(string userId, string providerId, string step, bool completed, Dictionary<string, object>? metadata = null)
    {
        try
        {
            var funnelLog = new BookingFunnelLog
            {
                UserId = userId,
                ProviderId = providerId,
                Step = step,
                Completed = completed,
                Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
                Timestamp = DateTime.UtcNow
            };

            await _context.BookingFunnelLogs.AddAsync(funnelLog);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Tracked booking funnel for user {UserId}: {Step} - {Status}", userId, step, completed ? "Completed" : "Abandoned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking booking funnel for user {UserId}", userId);
        }
    }

    public async Task<ABTestInsightsDto> GetABTestInsightsAsync(string testName, int daysPeriod = 30)
    {
        // Placeholder for A/B testing functionality
        // This would integrate with feature flagging systems
        await Task.CompletedTask;
        
        return new ABTestInsightsDto
        {
            TestName = testName,
            Status = "Not Implemented",
            TotalParticipants = 0
        };
    }

    public async Task<AutomatedInsightsDto> GenerateAutomatedInsightsAsync(string userId)
    {
        try
        {
            var insights = new List<InsightDto>();
            var anomalies = new List<AnomalyDto>();
            var opportunities = new List<OpportunityDto>();

            // Analyze user patterns for insights
            var recentSessions = await _context.UserSessionLogs
                .Where(s => s.UserId == userId && s.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .AsNoTracking()
                .ToListAsync();

            // Generate usage pattern insights
            if (recentSessions.Count > 10)
            {
                var avgSessionTime = recentSessions.Average(s => s.Duration.TotalMinutes);
                if (avgSessionTime > 15)
                {
                    insights.Add(new InsightDto
                    {
                        Type = "engagement",
                        Title = "High Engagement Detected",
                        Description = $"Your average session time is {avgSessionTime:F1} minutes, indicating strong platform engagement",
                        Impact = "High",
                        ConfidenceScore = 0.9,
                        RecommendedActions = new List<string>
                        {
                            "Consider premium features",
                            "Explore advanced dashboard customization"
                        }
                    });
                }
            }

            return new AutomatedInsightsDto
            {
                Insights = insights,
                Anomalies = anomalies,
                Opportunities = opportunities
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating automated insights for user {UserId}", userId);
            return new AutomatedInsightsDto();
        }
    }

    #region Private Helper Methods

    private List<UsageTrendDto> GenerateUsageTrends(List<UserSessionLog> sessions, int daysPeriod)
    {
        var trends = new List<UsageTrendDto>();
        var startDate = DateTime.UtcNow.AddDays(-daysPeriod);

        for (int i = 0; i < daysPeriod; i++)
        {
            var date = startDate.AddDays(i);
            var daySessions = sessions.Where(s => s.CreatedAt.Date == date.Date).ToList();

            trends.Add(new UsageTrendDto
            {
                Date = date,
                Sessions = daySessions.Count,
                Actions = daySessions.Sum(s => GetActionsCount(s.Actions)),
                Duration = daySessions.Any() 
                    ? TimeSpan.FromTicks((long)daySessions.Average(s => s.Duration.Ticks))
                    : TimeSpan.Zero
            });
        }

        return trends;
    }

    private int GetActionsCount(string? actionsJson)
    {
        if (string.IsNullOrEmpty(actionsJson))
            return 0;

        try
        {
            var actions = JsonSerializer.Deserialize<Dictionary<string, int>>(actionsJson);
            return actions?.Values.Sum() ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    private double CalculateEngagementScore(List<WidgetUsageLog> widgetUsage, List<UserSessionLog> sessions, List<Booking> bookings)
    {
        double score = 0;

        // Widget diversity score (0-40 points)
        var uniqueWidgets = widgetUsage.Select(w => w.WidgetType).Distinct().Count();
        score += Math.Min(40, uniqueWidgets * 8);

        // Session frequency score (0-30 points)
        var sessionsThisWeek = sessions.Count(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7));
        score += Math.Min(30, sessionsThisWeek * 5);

        // Booking conversion score (0-30 points)
        var completedBookings = bookings.Count(b => b.Status == "Completed");
        score += Math.Min(30, completedBookings * 10);

        return Math.Min(100, score);
    }

    private string DetermineUserSegment(List<Booking> bookings, List<UserSessionLog> sessions)
    {
        var totalSpent = bookings.Sum(b => b.TotalPrice);
        var sessionCount = sessions.Count;

        if (totalSpent > 500 && sessionCount > 20)
            return "Premium";
        else if (totalSpent > 200 || sessionCount > 10)
            return "Active";
        else if (bookings.Any() || sessionCount > 5)
            return "Standard";
        else
            return "New";
    }

    private async Task<double> CalculateCustomerSatisfactionScore(int daysPeriod)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysPeriod);
            var reviews = await _context.ServiceProviderReviews
                .Where(r => r.CreatedAt >= cutoffDate && r.IsActive)
                .Select(r => r.Rating)
                .ToListAsync();

            return reviews.Any() ? reviews.Average() : 0.0;
        }
        catch
        {
            return 0.0;
        }
    }

    private async Task<int> GetErrorCountAsync(int daysPeriod)
    {
        // This would typically query error logs or APM systems
        // Placeholder implementation
        await Task.CompletedTask;
        return 0;
    }

    #endregion
}

