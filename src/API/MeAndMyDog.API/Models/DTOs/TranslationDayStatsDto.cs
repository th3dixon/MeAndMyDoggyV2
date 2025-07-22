namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Daily translation statistics
/// </summary>
public class TranslationDayStatsDto
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Number of translations on this date
    /// </summary>
    public int TranslationCount { get; set; }

    /// <summary>
    /// Characters translated on this date
    /// </summary>
    public int CharactersTranslated { get; set; }

    /// <summary>
    /// Translation cost for this date
    /// </summary>
    public decimal TranslationCost { get; set; }

    /// <summary>
    /// Cache hits on this date
    /// </summary>
    public int CacheHits { get; set; }
}