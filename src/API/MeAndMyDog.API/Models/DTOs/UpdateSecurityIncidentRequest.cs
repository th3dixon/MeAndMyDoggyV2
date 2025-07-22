using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update a security incident
/// </summary>
public class UpdateSecurityIncidentRequest
{
    /// <summary>
    /// Updated severity level
    /// </summary>
    public IncidentSeverity? Severity { get; set; }

    /// <summary>
    /// Updated status
    /// </summary>
    public IncidentStatus? Status { get; set; }

    /// <summary>
    /// Assign investigator
    /// </summary>
    public string? AssignedInvestigator { get; set; }

    /// <summary>
    /// Investigation notes to add
    /// </summary>
    public string? InvestigationNotes { get; set; }

    /// <summary>
    /// Remediation actions taken
    /// </summary>
    public string? RemediationActions { get; set; }

    /// <summary>
    /// Resolution summary (when resolving)
    /// </summary>
    public string? ResolutionSummary { get; set; }

    /// <summary>
    /// Updated risk score
    /// </summary>
    public double? RiskScore { get; set; }

    /// <summary>
    /// Whether authorities were notified
    /// </summary>
    public bool? AuthoritiesNotified { get; set; }

    /// <summary>
    /// Whether users were notified
    /// </summary>
    public bool? UsersNotified { get; set; }
}