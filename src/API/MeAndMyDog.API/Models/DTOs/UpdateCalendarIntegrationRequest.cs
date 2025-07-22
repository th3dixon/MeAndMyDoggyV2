using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update calendar integration
/// </summary>
public class UpdateCalendarIntegrationRequest
{
    /// <summary>
    /// Calendar display name
    /// </summary>
    public string? CalendarName { get; set; }

    /// <summary>
    /// Whether this integration is active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether to sync appointments to external calendar
    /// </summary>
    public bool? SyncToExternal { get; set; }

    /// <summary>
    /// Whether to sync appointments from external calendar
    /// </summary>
    public bool? SyncFromExternal { get; set; }

    /// <summary>
    /// Sync direction
    /// </summary>
    public SyncDirection? SyncDirection { get; set; }

    /// <summary>
    /// Sync frequency in minutes
    /// </summary>
    public int? SyncFrequencyMinutes { get; set; }

    /// <summary>
    /// Whether automatic sync is enabled
    /// </summary>
    public bool? AutoSyncEnabled { get; set; }

    /// <summary>
    /// Calendar color code
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// Time zone for this calendar
    /// </summary>
    public string? TimeZone { get; set; }
}