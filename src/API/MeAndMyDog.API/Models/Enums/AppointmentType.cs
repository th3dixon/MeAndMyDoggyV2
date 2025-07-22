namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of appointments
/// </summary>
public enum AppointmentType
{
    /// <summary>
    /// General meeting appointment
    /// </summary>
    Meeting = 0,
    
    /// <summary>
    /// Service consultation appointment
    /// </summary>
    Consultation = 1,
    
    /// <summary>
    /// Pet grooming appointment
    /// </summary>
    Grooming = 2,
    
    /// <summary>
    /// Veterinary appointment
    /// </summary>
    Veterinary = 3,
    
    /// <summary>
    /// Pet training session
    /// </summary>
    Training = 4,
    
    /// <summary>
    /// Pet walking service
    /// </summary>
    Walking = 5,
    
    /// <summary>
    /// Pet boarding check-in/out
    /// </summary>
    Boarding = 6,
    
    /// <summary>
    /// Pet photography session
    /// </summary>
    Photography = 7,
    
    /// <summary>
    /// Follow-up appointment
    /// </summary>
    FollowUp = 8,
    
    /// <summary>
    /// Emergency appointment
    /// </summary>
    Emergency = 9,
    
    /// <summary>
    /// Other custom appointment type
    /// </summary>
    Other = 10
}