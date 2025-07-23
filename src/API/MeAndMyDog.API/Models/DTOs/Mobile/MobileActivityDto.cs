namespace MeAndMyDog.API.Models.DTOs.Mobile;

/// <summary>
/// Mobile activity item
/// </summary>
public class MobileActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
}