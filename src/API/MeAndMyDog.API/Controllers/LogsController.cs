using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    public IActionResult ReceiveLogs([FromBody] FrontendLogRequest request)
    {
        if (request?.Logs == null || !request.Logs.Any())
        {
            return BadRequest(new { message = "No log entries provided" });
        }

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
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process frontend log entry");
            }
        }

        return Ok(new { message = "Logs processed successfully", count = request.Logs.Count });
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

public class FrontendLogRequest
{
    [Required]
    public List<FrontendLogEntry> Logs { get; set; } = new();
}

public class FrontendLogEntry
{
    [Required]
    public string Timestamp { get; set; } = string.Empty;

    [Range(0, 4)]
    public int Level { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public Dictionary<string, object?>? Context { get; set; }
    public FrontendErrorInfo? Error { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
}

public class FrontendErrorInfo
{
    public string? Message { get; set; }
    public string? Stack { get; set; }
    public string? Name { get; set; }
}