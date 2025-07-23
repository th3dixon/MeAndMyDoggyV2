namespace MeAndMyDog.WebApp.Models.DTOs.RoleSwitcher;

/// <summary>
/// Request model for role switching
/// </summary>
public class SwitchRoleRequest
{
    public string Role { get; set; } = string.Empty;
}