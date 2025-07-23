namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// User information for dashboard
/// </summary>
public class UserInfoDto
{
    public string FirstName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
}