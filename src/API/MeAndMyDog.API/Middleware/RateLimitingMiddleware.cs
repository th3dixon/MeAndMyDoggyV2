using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Rate limiting middleware for API endpoints
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    // Rate limits for different endpoint categories
    private readonly Dictionary<string, RateLimitRule> _rateLimitRules = new()
    {
        // Messaging endpoints
        ["POST:/api/v1/messaging/send"] = new RateLimitRule { RequestsPerMinute = 30, RequestsPerHour = 500 },
        ["GET:/api/v1/messaging/conversations/{id}/messages"] = new RateLimitRule { RequestsPerMinute = 60, RequestsPerHour = 1000 },
        ["POST:/api/v1/messaging/messages/{id}/read"] = new RateLimitRule { RequestsPerMinute = 100, RequestsPerHour = 2000 },
        ["PUT:/api/v1/messaging/messages/{id}/edit"] = new RateLimitRule { RequestsPerMinute = 20, RequestsPerHour = 200 },
        ["DELETE:/api/v1/messaging/messages/{id}"] = new RateLimitRule { RequestsPerMinute = 10, RequestsPerHour = 100 },

        // Conversation endpoints
        ["POST:/api/v1/conversations"] = new RateLimitRule { RequestsPerMinute = 5, RequestsPerHour = 50 },
        ["GET:/api/v1/conversations"] = new RateLimitRule { RequestsPerMinute = 30, RequestsPerHour = 500 },
        ["PUT:/api/v1/conversations/{id}"] = new RateLimitRule { RequestsPerMinute = 10, RequestsPerHour = 100 },
        ["POST:/api/v1/conversations/{id}/participants"] = new RateLimitRule { RequestsPerMinute = 10, RequestsPerHour = 100 },

        // File upload endpoints
        ["POST:/api/v1/files/upload"] = new RateLimitRule { RequestsPerMinute = 20, RequestsPerHour = 200 },
        ["POST:/api/v1/files/upload/multiple"] = new RateLimitRule { RequestsPerMinute = 10, RequestsPerHour = 100 },

        // Voice message endpoints
        ["POST:/api/v1/voice/start-recording"] = new RateLimitRule { RequestsPerMinute = 15, RequestsPerHour = 150 },
        ["POST:/api/v1/voice/stop-recording"] = new RateLimitRule { RequestsPerMinute = 15, RequestsPerHour = 150 },

        // Search endpoints
        ["POST:/api/v1/messaging/search"] = new RateLimitRule { RequestsPerMinute = 30, RequestsPerHour = 300 },

        // Push notification endpoints
        ["POST:/api/v1/notifications/register"] = new RateLimitRule { RequestsPerMinute = 5, RequestsPerHour = 20 },

        // Default for other endpoints
        ["DEFAULT"] = new RateLimitRule { RequestsPerMinute = 60, RequestsPerHour = 1000 }
    };

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<RateLimitingMiddleware> logger,
        RateLimitOptions options)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for certain conditions
        if (ShouldSkipRateLimit(context))
        {
            await _next(context);
            return;
        }

        var userId = GetUserId(context);
        var ipAddress = GetClientIpAddress(context);
        var endpoint = GetEndpointKey(context);

        // Get rate limit rule for this endpoint
        var rule = GetRateLimitRule(endpoint);

        // Check rate limits
        var rateLimitResult = await CheckRateLimitsAsync(userId, ipAddress, endpoint, rule);

        if (rateLimitResult.IsLimited)
        {
            await HandleRateLimitExceededAsync(context, rateLimitResult);
            return;
        }

        // Add rate limit headers
        AddRateLimitHeaders(context, rateLimitResult);

        await _next(context);
    }

    private bool ShouldSkipRateLimit(HttpContext context)
    {
        // Skip rate limiting for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
            return true;

        // Skip for swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
            return true;

        // Skip for static files
        if (context.Request.Path.StartsWithSegments("/css") ||
            context.Request.Path.StartsWithSegments("/js") ||
            context.Request.Path.StartsWithSegments("/images") ||
            context.Request.Path.StartsWithSegments("/uploads"))
            return true;

        // Skip if disabled in options
        if (!_options.Enabled)
            return true;

        return false;
    }

    private string? GetUserId(HttpContext context)
    {
        return context.User?.FindFirst("sub")?.Value ?? 
               context.User?.FindFirst("id")?.Value ??
               context.User?.Identity?.Name;
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Try to get real IP from headers (for load balancers/proxies)
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

    private string GetEndpointKey(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";

        // Normalize path by removing dynamic segments (IDs)
        var normalizedPath = NormalizePath(path);
        
        return $"{method}:{normalizedPath}";
    }

    private string NormalizePath(string path)
    {
        // Replace common ID patterns with placeholders
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < segments.Length; i++)
        {
            // Replace GUIDs and common ID patterns
            if (IsId(segments[i]))
            {
                segments[i] = "{id}";
            }
        }

        return "/" + string.Join("/", segments);
    }

    private bool IsId(string segment)
    {
        // Check if segment looks like an ID (GUID, number, etc.)
        if (Guid.TryParse(segment, out _))
            return true;

        if (int.TryParse(segment, out _))
            return true;

        if (segment.Length > 10 && segment.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            return true;

        return false;
    }

    private RateLimitRule GetRateLimitRule(string endpoint)
    {
        if (_rateLimitRules.TryGetValue(endpoint, out var rule))
            return rule;

        return _rateLimitRules["DEFAULT"];
    }

    private async Task<RateLimitResult> CheckRateLimitsAsync(string? userId, string ipAddress, string endpoint, RateLimitRule rule)
    {
        var now = DateTimeOffset.UtcNow;
        var result = new RateLimitResult();

        // Create cache keys
        var userMinuteKey = $"rate_limit:user:{userId}:minute:{now:yyyy-MM-dd-HH-mm}:{endpoint}";
        var userHourKey = $"rate_limit:user:{userId}:hour:{now:yyyy-MM-dd-HH}:{endpoint}";
        var ipMinuteKey = $"rate_limit:ip:{ipAddress}:minute:{now:yyyy-MM-dd-HH-mm}:{endpoint}";
        var ipHourKey = $"rate_limit:ip:{ipAddress}:hour:{now:yyyy-MM-dd-HH}:{endpoint}";

        // Check user rate limits (if authenticated)
        if (!string.IsNullOrEmpty(userId))
        {
            var userMinuteCount = GetOrIncrementCounter(userMinuteKey, TimeSpan.FromMinutes(1));
            var userHourCount = GetOrIncrementCounter(userHourKey, TimeSpan.FromHours(1));

            if (userMinuteCount > rule.RequestsPerMinute)
            {
                result.IsLimited = true;
                result.LimitType = "user_minute";
                result.RetryAfterSeconds = 60 - now.Second;
                return result;
            }

            if (userHourCount > rule.RequestsPerHour)
            {
                result.IsLimited = true;
                result.LimitType = "user_hour";
                result.RetryAfterSeconds = (60 - now.Minute) * 60 - now.Second;
                return result;
            }

            result.UserRequestsThisMinute = userMinuteCount;
            result.UserRequestsThisHour = userHourCount;
        }

        // Check IP rate limits (always check for anonymous users)
        var ipMinuteCount = GetOrIncrementCounter(ipMinuteKey, TimeSpan.FromMinutes(1));
        var ipHourCount = GetOrIncrementCounter(ipHourKey, TimeSpan.FromHours(1));

        // More restrictive limits for anonymous users
        var ipMinuteLimit = string.IsNullOrEmpty(userId) ? rule.RequestsPerMinute / 2 : rule.RequestsPerMinute * 2;
        var ipHourLimit = string.IsNullOrEmpty(userId) ? rule.RequestsPerHour / 2 : rule.RequestsPerHour * 2;

        if (ipMinuteCount > ipMinuteLimit)
        {
            result.IsLimited = true;
            result.LimitType = "ip_minute";
            result.RetryAfterSeconds = 60 - now.Second;
            return result;
        }

        if (ipHourCount > ipHourLimit)
        {
            result.IsLimited = true;
            result.LimitType = "ip_hour";
            result.RetryAfterSeconds = (60 - now.Minute) * 60 - now.Second;
            return result;
        }

        result.IpRequestsThisMinute = ipMinuteCount;
        result.IpRequestsThisHour = ipHourCount;
        result.RemainingRequests = Math.Min(
            rule.RequestsPerMinute - (result.UserRequestsThisMinute ?? result.IpRequestsThisMinute),
            rule.RequestsPerHour - (result.UserRequestsThisHour ?? result.IpRequestsThisHour)
        );

        return result;
    }

    private int GetOrIncrementCounter(string key, TimeSpan expiry)
    {
        if (_cache.TryGetValue(key, out int count))
        {
            count++;
            _cache.Set(key, count, expiry);
            return count;
        }
        else
        {
            _cache.Set(key, 1, expiry);
            return 1;
        }
    }

    private async Task HandleRateLimitExceededAsync(HttpContext context, RateLimitResult result)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.Headers["Retry-After"] = result.RetryAfterSeconds.ToString();
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Rate limit exceeded",
            message = $"Too many requests. Limit exceeded: {result.LimitType}",
            retryAfterSeconds = result.RetryAfterSeconds,
            limitType = result.LimitType
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);

        _logger.LogWarning("Rate limit exceeded for {LimitType}. IP: {IpAddress}, User: {UserId}, Endpoint: {Endpoint}",
            result.LimitType, GetClientIpAddress(context), GetUserId(context), GetEndpointKey(context));
    }

    private void AddRateLimitHeaders(HttpContext context, RateLimitResult result)
    {
        context.Response.Headers["X-RateLimit-Remaining"] = result.RemainingRequests.ToString();
        
        if (result.UserRequestsThisMinute.HasValue)
        {
            context.Response.Headers["X-RateLimit-Limit-User"] = "30/minute, 500/hour";
            context.Response.Headers["X-RateLimit-Used-User"] = $"{result.UserRequestsThisMinute}/minute, {result.UserRequestsThisHour}/hour";
        }

        context.Response.Headers["X-RateLimit-Used-IP"] = $"{result.IpRequestsThisMinute}/minute, {result.IpRequestsThisHour}/hour";
    }
}