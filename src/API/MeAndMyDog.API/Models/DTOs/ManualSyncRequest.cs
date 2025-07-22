using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to manually sync calendar
/// </summary>
public class ManualSyncRequest
{
    /// <summary>
    /// Direction to sync (optional, uses integration default)
    /// </summary>
    public SyncDirection? Direction { get; set; }

    /// <summary>
    /// Whether to force full sync instead of incremental
    /// </summary>
    public bool ForceFullSync { get; set; } = false;

    /// <summary>
    /// Start date for sync range (optional)
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// End date for sync range (optional)
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
}