namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Dashboard preferences model for saving user customizations
/// </summary>
public class DashboardPreferences
{
    public string[]? WidgetOrder { get; set; }
    public Dictionary<string, string>? WidgetSizes { get; set; }
    public string[]? HiddenWidgets { get; set; }
    public string? Theme { get; set; }
}