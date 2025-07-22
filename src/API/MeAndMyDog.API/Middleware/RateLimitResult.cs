namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Rate limit check result
/// </summary>
public class RateLimitResult
{
    /// <summary>
    /// Whether request should be limited
    /// </summary>
    public bool IsLimited { get; set; }

    /// <summary>
    /// Type of limit that was exceeded
    /// </summary>
    public string LimitType { get; set; } = string.Empty;

    /// <summary>
    /// Seconds until user can retry
    /// </summary>
    public int RetryAfterSeconds { get; set; }

    /// <summary>
    /// Remaining requests in current window
    /// </summary>
    public int RemainingRequests { get; set; }

    /// <summary>
    /// User's requests this minute
    /// </summary>
    public int? UserRequestsThisMinute { get; set; }

    /// <summary>
    /// User's requests this hour
    /// </summary>
    public int? UserRequestsThisHour { get; set; }

    /// <summary>
    /// IP's requests this minute
    /// </summary>
    public int IpRequestsThisMinute { get; set; }

    /// <summary>
    /// IP's requests this hour
    /// </summary>
    public int IpRequestsThisHour { get; set; }
}