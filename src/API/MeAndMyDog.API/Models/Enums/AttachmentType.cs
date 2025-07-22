namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of message attachments
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// Generic file attachment
    /// </summary>
    File = 0,
    
    /// <summary>
    /// Image attachment (JPEG, PNG, GIF, etc.)
    /// </summary>
    Image = 1,
    
    /// <summary>
    /// Video attachment
    /// </summary>
    Video = 2,
    
    /// <summary>
    /// Audio attachment
    /// </summary>
    Audio = 3,
    
    /// <summary>
    /// Document attachment (PDF, DOC, etc.)
    /// </summary>
    Document = 4,
    
    /// <summary>
    /// Location sharing
    /// </summary>
    Location = 5
}