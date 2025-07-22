using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Security incident tracking for messages
/// </summary>
[Table("SecurityIncidents")]
public class SecurityIncident
{
    /// <summary>
    /// Security incident unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Related message ID (optional)
    /// </summary>
    [MaxLength(450)]
    public string? MessageId { get; set; }

    /// <summary>
    /// Related conversation ID (optional)
    /// </summary>
    [MaxLength(450)]
    public string? ConversationId { get; set; }

    /// <summary>
    /// User involved in the incident
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of security incident
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string IncidentType { get; set; } = string.Empty;

    /// <summary>
    /// Severity level of the incident
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Incident description
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// When the incident occurred
    /// </summary>
    [Required]
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the incident was detected
    /// </summary>
    public DateTimeOffset DetectedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Detection method used
    /// </summary>
    [MaxLength(50)]
    public string? DetectionMethod { get; set; }

    /// <summary>
    /// Current status of the incident
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Assigned investigator
    /// </summary>
    [MaxLength(450)]
    public string? AssignedInvestigator { get; set; }

    /// <summary>
    /// Investigation notes
    /// </summary>
    [MaxLength(4000)]
    public string? InvestigationNotes { get; set; }

    /// <summary>
    /// Remediation actions taken
    /// </summary>
    [MaxLength(2000)]
    public string? RemediationActions { get; set; }

    /// <summary>
    /// When the incident was resolved
    /// </summary>
    public DateTimeOffset? ResolvedAt { get; set; }

    /// <summary>
    /// Resolution summary
    /// </summary>
    [MaxLength(1000)]
    public string? ResolutionSummary { get; set; }

    /// <summary>
    /// Client information at time of incident (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? ClientInformation { get; set; }

    /// <summary>
    /// Risk assessment score
    /// </summary>
    public double RiskScore { get; set; } = 0.0;

    /// <summary>
    /// Whether authorities were notified
    /// </summary>
    public bool AuthoritiesNotified { get; set; } = false;

    /// <summary>
    /// Whether users were notified
    /// </summary>
    public bool UsersNotified { get; set; } = false;

    /// <summary>
    /// Additional incident data (JSON)
    /// </summary>
    [MaxLength(4000)]
    public string? IncidentData { get; set; }
}