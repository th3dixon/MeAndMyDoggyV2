using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to search appointments
/// </summary>
public class SearchAppointmentsRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Start date filter
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// End date filter
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Appointment type filter
    /// </summary>
    public AppointmentType? AppointmentType { get; set; }

    /// <summary>
    /// Status filter
    /// </summary>
    public AppointmentStatus? Status { get; set; }

    /// <summary>
    /// Priority filter
    /// </summary>
    public AppointmentPriority? Priority { get; set; }

    /// <summary>
    /// Location filter
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Participant email filter
    /// </summary>
    public string? ParticipantEmail { get; set; }

    /// <summary>
    /// Only show recurring appointments
    /// </summary>
    public bool? IsRecurring { get; set; }

    /// <summary>
    /// Only show all-day appointments
    /// </summary>
    public bool? IsAllDay { get; set; }

    /// <summary>
    /// Only show appointments with reminders
    /// </summary>
    public bool? HasReminders { get; set; }

    /// <summary>
    /// Color filter
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public AppointmentSortBy SortBy { get; set; } = AppointmentSortBy.StartTime;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Include cancelled appointments
    /// </summary>
    public bool IncludeCancelled { get; set; } = false;

    /// <summary>
    /// Include past appointments
    /// </summary>
    public bool IncludePast { get; set; } = true;

    /// <summary>
    /// Time zone for date filtering
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}