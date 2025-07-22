using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Message access logging for security monitoring
/// </summary>
[Table("MessageAccessLogs")]
public class MessageAccessLog
{
    /// <summary>
    /// Access log unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Message security ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(MessageSecurity))]
    public string MessageSecurityId { get; set; } = string.Empty;

    /// <summary>
    /// User who accessed the message
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of access (view, download, print, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AccessType { get; set; } = string.Empty;

    /// <summary>
    /// When the access occurred
    /// </summary>
    [Required]
    public DateTimeOffset AccessedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Client IP address
    /// </summary>
    [MaxLength(45)]
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Client user agent
    /// </summary>
    [MaxLength(500)]
    public string? ClientUserAgent { get; set; }

    /// <summary>
    /// Client device fingerprint
    /// </summary>
    [MaxLength(200)]
    public string? DeviceFingerprint { get; set; }

    /// <summary>
    /// Geographic location (country/city)
    /// </summary>
    [MaxLength(100)]
    public string? GeographicLocation { get; set; }

    /// <summary>
    /// Whether the access was successful
    /// </summary>
    public bool AccessGranted { get; set; } = true;

    /// <summary>
    /// Reason if access was denied
    /// </summary>
    [MaxLength(200)]
    public string? DenialReason { get; set; }

    /// <summary>
    /// Security verification method used
    /// </summary>
    [MaxLength(50)]
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// Risk score calculated for this access
    /// </summary>
    public double RiskScore { get; set; } = 0.0;

    /// <summary>
    /// Whether this access triggered security alerts
    /// </summary>
    public bool TriggeredAlert { get; set; } = false;

    /// <summary>
    /// Session identifier for tracking
    /// </summary>
    [MaxLength(200)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Additional access metadata (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? AccessMetadata { get; set; }

    /// <summary>
    /// Navigation property to message security
    /// </summary>
    public virtual MessageSecurity MessageSecurity { get; set; } = null!;
}