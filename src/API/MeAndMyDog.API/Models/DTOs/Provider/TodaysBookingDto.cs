namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// Today's booking information
/// </summary>
public class TodaysBookingDto
{
    public string Id { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
}