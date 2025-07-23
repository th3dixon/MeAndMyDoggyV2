namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Real-time update data
/// </summary>
public class MobileRealtimeUpdateDto
{
    public List<RealtimeUpdate> Updates { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool HasMoreUpdates { get; set; }
}