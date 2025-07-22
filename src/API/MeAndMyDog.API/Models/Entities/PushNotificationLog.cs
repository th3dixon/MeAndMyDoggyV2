namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Push notification delivery tracking
/// </summary>
public class PushNotificationLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Data { get; set; }
    public int DevicesTargeted { get; set; }
    public int DevicesReached { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}