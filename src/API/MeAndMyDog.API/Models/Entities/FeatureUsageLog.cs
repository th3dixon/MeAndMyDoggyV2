namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Feature usage analytics tracking
/// </summary>
public class FeatureUsageLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}