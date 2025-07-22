namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Priority levels for appointments
/// </summary>
public enum AppointmentPriority
{
    /// <summary>
    /// Low priority appointment
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Normal priority appointment
    /// </summary>
    Normal = 1,
    
    /// <summary>
    /// High priority appointment
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Urgent appointment requiring immediate attention
    /// </summary>
    Urgent = 3,
    
    /// <summary>
    /// Critical/emergency appointment
    /// </summary>
    Critical = 4
}