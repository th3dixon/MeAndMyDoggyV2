namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Appointment search results
/// </summary>
public class SearchAppointmentsResponse
{
    /// <summary>
    /// Found appointments
    /// </summary>
    public List<CalendarAppointmentDto> Appointments { get; set; } = new();

    /// <summary>
    /// Total number of appointments found
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there are more results
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Whether there are previous results
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Applied filters summary
    /// </summary>
    public List<string> AppliedFilters { get; set; } = new();
}