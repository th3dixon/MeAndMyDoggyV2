namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Connection status values for video call participants
/// </summary>
public enum ConnectionStatus
{
    /// <summary>
    /// Participant is attempting to connect to the call
    /// </summary>
    Connecting = 0,
    
    /// <summary>
    /// Participant is successfully connected to the call
    /// </summary>
    Connected = 1,
    
    /// <summary>
    /// Participant has disconnected from the call
    /// </summary>
    Disconnected = 2,
    
    /// <summary>
    /// Participant is attempting to reconnect after a connection issue
    /// </summary>
    Reconnecting = 3,
    
    /// <summary>
    /// Connection failed and cannot be established
    /// </summary>
    Failed = 4
}