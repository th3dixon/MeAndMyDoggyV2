namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Notification preferences
/// </summary>
public class NotificationPreferencesDto
{
    public bool AppointmentReminders { get; set; } = true;
    public bool BookingConfirmations { get; set; } = true;
    public bool HealthReminders { get; set; } = true;
    public bool ProviderMessages { get; set; } = true;
    public bool SystemAnnouncements { get; set; } = true;
    public bool MarketingOffers { get; set; } = false;
    public Dictionary<string, bool> CustomPreferences { get; set; } = new();
    public string QuietHoursStart { get; set; } = "22:00";
    public string QuietHoursEnd { get; set; } = "07:00";
    public bool WeekendNotifications { get; set; } = true;
}