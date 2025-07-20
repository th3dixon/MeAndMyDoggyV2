namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents AI-powered content moderation results
/// </summary>
public class AIContentModeration
{
    /// <summary>
    /// Unique identifier for the moderation record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Type of content being moderated (message, review, profile, etc.)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the content being moderated
    /// </summary>
    public string ContentId { get; set; } = string.Empty;
    
    /// <summary>
    /// The actual content that was moderated
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Result of the moderation (approved, flagged, blocked, etc.)
    /// </summary>
    public string ModerationResult { get; set; } = string.Empty;
    
    /// <summary>
    /// Toxicity score from 0.0 to 1.0
    /// </summary>
    public decimal? ToxicityScore { get; set; }
    
    /// <summary>
    /// JSON array of flags detected (spam, harassment, inappropriate, etc.)
    /// </summary>
    public string? Flags { get; set; }
    
    /// <summary>
    /// AI model used for moderation
    /// </summary>
    public string ModelUsed { get; set; } = "gemini-1.5-flash";
    
    /// <summary>
    /// Cost of the AI moderation request
    /// </summary>
    public decimal Cost { get; set; } = 0.0m;
    
    /// <summary>
    /// When the content was processed
    /// </summary>
    public DateTimeOffset ProcessedAt { get; set; }
    
    /// <summary>
    /// User ID who created the content (if applicable)
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Navigation property to the user who created the content
    /// </summary>
    public virtual ApplicationUser? User { get; set; }
}