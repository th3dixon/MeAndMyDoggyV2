namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Events that can trigger self-destruct timer
/// </summary>
public enum TriggerEvent
{
    /// <summary>
    /// Timer starts when message is sent
    /// </summary>
    MessageSent = 0,
    
    /// <summary>
    /// Timer starts when message is delivered
    /// </summary>
    MessageDelivered = 1,
    
    /// <summary>
    /// Timer starts when message is first read
    /// </summary>
    FirstRead = 2,
    
    /// <summary>
    /// Timer starts when all recipients have read
    /// </summary>
    AllRead = 3,
    
    /// <summary>
    /// Timer starts when any recipient reads
    /// </summary>
    AnyRead = 4,
    
    /// <summary>
    /// Timer starts manually by sender
    /// </summary>
    Manual = 5,
    
    /// <summary>
    /// Timer starts at specific date/time
    /// </summary>
    ScheduledStart = 6,
    
    /// <summary>
    /// Timer starts when conversation becomes inactive
    /// </summary>
    ConversationInactive = 7,
    
    /// <summary>
    /// Timer starts based on custom condition
    /// </summary>
    Custom = 8
}