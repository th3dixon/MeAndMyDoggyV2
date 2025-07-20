namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for individual availability time slots
/// </summary>
public class AvailabilitySlotDto
{
    /// <summary>
    /// Start time of the availability slot
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
    
    /// <summary>
    /// End time of the availability slot
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
    
    /// <summary>
    /// Whether this slot is currently available for booking
    /// </summary>
    public bool IsAvailable { get; set; }
    
    /// <summary>
    /// Maximum number of pets that can be accommodated in this slot
    /// </summary>
    public int MaxPets { get; set; }
    
    /// <summary>
    /// Current number of pets already booked for this slot
    /// </summary>
    public int BookedPets { get; set; }
    
    /// <summary>
    /// Price per pet for this time slot
    /// </summary>
    public decimal PricePerPet { get; set; }
    
    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "GBP";
    
    /// <summary>
    /// Whether this is an emergency/urgent slot
    /// </summary>
    public bool IsEmergencySlot { get; set; }
    
    /// <summary>
    /// Any special notes about this time slot
    /// </summary>
    public string? Notes { get; set; }
}