namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Daily security metrics
/// </summary>
public class DailySecurityMetricsDto
{
    /// <summary>
    /// Date
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Number of incidents on this date
    /// </summary>
    public int IncidentCount { get; set; }

    /// <summary>
    /// Number of access attempts
    /// </summary>
    public long AccessAttempts { get; set; }

    /// <summary>
    /// Number of blocked attempts
    /// </summary>
    public long BlockedAttempts { get; set; }

    /// <summary>
    /// Number of security alerts
    /// </summary>
    public int SecurityAlerts { get; set; }

    /// <summary>
    /// Number of self-destructed messages
    /// </summary>
    public int SelfDestructedMessages { get; set; }

    /// <summary>
    /// Average risk score for the day
    /// </summary>
    public double AverageRiskScore { get; set; }
}