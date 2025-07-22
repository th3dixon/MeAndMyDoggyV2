namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for location sharing statistics
/// </summary>
public class LocationSharingStatsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Total number of locations shared
    /// </summary>
    public int TotalLocationsShared { get; set; }

    /// <summary>
    /// Number of static location shares
    /// </summary>
    public int StaticShares { get; set; }

    /// <summary>
    /// Number of live location shares
    /// </summary>
    public int LiveShares { get; set; }

    /// <summary>
    /// Number of bookmark location shares
    /// </summary>
    public int BookmarkShares { get; set; }

    /// <summary>
    /// Total number of location bookmarks
    /// </summary>
    public int TotalBookmarks { get; set; }

    /// <summary>
    /// Number of active live location shares
    /// </summary>
    public int ActiveLiveShares { get; set; }

    /// <summary>
    /// Average duration of live location shares (in minutes)
    /// </summary>
    public double AverageLiveSharingDuration { get; set; }

    /// <summary>
    /// Most frequently shared location category
    /// </summary>
    public string? MostSharedCategory { get; set; }

    /// <summary>
    /// Total distance covered in live sharing (in meters)
    /// </summary>
    public double TotalDistanceCovered { get; set; }

    /// <summary>
    /// Number of different conversations where locations were shared
    /// </summary>
    public int ConversationsWithLocations { get; set; }

    /// <summary>
    /// Most active sharing hour (0-23)
    /// </summary>
    public int MostActiveSharingHour { get; set; }

    /// <summary>
    /// Statistics date range start
    /// </summary>
    public DateTimeOffset FromDate { get; set; }

    /// <summary>
    /// Statistics date range end
    /// </summary>
    public DateTimeOffset ToDate { get; set; }

    /// <summary>
    /// Recent location sharing activity by day
    /// </summary>
    public List<LocationSharingDayStats> DailyStats { get; set; } = new();

    /// <summary>
    /// Top shared locations
    /// </summary>
    public List<TopSharedLocationDto> TopLocations { get; set; } = new();
}