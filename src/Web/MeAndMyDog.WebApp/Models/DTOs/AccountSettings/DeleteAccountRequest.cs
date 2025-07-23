namespace MeAndMyDog.WebApp.Models.DTOs.AccountSettings;

/// <summary>
/// Request model for account deletion
/// </summary>
public class DeleteAccountRequest
{
    public string Password { get; set; }
    public string Reason { get; set; }
}