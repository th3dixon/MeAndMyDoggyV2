namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for location update
/// </summary>
public class UpdateLocationResponse
{
    /// <summary>
    /// Whether the update was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The location update details
    /// </summary>
    public LocationUpdateDto? LocationUpdate { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Whether live sharing is still active
    /// </summary>
    public bool IsLiveActive { get; set; }

    /// <summary>
    /// When live sharing expires
    /// </summary>
    public DateTimeOffset? LiveExpiresAt { get; set; }
}