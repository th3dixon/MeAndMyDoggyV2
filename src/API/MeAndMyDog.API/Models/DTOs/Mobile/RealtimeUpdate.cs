namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Individual real-time update
/// </summary>
public class RealtimeUpdate
{
    public string Type { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; }
}