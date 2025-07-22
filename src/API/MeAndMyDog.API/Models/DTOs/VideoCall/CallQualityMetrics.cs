namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Call quality metrics
/// </summary>
public class CallQualityMetrics
{
    /// <summary>
    /// Average connection quality rating (1-5)
    /// </summary>
    public double AverageQuality { get; set; }

    /// <summary>
    /// Number of connection issues
    /// </summary>
    public int ConnectionIssues { get; set; }

    /// <summary>
    /// Average latency in milliseconds
    /// </summary>
    public double AverageLatencyMs { get; set; }

    /// <summary>
    /// Packet loss percentage
    /// </summary>
    public double PacketLossPercentage { get; set; }
}