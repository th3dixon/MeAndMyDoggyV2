namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Rate limiting options
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Whether rate limiting is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Global rate limit multiplier
    /// </summary>
    public double GlobalMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Whether to use stricter limits for anonymous users
    /// </summary>
    public bool StrictAnonymousLimits { get; set; } = true;

    /// <summary>
    /// IP addresses to whitelist (no rate limiting)
    /// </summary>
    public List<string> WhitelistedIps { get; set; } = new();

    /// <summary>
    /// User IDs to whitelist (no rate limiting)
    /// </summary>
    public List<string> WhitelistedUsers { get; set; } = new();
}