namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents an availability slot for a service provider
/// </summary>
public class AvailabilitySlot
{
    /// <summary>
    /// Unique identifier for the availability slot
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Start time of the availability slot
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
    
    /// <summary>
    /// End time of the availability slot
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
    
    /// <summary>
    /// Whether the slot is available for booking
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// Recurrence rule for repeating slots (RRULE format)
    /// </summary>
    public string? RecurrenceRule { get; set; }
    
    /// <summary>
    /// When the slot was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the slot was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}