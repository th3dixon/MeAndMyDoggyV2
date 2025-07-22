namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Security trend information
/// </summary>
public class SecurityTrendDto
{
    /// <summary>
    /// Trend metric name
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// Current value
    /// </summary>
    public double CurrentValue { get; set; }

    /// <summary>
    /// Previous value for comparison
    /// </summary>
    public double PreviousValue { get; set; }

    /// <summary>
    /// Percentage change
    /// </summary>
    public double PercentageChange { get; set; }

    /// <summary>
    /// Trend direction (up, down, stable)
    /// </summary>
    public string TrendDirection { get; set; } = "stable";

    /// <summary>
    /// Whether this trend is concerning
    /// </summary>
    public bool IsConcerning { get; set; }
}