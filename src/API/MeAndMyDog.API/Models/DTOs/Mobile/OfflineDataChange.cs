namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Offline data change
/// </summary>
public class OfflineDataChange
{
    public string Id { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // create, update, delete
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; }
}