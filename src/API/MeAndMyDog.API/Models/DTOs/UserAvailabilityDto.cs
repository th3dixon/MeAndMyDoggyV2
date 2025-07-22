namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// User availability information
/// </summary>
public class UserAvailabilityDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// User display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Whether user is available for the entire requested time
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Available time slots for this user
    /// </summary>
    public List<AvailableTimeSlotDto> AvailableSlots { get; set; } = new();

    /// <summary>
    /// Conflicting appointments for this user
    /// </summary>
    public List<AppointmentConflictDto> Conflicts { get; set; } = new();

    /// <summary>
    /// User's time zone
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}