using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs.Logging;
using MeAndMyDog.API.Models.Common;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for handling frontend logging requests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;

    /// <summary>
    /// Initializes a new instance of the LogsController
    /// </summary>
    /// <param name="logger">Logger instance for this controller</param>
    public LogsController(ILogger<LogsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Receives log entries from frontend applications
    /// </summary>
    /// <param name="request">The log entries to process</param>
    /// <returns>Confirmation of log processing</returns>
    [HttpPost]
    public ActionResult<ApiResponse<object>> ReceiveLogs([FromBody] FrontendLogRequest request)
    {
        if (request?.Logs == null || !request.Logs.Any())
        {
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "No log entries provided"
            );
            errorResponse.CorrelationId = HttpContext.TraceIdentifier;
            return BadRequest(errorResponse);
        }

        var processedCount = 0;
        var failedCount = 0;

        foreach (var logEntry in request.Logs)
        {
            try
            {
                var logLevel = MapLogLevel(logEntry.Level);
                var message = $"[Frontend] {logEntry.Message}";
                
                using (_logger.BeginScope(new Dictionary<string, object?>
                {
                    ["UserId"] = logEntry.UserId,
                    ["SessionId"] = logEntry.SessionId,
                    ["UserAgent"] = logEntry.UserAgent,
                    ["Url"] = logEntry.Url,
                    ["Context"] = logEntry.Context,
                    ["Timestamp"] = logEntry.Timestamp
                }))
                {
                    switch (logLevel)
                    {
                        case LogLevel.Debug:
                            _logger.LogDebug(message);
                            break;
                        case LogLevel.Information:
                            _logger.LogInformation(message);
                            break;
                        case LogLevel.Warning:
                            _logger.LogWarning(message);
                            break;
                        case LogLevel.Error:
                        case LogLevel.Critical:
                            if (logEntry.Error != null)
                            {
                                _logger.LogError(message + " Error: {ErrorMessage}", logEntry.Error.Message);
                            }
                            else
                            {
                                _logger.LogError(message);
                            }
                            break;
                    }
                }
                processedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process frontend log entry");
                failedCount++;
            }
        }

        var responseData = new
        {
            ProcessedCount = processedCount,
            FailedCount = failedCount,
            TotalCount = request.Logs.Count
        };

        var successResponse = ApiResponse<object>.SuccessResponse(
            responseData,
            $"Processed {processedCount} log entries successfully"
        );
        successResponse.CorrelationId = HttpContext.TraceIdentifier;
        return Ok(successResponse);
    }

    private static LogLevel MapLogLevel(int frontendLevel)
    {
        return frontendLevel switch
        {
            0 => LogLevel.Debug,
            1 => LogLevel.Information,
            2 => LogLevel.Warning,
            3 => LogLevel.Error,
            4 => LogLevel.Critical,
            _ => LogLevel.Information
        };
    }
}

