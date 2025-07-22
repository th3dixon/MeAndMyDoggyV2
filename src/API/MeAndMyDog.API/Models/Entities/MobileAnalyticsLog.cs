namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Mobile app analytics tracking
/// </summary>
public class MobileAnalyticsLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}