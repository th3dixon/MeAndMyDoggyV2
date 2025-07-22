using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for calendar integration
/// </summary>
public class CalendarIntegrationDto
{
    /// <summary>
    /// Integration unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// External calendar provider
    /// </summary>
    public CalendarProvider Provider { get; set; }

    /// <summary>
    /// External calendar ID
    /// </summary>
    public string ExternalCalendarId { get; set; } = string.Empty;

    /// <summary>
    /// Calendar display name
    /// </summary>
    public string CalendarName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this integration is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether to sync appointments to external calendar
    /// </summary>
    public bool SyncToExternal { get; set; }

    /// <summary>
    /// Whether to sync appointments from external calendar
    /// </summary>
    public bool SyncFromExternal { get; set; }

    /// <summary>
    /// Sync direction
    /// </summary>
    public SyncDirection SyncDirection { get; set; }

    /// <summary>
    /// Whether the access token is valid
    /// </summary>
    public bool HasValidToken => TokenExpiresAt > DateTimeOffset.UtcNow;

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
    public int SyncFrequencyMinutes { get; set; }

    /// <summary>
    /// Whether automatic sync is enabled
    /// </summary>
    public bool AutoSyncEnabled { get; set; }

    /// <summary>
    /// Last sync status
    /// </summary>
    public string? LastSyncStatus { get; set; }

    /// <summary>
    /// Last sync error message
    /// </summary>
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Number of sync failures
    /// </summary>
    public int SyncFailureCount { get; set; }

    /// <summary>
    /// Maximum allowed sync failures before disabling
    /// </summary>
    public int MaxSyncFailures { get; set; }

    /// <summary>
    /// Calendar color code
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Time zone for this calendar
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// When the integration was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the integration was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// When the integration was last verified
    /// </summary>
    public DateTimeOffset? LastVerifiedAt { get; set; }

    /// <summary>
    /// Whether sync is currently healthy
    /// </summary>
    public bool IsSyncHealthy => SyncFailureCount < MaxSyncFailures && HasValidToken;
}