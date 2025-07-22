using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create calendar integration
/// </summary>
public class CreateCalendarIntegrationRequest
{
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
    public SyncDirection SyncDirection { get; set; } = SyncDirection.Bidirectional;

    /// <summary>
    /// Access token for external calendar API
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for external calendar API
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the access token expires
    /// </summary>
    public DateTimeOffset? TokenExpiresAt { get; set; }

    /// <summary>
    /// Sync frequency in minutes
    /// </summary>
    public int SyncFrequencyMinutes { get; set; } = 60;

    /// <summary>
    /// Whether automatic sync is enabled
    /// </summary>
    public bool AutoSyncEnabled { get; set; } = true;

    /// <summary>
    /// Calendar color code
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Time zone for this calendar
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
}