namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Search volume by date
/// </summary>
public class SearchVolumeByDateDto
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Number of searches on this date
    /// </summary>
    public int SearchCount { get; set; }

    /// <summary>
    /// Number of unique queries on this date
    /// </summary>
    public int UniqueQueries { get; set; }
}