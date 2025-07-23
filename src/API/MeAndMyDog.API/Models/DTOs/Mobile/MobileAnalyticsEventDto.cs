namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile analytics event
/// </summary>
public class MobileAnalyticsEventDto
{
    public string EventName { get; set; } = string.Empty;
    public string EventCategory { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string DeviceId { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
}