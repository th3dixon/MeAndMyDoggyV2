using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for security incident
/// </summary>
public class SecurityIncidentDto
{
    /// <summary>
    /// Security incident unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Related message ID (optional)
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Related conversation ID (optional)
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// User involved in the incident
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of security incident
    /// </summary>
    public IncidentType IncidentType { get; set; }

    /// <summary>
    /// Severity level of the incident
    /// </summary>
    public IncidentSeverity Severity { get; set; }

    /// <summary>
    /// Incident description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// When the incident occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// When the incident was detected
    /// </summary>
    public DateTimeOffset DetectedAt { get; set; }

    /// <summary>
    /// Detection method used
    /// </summary>
    public string? DetectionMethod { get; set; }

    /// <summary>
    /// Current status of the incident
    /// </summary>
    public IncidentStatus Status { get; set; }

    /// <summary>
    /// Assigned investigator
    /// </summary>
    public string? AssignedInvestigator { get; set; }

    /// <summary>
    /// Investigation notes
    /// </summary>
    public string? InvestigationNotes { get; set; }

    /// <summary>
    /// Remediation actions taken
    /// </summary>
    public string? RemediationActions { get; set; }

    /// <summary>
    /// When the incident was resolved
    /// </summary>
    public DateTimeOffset? ResolvedAt { get; set; }

    /// <summary>
    /// Resolution summary
    /// </summary>
    public string? ResolutionSummary { get; set; }

    /// <summary>
    /// Client information at time of incident
    /// </summary>
    public ClientInformationDto? ClientInformation { get; set; }

    /// <summary>
    /// Risk assessment score
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// Whether authorities were notified
    /// </summary>
    public bool AuthoritiesNotified { get; set; }

    /// <summary>
    /// Whether users were notified
    /// </summary>
    public bool UsersNotified { get; set; }

    /// <summary>
    /// Time elapsed since detection
    /// </summary>
    public TimeSpan TimeElapsed => DateTimeOffset.UtcNow - DetectedAt;

    /// <summary>
    /// Whether the incident is still open
    /// </summary>
    public bool IsOpen => Status != IncidentStatus.Resolved && Status != IncidentStatus.Closed && Status != IncidentStatus.FalsePositive;

    /// <summary>
    /// Time to resolve the incident
    /// </summary>
    public TimeSpan? TimeToResolve => ResolvedAt.HasValue ? ResolvedAt.Value - DetectedAt : null;
}