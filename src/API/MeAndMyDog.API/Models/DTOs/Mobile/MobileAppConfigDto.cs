namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile app configuration
/// </summary>
public class MobileAppConfigDto
{
    public string MinimumVersion { get; set; } = string.Empty;
    public bool ForceUpdate { get; set; }
    public Dictionary<string, object> FeatureFlags { get; set; } = new();
    public List<string> MaintenanceWindows { get; set; } = new();
    public Dictionary<string, string> ApiEndpoints { get; set; } = new();
    public int MaxCacheAge { get; set; } = 3600;
    public Dictionary<string, int> RateLimits { get; set; } = new();
}