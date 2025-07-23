namespace MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

/// <summary>
/// Request model for changing user password
/// </summary>
public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}