namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Daily location sharing statistics
/// </summary>
public class LocationSharingDayStats
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Number of locations shared on this date
    /// </summary>
    public int LocationsShared { get; set; }

    /// <summary>
    /// Number of live shares started
    /// </summary>
    public int LiveSharesStarted { get; set; }

    /// <summary>
    /// Total live sharing time in minutes
    /// </summary>
    public int TotalLiveSharingMinutes { get; set; }
}