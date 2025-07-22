using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace MeAndMyDog.API.Attributes;

/// <summary>
/// Attribute for applying custom rate limiting to specific actions
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Maximum requests per minute
    /// </summary>
    public int RequestsPerMinute { get; set; } = 10;

    /// <summary>
    /// Maximum requests per hour
    /// </summary>
    public int RequestsPerHour { get; set; } = 100;

    /// <summary>
    /// Custom error message
    /// </summary>
    public string ErrorMessage { get; set; } = "Rate limit exceeded. Please try again later.";

    /// <summary>
    /// Whether to apply rate limiting per user (true) or per IP (false)
    /// </summary>
    public bool PerUser { get; set; } = true;

    /// <summary>
    /// Whether to skip rate limiting for authenticated users
    /// </summary>
    public bool SkipForAuthenticated { get; set; } = false;

    /// <summary>
    /// Custom cache key prefix
    /// </summary>
    public string CacheKeyPrefix { get; set; } = "rate_limit";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RateLimitAttribute>>();

        // Skip if disabled for authenticated users
        if (SkipForAuthenticated && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            await next();
            return;
        }

        var identifier = GetIdentifier(context.HttpContext);
        var now = DateTimeOffset.UtcNow;
        
        // Create cache keys for minute and hour windows
        var minuteKey = $"{CacheKeyPrefix}:{identifier}:minute:{now:yyyy-MM-dd-HH-mm}";
        var hourKey = $"{CacheKeyPrefix}:{identifier}:hour:{now:yyyy-MM-dd-HH}";

        // Check minute limit
        var minuteCount = GetOrIncrementCounter(cache, minuteKey, TimeSpan.FromMinutes(1));
        if (minuteCount > RequestsPerMinute)
        {
            await HandleRateLimitExceeded(context, "minute", 60 - now.Second, logger);
            return;
        }

        // Check hour limit
        var hourCount = GetOrIncrementCounter(cache, hourKey, TimeSpan.FromHours(1));
        if (hourCount > RequestsPerHour)
        {
            await HandleRateLimitExceeded(context, "hour", (60 - now.Minute) * 60 - now.Second, logger);
            return;
        }

        // Add rate limit headers
        context.HttpContext.Response.Headers["X-RateLimit-Limit"] = $"{RequestsPerMinute}/minute, {RequestsPerHour}/hour";
        context.HttpContext.Response.Headers["X-RateLimit-Remaining-Minute"] = (RequestsPerMinute - minuteCount).ToString();
        context.HttpContext.Response.Headers["X-RateLimit-Remaining-Hour"] = (RequestsPerHour - hourCount).ToString();

        await next();
    }

    private string GetIdentifier(HttpContext context)
    {
        if (PerUser && context.User.Identity?.IsAuthenticated == true)
        {
            return context.User.FindFirst("sub")?.Value ?? 
                   context.User.FindFirst("id")?.Value ?? 
                   context.User.Identity.Name ?? 
                   GetIpAddress(context);
        }

        return GetIpAddress(context);
    }

    private string GetIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private int GetOrIncrementCounter(IMemoryCache cache, string key, TimeSpan expiry)
    {
        if (cache.TryGetValue(key, out int count))
        {
            count++;
            cache.Set(key, count, expiry);
            return count;
        }
        else
        {
            cache.Set(key, 1, expiry);
            return 1;
        }
    }

    private Task HandleRateLimitExceeded(ActionExecutingContext context, string limitType, int retryAfterSeconds, ILogger logger)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = retryAfterSeconds.ToString();

        var result = new ObjectResult(new
        {
            error = "Rate limit exceeded",
            message = ErrorMessage,
            limitType,
            retryAfterSeconds
        })
        {
            StatusCode = (int)HttpStatusCode.TooManyRequests
        };

        context.Result = result;

        logger.LogWarning("Rate limit exceeded for {LimitType}. Identifier: {Identifier}, Action: {Action}",
            limitType, GetIdentifier(context.HttpContext), context.ActionDescriptor.DisplayName);
            
        return Task.CompletedTask;
    }
}