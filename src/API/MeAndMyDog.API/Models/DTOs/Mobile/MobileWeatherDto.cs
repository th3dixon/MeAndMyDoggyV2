namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile weather data
/// </summary>
public class MobileWeatherDto
{
    public int Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string PetTip { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}