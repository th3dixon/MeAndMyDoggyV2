using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for message security configuration
/// </summary>
public class MessageSecurityDto
{
    /// <summary>
    /// Security configuration unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Related message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Security level applied to the message
    /// </summary>
    public SecurityLevel SecurityLevel { get; set; }

    /// <summary>
    /// Whether the message requires authentication to view
    /// </summary>
    public bool RequiresAuthentication { get; set; }

    /// <summary>
    /// Whether the message requires additional verification
    /// </summary>
    public bool RequiresVerification { get; set; }

    /// <summary>
    /// Verification method required
    /// </summary>
    public VerificationMethod? VerificationMethod { get; set; }

    /// <summary>
    /// Whether watermarking is applied
    /// </summary>
    public bool HasWatermark { get; set; }

    /// <summary>
    /// Watermark text or identifier
    /// </summary>
    public string? WatermarkText { get; set; }

    /// <summary>
    /// Whether screenshots are blocked
    /// </summary>
    public bool BlockScreenshots { get; set; }

    /// <summary>
    /// Whether copy/paste is blocked
    /// </summary>
    public bool BlockCopyPaste { get; set; }

    /// <summary>
    /// Whether right-click is blocked
    /// </summary>
    public bool BlockRightClick { get; set; }

    /// <summary>
    /// Whether forwarding is blocked
    /// </summary>
    public bool BlockForwarding { get; set; }

    /// <summary>
    /// Whether the message can be downloaded
    /// </summary>
    public bool AllowDownload { get; set; }

    /// <summary>
    /// Whether the message can be printed
    /// </summary>
    public bool AllowPrint { get; set; }

    /// <summary>
    /// Access expiry date
    /// </summary>
    public DateTimeOffset? AccessExpiresAt { get; set; }

    /// <summary>
    /// Geographic restrictions
    /// </summary>
    public List<string> GeographicRestrictions { get; set; } = new();

    /// <summary>
    /// Time-based access restrictions
    /// </summary>
    public TimeRestrictionDto? TimeRestrictions { get; set; }

    /// <summary>
    /// Device restrictions
    /// </summary>
    public List<string> DeviceRestrictions { get; set; } = new();

    /// <summary>
    /// IP address whitelist
    /// </summary>
    public List<string> IpWhitelist { get; set; } = new();

    /// <summary>
    /// IP address blacklist
    /// </summary>
    public List<string> IpBlacklist { get; set; } = new();

    /// <summary>
    /// Required security clearance level
    /// </summary>
    public string? RequiredClearanceLevel { get; set; }

    /// <summary>
    /// Data classification level
    /// </summary>
    public string? DataClassification { get; set; }

    /// <summary>
    /// Whether access is logged and audited
    /// </summary>
    public bool EnableAuditLogging { get; set; }

    /// <summary>
    /// Whether to track detailed access analytics
    /// </summary>
    public bool EnableAccessAnalytics { get; set; }

    /// <summary>
    /// Whether access has expired
    /// </summary>
    public bool IsAccessExpired => AccessExpiresAt.HasValue && AccessExpiresAt.Value <= DateTimeOffset.UtcNow;

    /// <summary>
    /// Time until access expires
    /// </summary>
    public TimeSpan? TimeUntilExpiry => AccessExpiresAt.HasValue ? AccessExpiresAt.Value - DateTimeOffset.UtcNow : null;

    /// <summary>
    /// When the security configuration was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the security configuration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Who configured the security settings
    /// </summary>
    public string? ConfiguredByUserId { get; set; }
}
