namespace MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

/// <summary>
/// Request model for updating notification preferences
/// </summary>
public class NotificationPreferencesRequest
{
    public bool EmailNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool MarketingEmails { get; set; }
    public bool BookingReminders { get; set; }
    public bool NewMessages { get; set; }
    public bool ServiceUpdates { get; set; }
}