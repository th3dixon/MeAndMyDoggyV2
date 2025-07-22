using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Advanced message security configuration entity
/// </summary>
[Table("MessageSecurities")]
public class MessageSecurity
{
    /// <summary>
    /// Security configuration unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Related message ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(Message))]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Security level applied to the message
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SecurityLevel { get; set; } = string.Empty;

    /// <summary>
    /// Whether the message requires authentication to view
    /// </summary>
    public bool RequiresAuthentication { get; set; } = false;

    /// <summary>
    /// Whether the message requires additional verification
    /// </summary>
    public bool RequiresVerification { get; set; } = false;

    /// <summary>
    /// Verification method required
    /// </summary>
    [MaxLength(50)]
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// Whether watermarking is applied
    /// </summary>
    public bool HasWatermark { get; set; } = false;

    /// <summary>
    /// Watermark text or identifier
    /// </summary>
    [MaxLength(200)]
    public string? WatermarkText { get; set; }

    /// <summary>
    /// Whether screenshots are blocked
    /// </summary>
    public bool BlockScreenshots { get; set; } = false;

    /// <summary>
    /// Whether copy/paste is blocked
    /// </summary>
    public bool BlockCopyPaste { get; set; } = false;

    /// <summary>
    /// Whether right-click is blocked
    /// </summary>
    public bool BlockRightClick { get; set; } = false;

    /// <summary>
    /// Whether forwarding is blocked
    /// </summary>
    public bool BlockForwarding { get; set; } = false;

    /// <summary>
    /// Whether the message can be downloaded
    /// </summary>
    public bool AllowDownload { get; set; } = true;

    /// <summary>
    /// Whether the message can be printed
    /// </summary>
    public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Access expiry date
    /// </summary>
    public DateTimeOffset? AccessExpiresAt { get; set; }

    /// <summary>
    /// Geographic restrictions (JSON array of country codes)
    /// </summary>
    [MaxLength(1000)]
    public string? GeographicRestrictions { get; set; }

    /// <summary>
    /// Time-based access restrictions (JSON configuration)
    /// </summary>
    [MaxLength(1000)]
    public string? TimeRestrictions { get; set; }

    /// <summary>
    /// Device restrictions (JSON array of device fingerprints)
    /// </summary>
    [MaxLength(2000)]
    public string? DeviceRestrictions { get; set; }

    /// <summary>
    /// IP address whitelist (JSON array)
    /// </summary>
    [MaxLength(2000)]
    public string? IpWhitelist { get; set; }

    /// <summary>
    /// IP address blacklist (JSON array)
    /// </summary>
    [MaxLength(2000)]
    public string? IpBlacklist { get; set; }

    /// <summary>
    /// Required security clearance level
    /// </summary>
    [MaxLength(50)]
    public string? RequiredClearanceLevel { get; set; }

    /// <summary>
    /// Data classification level
    /// </summary>
    [MaxLength(50)]
    public string? DataClassification { get; set; }

    /// <summary>
    /// Whether access is logged and audited
    /// </summary>
    public bool EnableAuditLogging { get; set; } = true;

    /// <summary>
    /// Whether to track detailed access analytics
    /// </summary>
    public bool EnableAccessAnalytics { get; set; } = false;

    /// <summary>
    /// Custom security policies (JSON)
    /// </summary>
    [MaxLength(4000)]
    public string? CustomPolicies { get; set; }

    /// <summary>
    /// When the security configuration was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the security configuration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Who configured the security settings
    /// </summary>
    [MaxLength(450)]
    public string? ConfiguredByUserId { get; set; }

    /// <summary>
    /// Navigation property to message
    /// </summary>
    public virtual Message Message { get; set; } = null!;

    /// <summary>
    /// Navigation property to access logs
    /// </summary>
    public virtual ICollection<MessageAccessLog> AccessLogs { get; set; } = new List<MessageAccessLog>();
}