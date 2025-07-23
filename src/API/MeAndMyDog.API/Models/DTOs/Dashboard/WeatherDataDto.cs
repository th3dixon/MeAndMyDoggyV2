namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Weather data for dashboard
/// </summary>
public class WeatherDataDto
{
    public int Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int FeelsLike { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string PetTip { get; set; } = string.Empty;
    public CoordinatesDto? Coordinates { get; set; }
    public string Source { get; set; } = string.Empty;
}