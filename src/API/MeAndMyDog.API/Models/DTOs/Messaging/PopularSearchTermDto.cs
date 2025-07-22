namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Popular search term data
/// </summary>
public class PopularSearchTermDto
{
    /// <summary>
    /// Search term
    /// </summary>
    public string Term { get; set; } = string.Empty;

    /// <summary>
    /// Number of times searched
    /// </summary>
    public int SearchCount { get; set; }

    /// <summary>
    /// Number of results typically found
    /// </summary>
    public int AverageResults { get; set; }

    /// <summary>
    /// Last time this term was searched
    /// </summary>
    public DateTimeOffset LastSearched { get; set; }

    /// <summary>
    /// Trend indicator (increasing/decreasing/stable)
    /// </summary>
    public string Trend { get; set; } = "stable";
}