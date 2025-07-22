namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Incident type statistics
/// </summary>
public class IncidentTypeStatsDto
{
    /// <summary>
    /// Incident type
    /// </summary>
    public string IncidentType { get; set; } = string.Empty;

    /// <summary>
    /// Number of incidents
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of total incidents
    /// </summary>
    public double Percentage { get; set; }

    /// <summary>
    /// Average severity
    /// </summary>
    public double AverageSeverity { get; set; }
}