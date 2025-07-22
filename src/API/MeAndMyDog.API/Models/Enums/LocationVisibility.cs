namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Visibility settings for location sharing
/// </summary>
public enum LocationVisibility
{
    /// <summary>
    /// Location visible to all conversation participants
    /// </summary>
    Conversation = 0,
    
    /// <summary>
    /// Location visible only to specific users
    /// </summary>
    Restricted = 1,
    
    /// <summary>
    /// Location visible to the sender only (private)
    /// </summary>
    Private = 2,
    
    /// <summary>
    /// Location visible to everyone (public)
    /// </summary>
    Public = 3
}