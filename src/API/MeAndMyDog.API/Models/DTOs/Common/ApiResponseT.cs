namespace MeAndMyDog.API.Models.DTOs.Common
{
    /// <summary>
    /// Generic API response wrapper for consistent response format
    /// </summary>
    /// <typeparam name="T">Type of the data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The actual data being returned
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// List of error messages if any
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful response
        /// </summary>
        /// <param name="data">Data to return</param>
        /// <param name="message">Success message</param>
        /// <returns>Successful ApiResponse</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="errors">List of specific errors</param>
        /// <returns>Error ApiResponse</returns>
        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }
    }
}