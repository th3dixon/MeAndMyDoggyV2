namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Widget usage analytics tracking
/// </summary>
public class WidgetUsageLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string WidgetType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Metadata { get; set; } = "{}"; // JSON serialized metadata
    public DateTime Timestamp { get; set; }
    public TimeSpan InteractionDuration { get; set; }
}