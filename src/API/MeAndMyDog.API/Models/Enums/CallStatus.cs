namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status values for video call sessions
/// </summary>
public enum CallStatus
{
    /// <summary>
    /// Call is being initiated and waiting for participants to join
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Call is ringing and waiting for participants to accept
    /// </summary>
    Ringing = 1,
    
    /// <summary>
    /// Call is active with participants connected
    /// </summary>
    Active = 2,
    
    /// <summary>
    /// Call has ended normally
    /// </summary>
    Ended = 3,
    
    /// <summary>
    /// Call was cancelled before starting
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Call was rejected by one or more participants
    /// </summary>
    Rejected = 5,
    
    /// <summary>
    /// Call failed due to technical issues
    /// </summary>
    Failed = 6,
    
    /// <summary>
    /// Call timed out without participants joining
    /// </summary>
    Timeout = 7
}