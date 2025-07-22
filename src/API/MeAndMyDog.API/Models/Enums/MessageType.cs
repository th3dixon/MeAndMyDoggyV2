namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of messages that can be sent
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Plain text message
    /// </summary>
    Text = 0,
    
    /// <summary>
    /// Image attachment
    /// </summary>
    Image = 1,
    
    /// <summary>
    /// File attachment
    /// </summary>
    File = 2,
    
    /// <summary>
    /// Audio message
    /// </summary>
    Audio = 3,
    
    /// <summary>
    /// Voice message (recorded audio)
    /// </summary>
    Voice = 8,
    
    /// <summary>
    /// Video message
    /// </summary>
    Video = 4,
    
    /// <summary>
    /// System message (automated)
    /// </summary>
    System = 5,
    
    /// <summary>
    /// Location sharing
    /// </summary>
    Location = 6,
    
    /// <summary>
    /// Booking/appointment information
    /// </summary>
    Booking = 7
}