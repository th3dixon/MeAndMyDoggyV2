using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Security incident search request
/// </summary>
public class SearchSecurityIncidentsRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Incident type filter
    /// </summary>
    public IncidentType? IncidentType { get; set; }

    /// <summary>
    /// Severity filter
    /// </summary>
    public IncidentSeverity? Severity { get; set; }

    /// <summary>
    /// Status filter
    /// </summary>
    public IncidentStatus? Status { get; set; }

    /// <summary>
    /// User ID filter
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Assigned investigator filter
    /// </summary>
    public string? AssignedInvestigator { get; set; }

    /// <summary>
    /// From date filter
    /// </summary>
    public DateTimeOffset? FromDate { get; set; }

    /// <summary>
    /// To date filter
    /// </summary>
    public DateTimeOffset? ToDate { get; set; }

    /// <summary>
    /// Minimum risk score
    /// </summary>
    public double? MinRiskScore { get; set; }

    /// <summary>
    /// Maximum risk score
    /// </summary>
    public double? MaxRiskScore { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public IncidentSortBy SortBy { get; set; } = IncidentSortBy.DetectedAt;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;
}