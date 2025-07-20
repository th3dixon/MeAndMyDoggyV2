namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Tracks AI service usage for billing and analytics
/// </summary>
public class AIUsageTracking
{
    /// <summary>
    /// Unique identifier for the usage record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the user (nullable for system-level usage)
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Type of AI feature used (health_recommendation, content_moderation, etc.)
    /// </summary>
    public string FeatureType { get; set; } = string.Empty;
    
    /// <summary>
    /// AI model that was used
    /// </summary>
    public string ModelUsed { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of tokens consumed
    /// </summary>
    public int TokensUsed { get; set; }
    
    /// <summary>
    /// Cost of the AI request
    /// </summary>
    public decimal Cost { get; set; }
    
    /// <summary>
    /// When the AI service was used
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
    
    /// <summary>
    /// Additional metadata (JSON format)
    /// </summary>
    public string? Metadata { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser? User { get; set; }
}