namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Offline sync request
/// </summary>
public class OfflineSyncRequest
{
    public DateTime LastSyncTime { get; set; }
    public List<OfflineDataChange> Changes { get; set; } = new();
    public string DeviceId { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
}