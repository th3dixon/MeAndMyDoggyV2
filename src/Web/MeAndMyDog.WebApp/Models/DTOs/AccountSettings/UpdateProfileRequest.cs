namespace MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

/// <summary>
/// Request model for updating user profile information
/// </summary>
public class UpdateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string TimeZone { get; set; }
    public string Language { get; set; }
}