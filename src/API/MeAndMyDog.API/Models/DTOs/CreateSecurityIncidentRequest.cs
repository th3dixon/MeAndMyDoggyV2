using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a security incident
/// </summary>
public class CreateSecurityIncidentRequest
{
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
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;

    /// <summary>
    /// Incident description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// When the incident occurred (optional, defaults to now)
    /// </summary>
    public DateTimeOffset? OccurredAt { get; set; }

    /// <summary>
    /// Detection method used
    /// </summary>
    public string? DetectionMethod { get; set; }

    /// <summary>
    /// Client information at time of incident
    /// </summary>
    public ClientInformationDto? ClientInformation { get; set; }

    /// <summary>
    /// Initial risk assessment score
    /// </summary>
    public double RiskScore { get; set; } = 0.0;
}