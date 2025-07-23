namespace MeAndMyDog.WebApp.Models.DTOs.UserProfile
{
    /// <summary>
    /// DTO for privacy settings
    /// </summary>
    public class PrivacySettingsDto
    {
        public bool ProfilePublic { get; set; }
        public bool ShowLocation { get; set; }
        public bool AllowDataSharing { get; set; }
        public bool AllowAnalytics { get; set; }
        public bool ShowOnlineStatus { get; set; }
    }
}