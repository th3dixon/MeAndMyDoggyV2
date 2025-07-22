using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Calendar sync status information
/// </summary>
public class CalendarSyncStatusDto
{
    /// <summary>
    /// Integration ID
    /// </summary>
    public string IntegrationId { get; set; } = string.Empty;

    /// <summary>
    /// Calendar provider
    /// </summary>
    public CalendarProvider Provider { get; set; }

    /// <summary>
    /// Calendar name
    /// </summary>
    public string CalendarName { get; set; } = string.Empty;

    /// <summary>
    /// Whether sync is currently running
    /// </summary>
    public bool IsSyncing { get; set; }

    /// <summary>
    /// Last sync timestamp
    /// </summary>
    public DateTimeOffset? LastSync { get; set; }

    /// <summary>
    /// Next scheduled sync
    /// </summary>
    public DateTimeOffset? NextSync { get; set; }

    /// <summary>
    /// Sync status message
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of appointments synced in last sync
    /// </summary>
    public int LastSyncAppointmentCount { get; set; }

    /// <summary>
    /// Total sync failure count
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Last error message
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Whether sync is healthy
    /// </summary>
    public bool IsHealthy { get; set; }
}