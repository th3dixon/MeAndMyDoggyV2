namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// User security statistics
/// </summary>
public class UserSecurityStatsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Number of incidents involving this user
    /// </summary>
    public int IncidentCount { get; set; }

    /// <summary>
    /// Average risk score
    /// </summary>
    public double AverageRiskScore { get; set; }

    /// <summary>
    /// Last incident date
    /// </summary>
    public DateTimeOffset? LastIncidentDate { get; set; }

    /// <summary>
    /// Most common incident type
    /// </summary>
    public string MostCommonIncidentType { get; set; } = string.Empty;
}