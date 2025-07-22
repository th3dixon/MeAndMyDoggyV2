namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of appointments
/// </summary>
public enum AppointmentStatus
{
    /// <summary>
    /// Appointment is scheduled and confirmed
    /// </summary>
    Scheduled = 0,
    
    /// <summary>
    /// Appointment is pending confirmation
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Appointment has been confirmed by all parties
    /// </summary>
    Confirmed = 2,
    
    /// <summary>
    /// Appointment is currently in progress
    /// </summary>
    InProgress = 3,
    
    /// <summary>
    /// Appointment has been completed
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// Appointment has been cancelled
    /// </summary>
    Cancelled = 5,
    
    /// <summary>
    /// Participant did not show up for appointment
    /// </summary>
    NoShow = 6,
    
    /// <summary>
    /// Appointment was rescheduled
    /// </summary>
    Rescheduled = 7,
    
    /// <summary>
    /// Appointment is on hold/paused
    /// </summary>
    OnHold = 8,
    
    /// <summary>
    /// Appointment requires attention/follow-up
    /// </summary>
    RequiresAttention = 9
}