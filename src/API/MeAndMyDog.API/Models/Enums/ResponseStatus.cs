namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Response status for appointment participants
/// </summary>
public enum ResponseStatus
{
    /// <summary>
    /// No response received yet
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Participant has accepted the appointment
    /// </summary>
    Accepted = 1,
    
    /// <summary>
    /// Participant has declined the appointment
    /// </summary>
    Declined = 2,
    
    /// <summary>
    /// Participant has tentatively accepted
    /// </summary>
    Tentative = 3,
    
    /// <summary>
    /// Participant needs to reschedule
    /// </summary>
    NeedsReschedule = 4,
    
    /// <summary>
    /// Response is delegated to another person
    /// </summary>
    Delegated = 5,
    
    /// <summary>
    /// Participant is not responding
    /// </summary>
    NotResponding = 6
}