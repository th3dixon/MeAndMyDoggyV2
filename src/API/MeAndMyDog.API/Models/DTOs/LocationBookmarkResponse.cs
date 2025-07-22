namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for bookmark operations
/// </summary>
public class LocationBookmarkResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The bookmark details
    /// </summary>
    public LocationBookmarkDto? Bookmark { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? Error { get; set; }
}