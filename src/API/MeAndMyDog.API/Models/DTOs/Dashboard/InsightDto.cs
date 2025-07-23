namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Individual insight
/// </summary>
public class InsightDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = "Medium"; // Low, Medium, High
    public double ConfidenceScore { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}