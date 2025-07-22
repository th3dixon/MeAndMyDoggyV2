namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Calendar availability response
/// </summary>
public class CheckAvailabilityResponse
{
    /// <summary>
    /// Availability for each requested user
    /// </summary>
    public List<UserAvailabilityDto> UserAvailability { get; set; } = new();

    /// <summary>
    /// Common available time slots for all users
    /// </summary>
    public List<AvailableTimeSlotDto> CommonAvailableSlots { get; set; } = new();

    /// <summary>
    /// Conflicting appointments during the requested time
    /// </summary>
    public List<AppointmentConflictDto> Conflicts { get; set; } = new();

    /// <summary>
    /// Suggested alternative time slots
    /// </summary>
    public List<AvailableTimeSlotDto> SuggestedAlternatives { get; set; } = new();
}