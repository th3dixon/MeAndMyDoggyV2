namespace MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

/// <summary>
/// Request model for updating privacy settings
/// </summary>
public class PrivacySettingsRequest
{
    public bool ProfileVisible { get; set; }
    public bool ShowLocation { get; set; }
    public bool ShareDataWithPartners { get; set; }
    public bool AllowAnalytics { get; set; }
}