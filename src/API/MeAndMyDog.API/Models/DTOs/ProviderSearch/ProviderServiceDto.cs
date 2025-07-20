namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// DTO for provider service information
/// </summary>
public class ProviderServiceDto
{
    /// <summary>
    /// Service identifier
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;
    
    /// <summary>
    /// Main service category name (e.g., "Dog Walking")
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;
    
    /// <summary>
    /// Sub-service name (e.g., "Standard Walk", "Premium Walk")
    /// </summary>
    public string SubServiceName { get; set; } = string.Empty;
    
    /// <summary>
    /// Base price for this service
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Currency code (e.g., "GBP")
    /// </summary>
    public string Currency { get; set; } = "GBP";
    
    /// <summary>
    /// Typical service duration in minutes
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Maximum number of pets for this service
    /// </summary>
    public int MaxPets { get; set; }
    
    /// <summary>
    /// Whether this service is available for emergency bookings
    /// </summary>
    public bool IsEmergencyAvailable { get; set; }
    
    /// <summary>
    /// Whether weekend surcharge applies
    /// </summary>
    public bool HasWeekendSurcharge { get; set; }
    
    /// <summary>
    /// Whether evening surcharge applies
    /// </summary>
    public bool HasEveningSurcharge { get; set; }
    
    /// <summary>
    /// Brief description of what this service includes
    /// </summary>
    public string? Description { get; set; }
}