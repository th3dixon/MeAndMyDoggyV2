namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile quick action
/// </summary>
public class MobileQuickActionDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007AFF";
    public bool Enabled { get; set; } = true;
}