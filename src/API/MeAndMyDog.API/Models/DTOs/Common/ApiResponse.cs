namespace MeAndMyDog.API.Models.DTOs.Common
{
    /// <summary>
    /// Non-generic API response for cases where no data is returned
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        /// <param name="message">Success message</param>
        /// <returns>Successful ApiResponse</returns>
        public static new ApiResponse SuccessResponse(string message = "Success")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates an error response without data
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="errors">List of specific errors</param>
        /// <returns>Error ApiResponse</returns>
        public static new ApiResponse ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }
    }
}