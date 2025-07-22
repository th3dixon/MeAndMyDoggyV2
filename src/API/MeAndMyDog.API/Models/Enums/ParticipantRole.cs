namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Roles for appointment participants
/// </summary>
public enum ParticipantRole
{
    /// <summary>
    /// Participant is the appointment organizer
    /// </summary>
    Organizer = 0,
    
    /// <summary>
    /// Required attendee for the appointment
    /// </summary>
    RequiredAttendee = 1,
    
    /// <summary>
    /// Optional attendee for the appointment
    /// </summary>
    OptionalAttendee = 2,
    
    /// <summary>
    /// Service provider for the appointment
    /// </summary>
    ServiceProvider = 3,
    
    /// <summary>
    /// Pet owner attending the appointment
    /// </summary>
    PetOwner = 4,
    
    /// <summary>
    /// Veterinarian or medical professional
    /// </summary>
    Veterinarian = 5,
    
    /// <summary>
    /// Pet trainer or behaviorist
    /// </summary>
    Trainer = 6,
    
    /// <summary>
    /// Pet groomer
    /// </summary>
    Groomer = 7,
    
    /// <summary>
    /// Pet walker or caregiver
    /// </summary>
    Caregiver = 8,
    
    /// <summary>
    /// Observer or informational participant
    /// </summary>
    Observer = 9,
    
    /// <summary>
    /// Support staff member
    /// </summary>
    Support = 10
}