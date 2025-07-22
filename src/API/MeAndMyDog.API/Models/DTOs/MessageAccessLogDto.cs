namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Message access log DTO
/// </summary>
public class MessageAccessLogDto
{
    /// <summary>
    /// Access log unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Message security ID
    /// </summary>
    public string MessageSecurityId { get; set; } = string.Empty;

    /// <summary>
    /// User who accessed the message
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of access
    /// </summary>
    public string AccessType { get; set; } = string.Empty;

    /// <summary>
    /// When the access occurred
    /// </summary>
    public DateTimeOffset AccessedAt { get; set; }

    /// <summary>
    /// Client IP address
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Client user agent
    /// </summary>
    public string? ClientUserAgent { get; set; }

    /// <summary>
    /// Client device fingerprint
    /// </summary>
    public string? DeviceFingerprint { get; set; }

    /// <summary>
    /// Geographic location
    /// </summary>
    public string? GeographicLocation { get; set; }

    /// <summary>
    /// Whether the access was successful
    /// </summary>
    public bool AccessGranted { get; set; }

    /// <summary>
    /// Reason if access was denied
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Verification method used
    /// </summary>
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// Risk score calculated for this access
    /// </summary>
    public double RiskScore { get; set; }

    /// <summary>
    /// Whether this access triggered security alerts
    /// </summary>
    public bool TriggeredAlert { get; set; }

    /// <summary>
    /// Session identifier
    /// </summary>
    public string? SessionId { get; set; }
}