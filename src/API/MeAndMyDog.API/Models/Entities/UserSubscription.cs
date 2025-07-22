namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a user's subscription to a service plan
/// </summary>
public class UserSubscription
{
    /// <summary>
    /// Gets or sets the unique identifier for the user subscription
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Gets or sets the unique identifier of the user who owns this subscription
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the unique identifier of the subscription plan associated with this subscription
    /// </summary>
    public string SubscriptionPlanId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the date and time when the subscription started
    /// </summary>
    public DateTimeOffset StartDate { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the subscription ended, or null if the subscription is still active
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    /// <summary>
    /// Gets or sets the current status of the subscription (e.g., Active, Inactive, Cancelled)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets whether the subscription is currently active
    /// </summary>
    public bool IsActive => Status.Equals("Active", StringComparison.OrdinalIgnoreCase) && 
                           (EndDate == null || EndDate > DateTimeOffset.UtcNow);
    /// <summary>
    /// Gets or sets the amount paid for this subscription
    /// </summary>
    public decimal PaidAmount { get; set; }
    /// <summary>
    /// Gets or sets the date and time of the next billing cycle, or null if not applicable
    /// </summary>
    public DateTimeOffset? NextBillingDate { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the payment method used for this subscription, or null if not specified
    /// </summary>
    public string? PaymentMethodId { get; set; }
    /// <summary>
    /// Gets or sets the date and time when this subscription record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the date and time when this subscription record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the user who owns this subscription
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
    /// <summary>
    /// Gets or sets the navigation property to the subscription plan associated with this subscription
    /// </summary>
    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}