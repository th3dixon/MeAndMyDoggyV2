namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Compliance violation details
/// </summary>
public class ComplianceViolationDto
{
    /// <summary>
    /// Violation type
    /// </summary>
    public string ViolationType { get; set; } = string.Empty;

    /// <summary>
    /// Violation description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Severity level
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Number of occurrences
    /// </summary>
    public int Occurrences { get; set; }

    /// <summary>
    /// First occurrence date
    /// </summary>
    public DateTimeOffset FirstOccurrence { get; set; }

    /// <summary>
    /// Last occurrence date
    /// </summary>
    public DateTimeOffset LastOccurrence { get; set; }

    /// <summary>
    /// Affected users
    /// </summary>
    public List<string> AffectedUsers { get; set; } = new();

    /// <summary>
    /// Remediation status
    /// </summary>
    public string RemediationStatus { get; set; } = "Open";
}