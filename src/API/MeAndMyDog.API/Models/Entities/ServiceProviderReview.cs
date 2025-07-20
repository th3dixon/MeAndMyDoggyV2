namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a review for a service provider
/// </summary>
public class ServiceProviderReview
{
    /// <summary>
    /// Unique identifier for the review
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the reviewer
    /// </summary>
    public string ReviewerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Rating (1-5 stars)
    /// </summary>
    public int Rating { get; set; }
    
    /// <summary>
    /// Review title
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Review comment
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// When the review was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Whether the review is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the reviewer
    /// </summary>
    public virtual ApplicationUser Reviewer { get; set; } = null!;
}