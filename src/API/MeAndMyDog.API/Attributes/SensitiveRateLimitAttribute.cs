namespace MeAndMyDog.API.Attributes;

/// <summary>
/// Rate limit attribute for sensitive endpoints
/// </summary>
public class SensitiveRateLimitAttribute : RateLimitAttribute
{
    public SensitiveRateLimitAttribute()
    {
        RequestsPerMinute = 5;
        RequestsPerHour = 50;
        ErrorMessage = "Rate limit exceeded for sensitive operation. Please wait before trying again.";
        PerUser = true;
    }
}