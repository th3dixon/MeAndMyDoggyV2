namespace MeAndMyDog.API.Models.DTOs.Dashboard;

/// <summary>
/// Business opportunity
/// </summary>
public class OpportunityDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double EstimatedImpact { get; set; }
    public string ImpactUnit { get; set; } = "percentage";
    public string Priority { get; set; } = "Medium";
    public double ImplementationEffort { get; set; }
    public List<string> RequiredActions { get; set; } = new();
    public DateTime IdentifiedAt { get; set; } = DateTime.UtcNow;
}