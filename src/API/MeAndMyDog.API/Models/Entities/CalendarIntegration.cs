using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Calendar integration settings entity
/// </summary>
[Table("CalendarIntegrations")]
public class CalendarIntegration
{
    /// <summary>
    /// Integration unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// User ID
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// External calendar provider
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// External calendar ID
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string ExternalCalendarId { get; set; } = string.Empty;

    /// <summary>
    /// Calendar display name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CalendarName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this integration is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether to sync appointments to external calendar
    /// </summary>
    public bool SyncToExternal { get; set; } = true;

    /// <summary>
    /// Whether to sync appointments from external calendar
    /// </summary>
    public bool SyncFromExternal { get; set; } = true;

    /// <summary>
    /// Sync direction
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SyncDirection { get; set; } = string.Empty;

    /// <summary>
    /// Access token for external calendar API
    /// </summary>
    [MaxLength(2000)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token for external calendar API
    /// </summary>
    [MaxLength(2000)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the access token expires
    /// </summary>
    public DateTimeOffset? TokenExpiresAt { get; set; }

    /// <summary>
    /// Last successful sync time
    /// </summary>
    public DateTimeOffset? LastSyncAt { get; set; }

    /// <summary>
    /// Next scheduled sync time
    /// </summary>
    public DateTimeOffset? NextSyncAt { get; set; }

    /// <summary>
    /// Sync frequency in minutes
    /// </summary>
    public int SyncFrequencyMinutes { get; set; } = 60;

    /// <summary>
    /// Whether automatic sync is enabled
    /// </summary>
    public bool AutoSyncEnabled { get; set; } = true;

    /// <summary>
    /// Last sync status
    /// </summary>
    [MaxLength(50)]
    public string? LastSyncStatus { get; set; }

    /// <summary>
    /// Last sync error message
    /// </summary>
    [MaxLength(1000)]
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Number of sync failures
    /// </summary>
    public int SyncFailureCount { get; set; } = 0;

    /// <summary>
    /// Maximum allowed sync failures before disabling
    /// </summary>
    public int MaxSyncFailures { get; set; } = 5;

    /// <summary>
    /// Calendar color code
    /// </summary>
    [MaxLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// Time zone for this calendar
    /// </summary>
    [MaxLength(100)]
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Additional configuration settings (JSON)
    /// </summary>
    [MaxLength(4000)]
    public string? ConfigurationSettings { get; set; }

    /// <summary>
    /// When the integration was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the integration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the integration was last verified
    /// </summary>
    public DateTimeOffset? LastVerifiedAt { get; set; }
}