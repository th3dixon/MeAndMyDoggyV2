namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Recurrence patterns for appointments
/// </summary>
public enum RecurrencePattern
{
    /// <summary>
    /// No recurrence (one-time appointment)
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Recurring daily
    /// </summary>
    Daily = 1,
    
    /// <summary>
    /// Recurring weekly
    /// </summary>
    Weekly = 2,
    
    /// <summary>
    /// Recurring bi-weekly (every 2 weeks)
    /// </summary>
    BiWeekly = 3,
    
    /// <summary>
    /// Recurring monthly
    /// </summary>
    Monthly = 4,
    
    /// <summary>
    /// Recurring quarterly (every 3 months)
    /// </summary>
    Quarterly = 5,
    
    /// <summary>
    /// Recurring yearly
    /// </summary>
    Yearly = 6,
    
    /// <summary>
    /// Recurring on specific weekdays
    /// </summary>
    Weekdays = 7,
    
    /// <summary>
    /// Custom recurrence pattern
    /// </summary>
    Custom = 8
}