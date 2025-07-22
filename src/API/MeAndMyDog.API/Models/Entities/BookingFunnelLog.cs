namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Booking funnel analytics tracking
/// </summary>
public class BookingFunnelLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string Metadata { get; set; } = "{}"; // JSON serialized metadata
    public DateTime Timestamp { get; set; }
    public string? BookingId { get; set; }
}