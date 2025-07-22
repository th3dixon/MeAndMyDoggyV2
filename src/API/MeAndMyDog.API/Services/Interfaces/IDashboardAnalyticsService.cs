using MeAndMyDog.API.Models.DTOs.Dashboard;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Advanced analytics service for dashboard insights and usage tracking
/// </summary>
public interface IDashboardAnalyticsService
{
    /// <summary>
    /// Track user dashboard session
    /// </summary>
    Task TrackSessionAsync(string userId, TimeSpan sessionDuration, Dictionary<string, int> actionsPerformed);
    
    /// <summary>
    /// Track widget usage
    /// </summary>
    Task TrackWidgetUsageAsync(string userId, string widgetType, string action, Dictionary<string, object>? metadata = null);
    
    /// <summary>
    /// Track feature usage
    /// </summary>
    Task TrackFeatureUsageAsync(string userId, string featureName, string context, bool success = true);
    
    /// <summary>
    /// Get comprehensive user analytics
    /// </summary>
    Task<DashboardAnalyticsDto> GetUserAnalyticsAsync(string userId, int daysPeriod = 30);
    
    /// <summary>
    /// Get system-wide dashboard analytics
    /// </summary>
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync(int daysPeriod = 7);
    
    /// <summary>
    /// Get user behavior insights
    /// </summary>
    Task<UserBehaviorInsightsDto> GetUserBehaviorInsightsAsync(string userId);
    
    /// <summary>
    /// Get performance metrics for dashboard components
    /// </summary>
    Task<DashboardPerformanceMetricsDto> GetPerformanceMetricsAsync(int daysPeriod = 7);
    
    /// <summary>
    /// Generate personalized recommendations
    /// </summary>
    Task<List<PersonalizedRecommendationDto>> GenerateRecommendationsAsync(string userId);
    
    /// <summary>
    /// Track booking completion funnel
    /// </summary>
    Task TrackBookingFunnelAsync(string userId, string providerId, string step, bool completed, Dictionary<string, object>? metadata = null);
    
    /// <summary>
    /// Get A/B testing insights
    /// </summary>
    Task<ABTestInsightsDto> GetABTestInsightsAsync(string testName, int daysPeriod = 30);
    
    /// <summary>
    /// Generate automated insights report
    /// </summary>
    Task<AutomatedInsightsDto> GenerateAutomatedInsightsAsync(string userId);
}