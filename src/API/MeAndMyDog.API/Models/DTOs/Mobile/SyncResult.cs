namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Sync result
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public List<string> Conflicts { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public DateTime ServerTime { get; set; } = DateTime.UtcNow;
    public int ProcessedChanges { get; set; }
    public Dictionary<string, object> UpdatedData { get; set; } = new();
}