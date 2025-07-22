namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of location sharing
/// </summary>
public enum LocationShareType
{
    /// <summary>
    /// Static location that doesn't update
    /// </summary>
    Static = 0,
    
    /// <summary>
    /// Current location at time of sending
    /// </summary>
    Current = 1,
    
    /// <summary>
    /// Live location that updates in real-time
    /// </summary>
    Live = 2,
    
    /// <summary>
    /// Saved bookmark location
    /// </summary>
    Bookmark = 3
}