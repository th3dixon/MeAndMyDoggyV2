namespace MeAndMyDog.API.Models.Common;

/// <summary>
/// Standard API response wrapper for consistent response formatting
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The response data
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Human-readable message describing the result
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// List of error messages if the operation failed
    /// </summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// Correlation ID for request tracing
    /// </summary>
    public string? CorrelationId { get; set; }
    
    /// <summary>
    /// Timestamp when the response was generated
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Creates an error response with error messages
    /// </summary>
    /// <param name="errors">List of error messages</param>
    /// <param name="message">Optional error message</param>
    /// <returns>Error API response</returns>
    public static ApiResponse<T> ErrorResponse(List<string> errors, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors,
            Message = message
        };
    }

    /// <summary>
    /// Creates an error response with a single error message
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="message">Optional error message</param>
    /// <returns>Error API response</returns>
    public static ApiResponse<T> ErrorResponse(string error, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = new List<string> { error },
            Message = message
        };
    }
}