namespace MeAndMyDog.API.Models.DTOs.DashboardAnalytics;

/// <summary>
/// Feature tracking request
/// </summary>
public class FeatureTrackingRequest
{
    public string FeatureName { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}