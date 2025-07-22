namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// User session analytics tracking
/// </summary>
public class UserSessionLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    public TimeSpan Duration { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string Actions { get; set; } = "{}"; // JSON serialized actions
    public int PageViews { get; set; }
    public DateTime CreatedAt { get; set; }
}