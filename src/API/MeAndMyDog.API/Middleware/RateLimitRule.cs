namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Rate limit rule for an endpoint
/// </summary>
public class RateLimitRule
{
    /// <summary>
    /// Maximum requests per minute
    /// </summary>
    public int RequestsPerMinute { get; set; }

    /// <summary>
    /// Maximum requests per hour
    /// </summary>
    public int RequestsPerHour { get; set; }

    /// <summary>
    /// Custom message for this limit
    /// </summary>
    public string? CustomMessage { get; set; }
}