namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Security anomaly detection result
/// </summary>
public class SecurityAnomalyDto
{
    /// <summary>
    /// Type of anomaly detected
    /// </summary>
    public string AnomalyType { get; set; } = string.Empty;

    /// <summary>
    /// Anomaly description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score for the anomaly (0-1)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Severity level
    /// </summary>
    public string Severity { get; set; } = "Medium";

    /// <summary>
    /// When the anomaly was detected
    /// </summary>
    public DateTimeOffset DetectedAt { get; set; }

    /// <summary>
    /// Additional context data
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
}