namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents an AI-generated health recommendation for pets
/// </summary>
public class AIHealthRecommendation
{
    /// <summary>
    /// Unique identifier for the health recommendation
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Foreign key to the user who received this recommendation
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>
    /// Foreign key to the dog this recommendation is for (optional for general recommendations)
    /// </summary>
    public string? DogId { get; set; }
    /// <summary>
    /// Type of recommendation (Health, Nutrition, Exercise, Training, etc.)
    /// </summary>
    public string RecommendationType { get; set; } = string.Empty;
    /// <summary>
    /// Title of the recommendation
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Detailed description of the recommendation
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// AI reasoning behind the recommendation
    /// </summary>
    public string? Reasoning { get; set; }
    /// <summary>
    /// AI confidence score for this recommendation (0.0 to 1.0)
    /// </summary>
    public decimal? Confidence { get; set; }
    /// <summary>
    /// JSON data that was input to the AI model
    /// </summary>
    public string? InputData { get; set; }
    /// <summary>
    /// Name of the AI model used to generate this recommendation
    /// </summary>
    public string ModelUsed { get; set; } = "gemini-1.5-flash";
    /// <summary>
    /// Cost in GBP for generating this recommendation
    /// </summary>
    public decimal Cost { get; set; } = 0.0m;
    /// <summary>
    /// Implementation status: Pending, Implemented, Dismissed
    /// </summary>
    public string ImplementationStatus { get; set; } = "Pending";
    /// <summary>
    /// When this recommendation was generated
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// When this recommendation was implemented (if applicable)
    /// </summary>
    public DateTimeOffset? ImplementedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
    /// <summary>
    /// Navigation property to the dog (optional)
    /// </summary>
    public virtual DogProfile? Dog { get; set; }
}