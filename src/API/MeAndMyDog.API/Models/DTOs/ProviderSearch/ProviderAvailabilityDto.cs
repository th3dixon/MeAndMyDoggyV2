namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for provider availability information
/// </summary>
public class ProviderAvailabilityDto
{
    /// <summary>
    /// Provider identifier
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the provider is available for the requested time period
    /// </summary>
    public bool IsAvailable { get; set; }
    
    /// <summary>
    /// Number of available time slots in the requested period
    /// </summary>
    public int AvailableSlots { get; set; }
    
    /// <summary>
    /// Next available appointment slot
    /// </summary>
    public DateTimeOffset? NextAvailableSlot { get; set; }
    
    /// <summary>
    /// Maximum capacity (number of pets) for the available slots
    /// </summary>
    public int MaxCapacity { get; set; }
    
    /// <summary>
    /// Response time in minutes for booking confirmations
    /// </summary>
    public int ResponseTimeMinutes { get; set; }
    
    /// <summary>
    /// Additional message about availability
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// List of specific available time periods
    /// </summary>
    public List<AvailabilitySlotDto> AvailableTimeSlots { get; set; } = new();
}