namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of appointment reminders
/// </summary>
public enum ReminderType
{
    /// <summary>
    /// Email reminder notification
    /// </summary>
    Email = 0,
    
    /// <summary>
    /// SMS text message reminder
    /// </summary>
    SMS = 1,
    
    /// <summary>
    /// Push notification reminder
    /// </summary>
    PushNotification = 2,
    
    /// <summary>
    /// In-app notification reminder
    /// </summary>
    InApp = 3,
    
    /// <summary>
    /// Phone call reminder
    /// </summary>
    PhoneCall = 4,
    
    /// <summary>
    /// Desktop notification
    /// </summary>
    Desktop = 5,
    
    /// <summary>
    /// Webhook/API callback reminder
    /// </summary>
    Webhook = 6,
    
    /// <summary>
    /// Calendar popup reminder
    /// </summary>
    CalendarPopup = 7
}