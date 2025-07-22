namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// External calendar providers for integration
/// </summary>
public enum CalendarProvider
{
    /// <summary>
    /// Google Calendar integration
    /// </summary>
    Google = 0,
    
    /// <summary>
    /// Microsoft Outlook/Office 365 Calendar
    /// </summary>
    Outlook = 1,
    
    /// <summary>
    /// Apple iCloud Calendar
    /// </summary>
    iCloud = 2,
    
    /// <summary>
    /// Yahoo Calendar
    /// </summary>
    Yahoo = 3,
    
    /// <summary>
    /// CalDAV compatible calendar
    /// </summary>
    CalDAV = 4,
    
    /// <summary>
    /// Exchange Server calendar
    /// </summary>
    Exchange = 5,
    
    /// <summary>
    /// Thunderbird/Lightning calendar
    /// </summary>
    Thunderbird = 6,
    
    /// <summary>
    /// Internal/native calendar system
    /// </summary>
    Internal = 7,
    
    /// <summary>
    /// Other/custom calendar provider
    /// </summary>
    Other = 8
}