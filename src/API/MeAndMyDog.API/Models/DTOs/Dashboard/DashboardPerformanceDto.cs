namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Dashboard configuration data transfer object
/// </summary>
public class DashboardConfigDto
{
    public string[] WidgetLayout { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> Preferences { get; set; } = new();
    public UserInfoDto User { get; set; } = new();
}

/// <summary>
/// User information for dashboard
/// </summary>
public class UserInfoDto
{
    public string FirstName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
}

/// <summary>
/// Pet summary for dashboard
/// </summary>
public class PetSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string Age { get; set; } = "Unknown age";
    public string? Image { get; set; }
    public string HealthStatus { get; set; } = "Unknown";
    public DateTimeOffset? LastCheckup { get; set; }
    public string LastCheckupFormatted { get; set; } = "No checkup recorded";
    public DateTimeOffset? NextAppointment { get; set; }
    public int PendingHealthActions { get; set; }
    public decimal? Weight { get; set; }
    public string VaccinationStatus { get; set; } = "Unknown";
    public int HealthScore { get; set; }
}

/// <summary>
/// Upcoming service summary
/// </summary>
public class UpcomingServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceCategory { get; set; } = string.Empty;
    public string ServiceCategoryIcon { get; set; } = "fas fa-paw";
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderImage { get; set; }
    public double ProviderRating { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Pet { get; set; } = string.Empty;
    public string? PetBreed { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Weather data for dashboard
/// </summary>
public class WeatherDataDto
{
    public int Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int FeelsLike { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string PetTip { get; set; } = string.Empty;
    public CoordinatesDto? Coordinates { get; set; }
    public string Source { get; set; } = string.Empty;
}

/// <summary>
/// Coordinates data
/// </summary>
public class CoordinatesDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

/// <summary>
/// Dashboard analytics data
/// </summary>
public class DashboardAnalyticsDto
{
    public int TotalLogins { get; set; }
    public int WeeklyLogins { get; set; }
    public Dictionary<string, int> WidgetUsage { get; set; } = new();
    public List<string> MostUsedFeatures { get; set; } = new();
    public TimeSpan AverageSessionDuration { get; set; }
    public Dictionary<string, double> ServiceUsageByCategory { get; set; } = new();
    public decimal MonthlySpending { get; set; }
    public int CompletedBookings { get; set; }
    public List<UsageTrendDto> UsageTrends { get; set; } = new();
}

/// <summary>
/// Usage trend data point
/// </summary>
public class UsageTrendDto
{
    public DateTime Date { get; set; }
    public int Sessions { get; set; }
    public int Actions { get; set; }
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Cache statistics
/// </summary>
public class DashboardCacheStatsDto
{
    public long TotalKeys { get; set; }
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public double HitRatio { get; set; }
    public Dictionary<string, long> KeysByType { get; set; } = new();
    public long MemoryUsageBytes { get; set; }
}