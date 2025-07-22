namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Popular query data
/// </summary>
public class PopularQueryDto
{
    /// <summary>
    /// Search query
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Number of times searched
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Last time this query was used
    /// </summary>
    public DateTimeOffset LastUsed { get; set; }

    /// <summary>
    /// Average results returned
    /// </summary>
    public double AverageResults { get; set; }
}