namespace MeAndMyDog.WebApp.Models.DTOs.UserProfile
{
    /// <summary>
    /// DTO for notification preferences
    /// </summary>
    public class NotificationPreferencesDto
    {
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool PushNotifications { get; set; }
        
        public bool BookingReminders { get; set; }
        public bool NewMessages { get; set; }
        public bool ServiceUpdates { get; set; }
        public bool MarketingCommunications { get; set; }
        
        public string QuietHoursStart { get; set; }
        public string QuietHoursEnd { get; set; }
    }
}