using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update a recurring appointment instance
/// </summary>
public class UpdateRecurringInstanceRequest
{
    /// <summary>
    /// Custom title for this instance (if different from parent)
    /// </summary>
    public string? CustomTitle { get; set; }

    /// <summary>
    /// Custom description for this instance
    /// </summary>
    public string? CustomDescription { get; set; }

    /// <summary>
    /// Custom location for this instance
    /// </summary>
    public string? CustomLocation { get; set; }

    /// <summary>
    /// New start time for this instance
    /// </summary>
    public DateTimeOffset? ActualStartTime { get; set; }

    /// <summary>
    /// New end time for this instance
    /// </summary>
    public DateTimeOffset? ActualEndTime { get; set; }

    /// <summary>
    /// Instance status
    /// </summary>
    public AppointmentStatus? Status { get; set; }

    /// <summary>
    /// Additional notes for this instance
    /// </summary>
    public string? Notes { get; set; }
}