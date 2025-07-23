namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// System alert
/// </summary>
public class AlertDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public string ActionUrl { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}