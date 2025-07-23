namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Dashboard configuration data transfer object
/// </summary>
public class DashboardConfigDto
{
    public string[] WidgetLayout { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> Preferences { get; set; } = new();
    public UserInfoDto User { get; set; } = new();
}