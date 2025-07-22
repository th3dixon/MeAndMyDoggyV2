namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Security statistics summary
/// </summary>
public class SecurityStatsDto
{
    /// <summary>
    /// Statistics period start
    /// </summary>
    public DateTimeOffset PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end
    /// </summary>
    public DateTimeOffset PeriodEnd { get; set; }

    /// <summary>
    /// Total security incidents
    /// </summary>
    public int TotalIncidents { get; set; }

    /// <summary>
    /// Open incidents
    /// </summary>
    public int OpenIncidents { get; set; }

    /// <summary>
    /// Resolved incidents
    /// </summary>
    public int ResolvedIncidents { get; set; }

    /// <summary>
    /// Critical incidents
    /// </summary>
    public int CriticalIncidents { get; set; }

    /// <summary>
    /// Average resolution time
    /// </summary>
    public TimeSpan AverageResolutionTime { get; set; }

    /// <summary>
    /// Total self-destructed messages
    /// </summary>
    public int SelfDestructedMessages { get; set; }

    /// <summary>
    /// Total secure messages
    /// </summary>
    public int SecureMessages { get; set; }

    /// <summary>
    /// Total access attempts
    /// </summary>
    public long TotalAccessAttempts { get; set; }

    /// <summary>
    /// Blocked access attempts
    /// </summary>
    public long BlockedAccessAttempts { get; set; }

    /// <summary>
    /// Top incident types
    /// </summary>
    public List<IncidentTypeStatsDto> TopIncidentTypes { get; set; } = new();

    /// <summary>
    /// Top affected users
    /// </summary>
    public List<UserSecurityStatsDto> TopAffectedUsers { get; set; } = new();

    /// <summary>
    /// Security trends
    /// </summary>
    public List<SecurityTrendDto> Trends { get; set; } = new();
}