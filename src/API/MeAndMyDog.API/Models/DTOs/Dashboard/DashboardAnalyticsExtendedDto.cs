namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// System-wide analytics data
/// </summary>
public class SystemAnalyticsDto
{
    public int TotalActiveUsers { get; set; }
    public int NewUsersThisPeriod { get; set; }
    public Dictionary<string, int> WidgetPopularity { get; set; } = new();
    public Dictionary<string, double> FeatureAdoptionRates { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public List<UsageTrendDto> UserGrowthTrend { get; set; } = new();
    public Dictionary<string, int> DeviceBreakdown { get; set; } = new();
    public Dictionary<string, int> GeographicDistribution { get; set; } = new();
    public double CustomerSatisfactionScore { get; set; }
    public int TotalBookingsCompleted { get; set; }
    public decimal TotalRevenue { get; set; }
}

/// <summary>
/// User behavior insights
/// </summary>
public class UserBehaviorInsightsDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> PreferredWidgets { get; set; } = new();
    public Dictionary<string, int> ServicePreferences { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public List<string> PeakUsageHours { get; set; } = new();
    public Dictionary<string, double> ConversionRates { get; set; } = new();
    public string UserSegment { get; set; } = "Standard";
    public double EngagementScore { get; set; }
    public List<BehaviorPatternDto> BehaviorPatterns { get; set; } = new();
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}

/// <summary>
/// Behavior pattern data
/// </summary>
public class BehaviorPatternDto
{
    public string PatternName { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime LastObserved { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Dashboard performance metrics
/// </summary>
public class DashboardPerformanceMetricsDto
{
    public Dictionary<string, double> AverageLoadTimes { get; set; } = new();
    public Dictionary<string, int> ErrorRates { get; set; } = new();
    public Dictionary<string, double> CacheHitRatios { get; set; } = new();
    public List<PerformanceTrendDto> PerformanceTrends { get; set; } = new();
    public Dictionary<string, int> APICallVolumes { get; set; } = new();
    public double SystemUptime { get; set; }
    public int TotalErrors { get; set; }
    public Dictionary<string, double> DatabaseQueryPerformance { get; set; } = new();
}

/// <summary>
/// Performance trend data point
/// </summary>
public class PerformanceTrendDto
{
    public DateTime Timestamp { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}

/// <summary>
/// Personalized recommendation
/// </summary>
public class PersonalizedRecommendationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty; // service, provider, feature, etc.
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public string ReasoningText { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionText { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// A/B testing insights
/// </summary>
public class ABTestInsightsDto
{
    public string TestName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public Dictionary<string, ABTestVariantDto> Variants { get; set; } = new();
    public string WinningVariant { get; set; } = string.Empty;
    public double ConfidenceLevel { get; set; }
    public Dictionary<string, double> ConversionRates { get; set; } = new();
    public int TotalParticipants { get; set; }
    public string StatisticalSignificance { get; set; } = string.Empty;
}

/// <summary>
/// A/B test variant data
/// </summary>
public class ABTestVariantDto
{
    public string Name { get; set; } = string.Empty;
    public int Participants { get; set; }
    public int Conversions { get; set; }
    public double ConversionRate { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
}

/// <summary>
/// Automated insights
/// </summary>
public class AutomatedInsightsDto
{
    public List<InsightDto> Insights { get; set; } = new();
    public List<AnomalyDto> Anomalies { get; set; } = new();
    public List<OpportunityDto> Opportunities { get; set; } = new();
    public List<AlertDto> Alerts { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string InsightsVersion { get; set; } = "1.0";
}

/// <summary>
/// Individual insight
/// </summary>
public class InsightDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = "Medium"; // Low, Medium, High
    public double ConfidenceScore { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Anomaly detection
/// </summary>
public class AnomalyDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MetricName { get; set; } = string.Empty;
    public double ExpectedValue { get; set; }
    public double ActualValue { get; set; }
    public double DeviationScore { get; set; }
    public string Severity { get; set; } = "Medium";
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public string PossibleCause { get; set; } = string.Empty;
    public List<string> SuggestedInvestigations { get; set; } = new();
}

/// <summary>
/// Business opportunity
/// </summary>
public class OpportunityDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double EstimatedImpact { get; set; }
    public string ImpactUnit { get; set; } = "percentage";
    public string Priority { get; set; } = "Medium";
    public double ImplementationEffort { get; set; }
    public List<string> RequiredActions { get; set; } = new();
    public DateTime IdentifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// System alert
/// </summary>
public class AlertDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public string ActionUrl { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}