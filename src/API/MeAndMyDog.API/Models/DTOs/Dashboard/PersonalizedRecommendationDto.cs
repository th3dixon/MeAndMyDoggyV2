namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Personalized recommendation
/// </summary>
public class PersonalizedRecommendationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty; // service, provider, feature, etc.
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public string ReasoningText { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionText { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}