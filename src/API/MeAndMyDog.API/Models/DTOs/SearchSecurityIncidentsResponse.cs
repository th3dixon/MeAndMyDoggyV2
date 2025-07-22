namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Security incident search results
/// </summary>
public class SearchSecurityIncidentsResponse
{
    /// <summary>
    /// Found incidents
    /// </summary>
    public List<SecurityIncidentDto> Incidents { get; set; } = new();

    /// <summary>
    /// Total number of incidents found
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
}