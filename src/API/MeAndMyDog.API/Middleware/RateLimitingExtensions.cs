namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Extension methods for rate limiting
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// Add rate limiting middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, RateLimitOptions? options = null)
    {
        options ??= new RateLimitOptions();
        return builder.UseMiddleware<RateLimitingMiddleware>(options);
    }

    /// <summary>
    /// Add rate limiting services
    /// </summary>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddMemoryCache();

        return services;
    }
}