namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Network quality levels for video calls
/// </summary>
public enum NetworkQuality
{
    /// <summary>
    /// Unknown or not measured network quality
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Poor network quality - significant issues expected
    /// </summary>
    Poor = 1,
    
    /// <summary>
    /// Fair network quality - some issues may occur
    /// </summary>
    Fair = 2,
    
    /// <summary>
    /// Good network quality - minor issues possible
    /// </summary>
    Good = 3,
    
    /// <summary>
    /// Excellent network quality - optimal experience
    /// </summary>
    Excellent = 4
}