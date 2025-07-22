using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.DTOs.ScheduledMessage;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing scheduled messages
/// </summary>
[ApiController]
[Route("api/v1/scheduled-messages")]
[Authorize]
public class ScheduledMessageController : ControllerBase
{
    private readonly IScheduledMessageService _scheduledMessageService;
    private readonly ILogger<ScheduledMessageController> _logger;

    /// <summary>
    /// Initializes a new instance of ScheduledMessageController
    /// </summary>
    public ScheduledMessageController(
        IScheduledMessageService scheduledMessageService,
        ILogger<ScheduledMessageController> logger)
    {
        _scheduledMessageService = scheduledMessageService;
        _logger = logger;
    }

    /// <summary>
    /// Schedule a message for future delivery
    /// </summary>
    /// <param name="request">Message scheduling details</param>
    /// <returns>Scheduled message response</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ScheduledMessageResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ScheduleMessage([FromBody] ScheduleMessageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _scheduledMessageService.ScheduleMessageAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Message scheduled successfully for user {UserId}", userId);
                return CreatedAtAction(nameof(GetScheduledMessage), new { scheduledMessageId = response.ScheduledMessageId }, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling message");
            return StatusCode(500, "An error occurred while scheduling the message");
        }
    }

    /// <summary>
    /// Update a scheduled message
    /// </summary>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <param name="request">Update details</param>
    /// <returns>Updated scheduled message response</returns>
    [HttpPut("{scheduledMessageId}")]
    [ProducesResponseType(typeof(ScheduledMessageResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateScheduledMessage(string scheduledMessageId, [FromBody] UpdateScheduledMessageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _scheduledMessageService.UpdateScheduledMessageAsync(userId, scheduledMessageId, request);

            if (response.Success)
            {
                _logger.LogInformation("Scheduled message {ScheduledMessageId} updated successfully", scheduledMessageId);
                return Ok(response);
            }
            else
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating scheduled message {ScheduledMessageId}", scheduledMessageId);
            return StatusCode(500, "An error occurred while updating the scheduled message");
        }
    }

    /// <summary>
    /// Cancel a scheduled message
    /// </summary>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{scheduledMessageId}/cancel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelScheduledMessage(string scheduledMessageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _scheduledMessageService.CancelScheduledMessageAsync(userId, scheduledMessageId);
            if (success)
            {
                _logger.LogInformation("Scheduled message {ScheduledMessageId} cancelled successfully", scheduledMessageId);
                return Ok(new { Success = true, Message = "Scheduled message cancelled successfully" });
            }
            else
            {
                return NotFound("Scheduled message not found or cannot be cancelled");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling scheduled message {ScheduledMessageId}", scheduledMessageId);
            return StatusCode(500, "An error occurred while cancelling the scheduled message");
        }
    }

    /// <summary>
    /// Pause a recurring scheduled message
    /// </summary>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{scheduledMessageId}/pause")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PauseScheduledMessage(string scheduledMessageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _scheduledMessageService.PauseScheduledMessageAsync(userId, scheduledMessageId);
            if (success)
            {
                _logger.LogInformation("Scheduled message {ScheduledMessageId} paused successfully", scheduledMessageId);
                return Ok(new { Success = true, Message = "Scheduled message paused successfully" });
            }
            else
            {
                return NotFound("Scheduled message not found or cannot be paused");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing scheduled message {ScheduledMessageId}", scheduledMessageId);
            return StatusCode(500, "An error occurred while pausing the scheduled message");
        }
    }

    /// <summary>
    /// Resume a paused recurring scheduled message
    /// </summary>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{scheduledMessageId}/resume")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ResumeScheduledMessage(string scheduledMessageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _scheduledMessageService.ResumeScheduledMessageAsync(userId, scheduledMessageId);
            if (success)
            {
                _logger.LogInformation("Scheduled message {ScheduledMessageId} resumed successfully", scheduledMessageId);
                return Ok(new { Success = true, Message = "Scheduled message resumed successfully" });
            }
            else
            {
                return NotFound("Scheduled message not found or cannot be resumed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming scheduled message {ScheduledMessageId}", scheduledMessageId);
            return StatusCode(500, "An error occurred while resuming the scheduled message");
        }
    }

    /// <summary>
    /// Get a scheduled message by ID
    /// </summary>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>Scheduled message details</returns>
    [HttpGet("{scheduledMessageId}")]
    [ProducesResponseType(typeof(ScheduledMessageDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetScheduledMessage(string scheduledMessageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var scheduledMessage = await _scheduledMessageService.GetScheduledMessageAsync(userId, scheduledMessageId);
            if (scheduledMessage == null)
            {
                return NotFound("Scheduled message not found");
            }

            return Ok(scheduledMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled message {ScheduledMessageId}", scheduledMessageId);
            return StatusCode(500, "An error occurred while getting the scheduled message");
        }
    }

    /// <summary>
    /// Get scheduled messages for the current user
    /// </summary>
    /// <param name="conversationId">Optional conversation filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="includeRecurring">Whether to include recurring messages</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of scheduled messages</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ScheduledMessageDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserScheduledMessages(
        [FromQuery] string? conversationId = null,
        [FromQuery] string? status = null,
        [FromQuery] bool includeRecurring = true,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            ScheduledMessageStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ScheduledMessageStatus>(status, true, out var parsed))
            {
                statusEnum = parsed;
            }

            var scheduledMessages = await _scheduledMessageService.GetUserScheduledMessagesAsync(
                userId, conversationId, statusEnum, includeRecurring, skip, take);

            return Ok(scheduledMessages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled messages for user");
            return StatusCode(500, "An error occurred while getting scheduled messages");
        }
    }

    /// <summary>
    /// Get scheduled messages for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of scheduled messages</returns>
    [HttpGet("conversations/{conversationId}")]
    [ProducesResponseType(typeof(List<ScheduledMessageDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversationScheduledMessages(
        string conversationId,
        [FromQuery] string? status = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            ScheduledMessageStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ScheduledMessageStatus>(status, true, out var parsed))
            {
                statusEnum = parsed;
            }

            var scheduledMessages = await _scheduledMessageService.GetConversationScheduledMessagesAsync(
                userId, conversationId, statusEnum, skip, take);

            return Ok(scheduledMessages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled messages for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while getting scheduled messages");
        }
    }

    /// <summary>
    /// Validate recurrence pattern
    /// </summary>
    /// <param name="request">Recurrence pattern to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate-recurrence")]
    [ProducesResponseType(typeof(RecurrenceValidationResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ValidateRecurrencePattern([FromBody] ValidateRecurrenceRequest request)
    {
        try
        {
            if (!ModelState.IsValid || request.RecurrencePattern == null)
            {
                return BadRequest("Recurrence pattern is required");
            }

            var validationResult = await _scheduledMessageService.ValidateRecurrencePatternAsync(request.RecurrencePattern);
            return Ok(validationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating recurrence pattern");
            return StatusCode(500, "An error occurred while validating the recurrence pattern");
        }
    }

    /// <summary>
    /// Preview next occurrences for a recurring message
    /// </summary>
    /// <param name="request">Recurrence preview request</param>
    /// <returns>List of next occurrence dates</returns>
    [HttpPost("preview-recurrence")]
    [ProducesResponseType(typeof(PreviewRecurrenceResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> PreviewRecurrence([FromBody] PreviewRecurrenceRequest request)
    {
        try
        {
            if (!ModelState.IsValid || request.RecurrencePattern == null)
            {
                return BadRequest("Recurrence pattern and start date are required");
            }

            var count = Math.Min(Math.Max(1, request.Count), 20); // Limit to 20 occurrences
            var occurrences = await _scheduledMessageService.PreviewRecurrenceAsync(
                request.StartDate, request.RecurrencePattern, request.TimeZone, count);

            var response = new PreviewRecurrenceResponse
            {
                Success = true,
                Occurrences = occurrences,
                Count = occurrences.Count
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing recurrence");
            return StatusCode(500, "An error occurred while previewing the recurrence");
        }
    }

    /// <summary>
    /// Get scheduled message statistics for user
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Scheduled message statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ScheduledMessageStatistics), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetScheduledMessageStatistics(
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Default to last 30 days if dates not provided
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            if (fromDate > toDate)
            {
                return BadRequest("From date cannot be after to date");
            }

            var statistics = await _scheduledMessageService.GetScheduledMessageStatisticsAsync(userId, fromDate, toDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled message statistics");
            return StatusCode(500, "An error occurred while getting statistics");
        }
    }
}