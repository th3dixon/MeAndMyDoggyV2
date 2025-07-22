namespace MeAndMyDog.API.Attributes;

/// <summary>
/// Rate limit attribute for search endpoints
/// </summary>
public class SearchRateLimitAttribute : RateLimitAttribute
{
    public SearchRateLimitAttribute()
    {
        RequestsPerMinute = 20;
        RequestsPerHour = 200;
        ErrorMessage = "Search rate limit exceeded. Please wait before searching again.";
        PerUser = true;
    }
}