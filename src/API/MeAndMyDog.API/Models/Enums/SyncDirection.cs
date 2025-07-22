namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Calendar synchronization directions
/// </summary>
public enum SyncDirection
{
    /// <summary>
    /// Only sync from internal calendar to external calendar
    /// </summary>
    OutboundOnly = 0,
    
    /// <summary>
    /// Only sync from external calendar to internal calendar
    /// </summary>
    InboundOnly = 1,
    
    /// <summary>
    /// Sync in both directions (bidirectional)
    /// </summary>
    Bidirectional = 2,
    
    /// <summary>
    /// No automatic synchronization
    /// </summary>
    None = 3
}