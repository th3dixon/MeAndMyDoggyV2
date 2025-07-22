namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// User security analysis result
/// </summary>
public class UserSecurityAnalysisDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Analysis time window
    /// </summary>
    public TimeSpan AnalysisWindow { get; set; }

    /// <summary>
    /// Overall risk score (0-100)
    /// </summary>
    public double OverallRiskScore { get; set; }

    /// <summary>
    /// Risk level classification
    /// </summary>
    public string RiskLevel { get; set; } = "Low";

    /// <summary>
    /// Number of security incidents
    /// </summary>
    public int SecurityIncidentCount { get; set; }

    /// <summary>
    /// Number of failed authentication attempts
    /// </summary>
    public int FailedAuthAttempts { get; set; }

    /// <summary>
    /// Number of suspicious access patterns
    /// </summary>
    public int SuspiciousAccessCount { get; set; }

    /// <summary>
    /// Unusual geographic locations accessed from
    /// </summary>
    public List<string> UnusualLocations { get; set; } = new();

    /// <summary>
    /// Unusual access times
    /// </summary>
    public List<DateTimeOffset> UnusualAccessTimes { get; set; } = new();

    /// <summary>
    /// Device fingerprints used
    /// </summary>
    public List<string> DeviceFingerprints { get; set; } = new();

    /// <summary>
    /// IP addresses used
    /// </summary>
    public List<string> IpAddresses { get; set; } = new();

    /// <summary>
    /// Security recommendations
    /// </summary>
    public List<string> SecurityRecommendations { get; set; } = new();

    /// <summary>
    /// Anomaly detection results
    /// </summary>
    public List<SecurityAnomalyDto> Anomalies { get; set; } = new();

    /// <summary>
    /// When the analysis was performed
    /// </summary>
    public DateTimeOffset AnalyzedAt { get; set; } = DateTimeOffset.UtcNow;
}