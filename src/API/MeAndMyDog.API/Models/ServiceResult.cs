namespace MeAndMyDog.API.Models;

/// <summary>
/// Generic service result wrapper for API operations
/// </summary>
/// <typeparam name="T">Type of data returned by the service</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The data returned by the operation (if successful)
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// List of error messages (if operation failed)
    /// </summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    /// <param name="data">The data to return</param>
    /// <returns>A successful service result</returns>
    public static ServiceResult<T> SuccessResult(T data)
    {
        return new ServiceResult<T> { Success = true, Data = data };
    }
    
    /// <summary>
    /// Creates a failed result with error messages
    /// </summary>
    /// <param name="errors">Error messages</param>
    /// <returns>A failed service result</returns>
    public static ServiceResult<T> FailureResult(params string[] errors)
    {
        return new ServiceResult<T> { Success = false, Errors = errors.ToList() };
    }
}