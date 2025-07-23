namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile push notification data
/// </summary>
public class MobilePushNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = "general"; // appointment, booking, system, etc.
    public Dictionary<string, object> Data { get; set; } = new();
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public string? Sound { get; set; }
    public int Badge { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string Priority { get; set; } = "normal"; // low, normal, high
    public TimeSpan? TimeToLive { get; set; }
}