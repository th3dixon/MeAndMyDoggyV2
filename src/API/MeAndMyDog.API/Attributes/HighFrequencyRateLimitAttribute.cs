namespace MeAndMyDog.API.Attributes;

/// <summary>
/// Rate limit attribute for high-frequency endpoints
/// </summary>
public class HighFrequencyRateLimitAttribute : RateLimitAttribute
{
    public HighFrequencyRateLimitAttribute()
    {
        RequestsPerMinute = 100;
        RequestsPerHour = 2000;
        ErrorMessage = "Too many requests. This is a high-frequency endpoint with strict limits.";
    }
}