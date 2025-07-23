namespace MeAndMyDog.API.Middleware;

/// <summary>
/// Extension methods for adding error handling middleware
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    /// <summary>
    /// Add global error handling middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}