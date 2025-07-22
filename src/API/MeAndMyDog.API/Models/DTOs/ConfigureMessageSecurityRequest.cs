using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to configure message security
/// </summary>
public class ConfigureMessageSecurityRequest
{
    /// <summary>
    /// Message ID to secure
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Security level to apply
    /// </summary>
    public SecurityLevel SecurityLevel { get; set; } = SecurityLevel.Standard;

    /// <summary>
    /// Whether to require authentication
    /// </summary>
    public bool RequiresAuthentication { get; set; } = false;

    /// <summary>
    /// Whether to require additional verification
    /// </summary>
    public bool RequiresVerification { get; set; } = false;

    /// <summary>
    /// Verification method if required
    /// </summary>
    public VerificationMethod? VerificationMethod { get; set; }

    /// <summary>
    /// Whether to apply watermarking
    /// </summary>
    public bool HasWatermark { get; set; } = false;

    /// <summary>
    /// Watermark text
    /// </summary>
    public string? WatermarkText { get; set; }

    /// <summary>
    /// Whether to block screenshots
    /// </summary>
    public bool BlockScreenshots { get; set; } = false;

    /// <summary>
    /// Whether to block copy/paste
    /// </summary>
    public bool BlockCopyPaste { get; set; } = false;

    /// <summary>
    /// Whether to block right-click
    /// </summary>
    public bool BlockRightClick { get; set; } = false;

    /// <summary>
    /// Whether to block forwarding
    /// </summary>
    public bool BlockForwarding { get; set; } = false;

    /// <summary>
    /// Whether to allow downloads
    /// </summary>
    public bool AllowDownload { get; set; } = true;

    /// <summary>
    /// Whether to allow printing
    /// </summary>
    public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Access expiry date
    /// </summary>
    public DateTimeOffset? AccessExpiresAt { get; set; }

    /// <summary>
    /// Geographic restrictions (country codes)
    /// </summary>
    public List<string> GeographicRestrictions { get; set; } = new();

    /// <summary>
    /// Time-based access restrictions
    /// </summary>
    public TimeRestrictionDto? TimeRestrictions { get; set; }

    /// <summary>
    /// Device restrictions (fingerprints)
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
    /// Required clearance level
    /// </summary>
    public string? RequiredClearanceLevel { get; set; }

    /// <summary>
    /// Data classification
    /// </summary>
    public string? DataClassification { get; set; }

    /// <summary>
    /// Whether to enable audit logging
    /// </summary>
    public bool EnableAuditLogging { get; set; } = true;

    /// <summary>
    /// Whether to enable access analytics
    /// </summary>
    public bool EnableAccessAnalytics { get; set; } = false;
}