namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Self-destruct modes for messages
/// </summary>
public enum DestructMode
{
    /// <summary>
    /// No self-destruction
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Destruct after timer expires
    /// </summary>
    Timer = 1,
    
    /// <summary>
    /// Destruct after being read
    /// </summary>
    AfterReading = 2,
    
    /// <summary>
    /// Destruct after maximum views reached
    /// </summary>
    ViewLimit = 3,
    
    /// <summary>
    /// Destruct at specific date/time
    /// </summary>
    ScheduledTime = 4,
    
    /// <summary>
    /// Destruct when sender requests
    /// </summary>
    OnDemand = 5,
    
    /// <summary>
    /// Destruct when conversation ends
    /// </summary>
    ConversationEnd = 6,
    
    /// <summary>
    /// Destruct based on custom conditions
    /// </summary>
    Custom = 7
}