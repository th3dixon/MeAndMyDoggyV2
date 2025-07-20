namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a subscription plan available to users
/// </summary>
public class SubscriptionPlan
{
    /// <summary>
    /// Unique identifier for the subscription plan
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Name of the subscription plan
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the subscription plan
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Price of the subscription plan
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Billing cycle (Monthly, Annual, etc.)
    /// </summary>
    public string BillingCycle { get; set; } = "Monthly";
    
    /// <summary>
    /// Features included in the plan (JSON format)
    /// </summary>
    public string Features { get; set; } = string.Empty;
    
    /// <summary>
    /// Maximum number of dog profiles allowed
    /// </summary>
    public int MaxDogProfiles { get; set; } = 1;
    
    /// <summary>
    /// Maximum number of appointments per month
    /// </summary>
    public int MaxAppointments { get; set; } = 10;
    
    /// <summary>
    /// Whether AI features are included
    /// </summary>
    public bool HasAIFeatures { get; set; } = false;
    
    /// <summary>
    /// Whether priority support is included
    /// </summary>
    public bool HasPrioritySupport { get; set; } = false;
    
    /// <summary>
    /// Whether the plan is currently active/available
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the plan was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the plan was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to user subscriptions
    /// </summary>
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}