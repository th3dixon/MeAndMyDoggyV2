namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile view preferences
/// </summary>
public class MobileViewPreferences
{
    public List<string> EnabledWidgets { get; set; } = new();
    public string Theme { get; set; } = "system"; // light, dark, system
    public int MaxItemsPerWidget { get; set; } = 5;
    public bool ReduceAnimations { get; set; }
    public bool HighContrast { get; set; }
    public string DataUsageMode { get; set; } = "normal"; // minimal, normal, full
}