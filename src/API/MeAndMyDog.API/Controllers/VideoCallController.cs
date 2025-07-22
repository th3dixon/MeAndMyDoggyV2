using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for video call operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class VideoCallController : ControllerBase
{
    private readonly IVideoCallService _videoCallService;
    private readonly ILogger<VideoCallController> _logger;

    /// <summary>
    /// Initialize the video call controller
    /// </summary>
    public VideoCallController(IVideoCallService videoCallService, ILogger<VideoCallController> logger)
    {
        _videoCallService = videoCallService;
        _logger = logger;
    }

    /// <summary>
    /// Start a new video call
    /// </summary>
    /// <param name="request">Call start request</param>
    /// <returns>Video call session data</returns>
    [HttpPost("start")]
    public async Task<ActionResult<VideoCallResponse>> StartCall([FromBody] StartVideoCallRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var call = await _videoCallService.StartCallAsync(userId, request);
            
            _logger.LogInformation("Video call {CallId} started successfully by user {UserId}", call.Id, userId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Video call started successfully", 
                Call = call 
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new VideoCallResponse { Success = false, Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new VideoCallResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting video call for user {UserId}", GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Join an existing video call
    /// </summary>
    /// <param name="request">Join call request</param>
    /// <returns>Video call session data</returns>
    [HttpPost("join")]
    public async Task<ActionResult<VideoCallResponse>> JoinCall([FromBody] JoinVideoCallRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var call = await _videoCallService.JoinCallAsync(userId, request);
            
            if (call == null)
            {
                return NotFound(new VideoCallResponse { Success = false, Message = "Video call not found" });
            }

            _logger.LogInformation("User {UserId} joined video call {CallId} successfully", userId, request.CallId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Joined video call successfully", 
                Call = call 
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new VideoCallResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining video call {CallId} for user {UserId}", request.CallId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Leave a video call
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <returns>Operation result</returns>
    [HttpPost("{callId}/leave")]
    public async Task<ActionResult<VideoCallResponse>> LeaveCall(string callId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.LeaveCallAsync(userId, callId);
            
            if (!success)
            {
                return NotFound(new VideoCallResponse { Success = false, Message = "Video call not found or user not in call" });
            }

            _logger.LogInformation("User {UserId} left video call {CallId} successfully", userId, callId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Left video call successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// End a video call (initiator only)
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <returns>Operation result</returns>
    [HttpPost("{callId}/end")]
    public async Task<ActionResult<VideoCallResponse>> EndCall(string callId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.EndCallAsync(userId, callId);
            
            if (!success)
            {
                return NotFound(new VideoCallResponse { Success = false, Message = "Video call not found" });
            }

            _logger.LogInformation("Video call {CallId} ended successfully by user {UserId}", callId, userId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Video call ended successfully" 
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Accept a video call invitation
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <returns>Operation result</returns>
    [HttpPost("{callId}/accept")]
    public async Task<ActionResult<VideoCallResponse>> AcceptCall(string callId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.AcceptCallAsync(userId, callId);
            
            if (!success)
            {
                return NotFound(new VideoCallResponse { Success = false, Message = "Video call invitation not found" });
            }

            _logger.LogInformation("User {UserId} accepted video call {CallId} successfully", userId, callId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Video call accepted successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Reject a video call invitation
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="reason">Optional reason for rejection</param>
    /// <returns>Operation result</returns>
    [HttpPost("{callId}/reject")]
    public async Task<ActionResult<VideoCallResponse>> RejectCall(string callId, [FromQuery] string? reason = null)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.RejectCallAsync(userId, callId, reason);
            
            if (!success)
            {
                return NotFound(new VideoCallResponse { Success = false, Message = "Video call invitation not found" });
            }

            _logger.LogInformation("User {UserId} rejected video call {CallId} successfully", userId, callId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Video call rejected successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update participant settings during a call
    /// </summary>
    /// <param name="request">Update participant request</param>
    /// <returns>Updated participant data</returns>
    [HttpPut("participant")]
    public async Task<ActionResult<VideoCallParticipantDto>> UpdateParticipant([FromBody] UpdateCallParticipantRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var participant = await _videoCallService.UpdateParticipantAsync(userId, request);
            
            if (participant == null)
            {
                return NotFound("Participant not found in the specified call");
            }

            _logger.LogDebug("Participant settings updated successfully for user {UserId} in call {CallId}", userId, request.CallId);
            
            return Ok(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant settings for user {UserId} in call {CallId}", GetUserId(), request.CallId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Handle WebRTC signaling messages
    /// </summary>
    /// <param name="request">WebRTC signaling request</param>
    /// <returns>Operation result</returns>
    [HttpPost("signaling")]
    public async Task<ActionResult<VideoCallResponse>> HandleSignaling([FromBody] WebRTCSignalingRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.HandleSignalingAsync(userId, request);
            
            if (!success)
            {
                return BadRequest(new VideoCallResponse { Success = false, Message = "Failed to process signaling message" });
            }

            _logger.LogDebug("WebRTC signaling message processed successfully for user {UserId} in call {CallId}", userId, request.CallId);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Signaling message processed successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WebRTC signaling for user {UserId} in call {CallId}", GetUserId(), request.CallId);
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get details of a specific video call
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <returns>Video call data</returns>
    [HttpGet("{callId}")]
    public async Task<ActionResult<VideoCallDto>> GetCall(string callId)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var call = await _videoCallService.GetCallAsync(userId, callId);
            
            if (call == null)
            {
                return NotFound("Video call not found or user not authorized");
            }

            return Ok(call);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active video calls for the current user
    /// </summary>
    /// <returns>List of active video calls</returns>
    [HttpGet("active")]
    public async Task<ActionResult<List<VideoCallDto>>> GetActiveCalls()
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var calls = await _videoCallService.GetActiveCallsAsync(userId);
            
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active calls for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get call history for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>List of historical video calls</returns>
    [HttpGet("history/{conversationId}")]
    public async Task<ActionResult<List<VideoCallDto>>> GetCallHistory(string conversationId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var calls = await _videoCallService.GetCallHistoryAsync(userId, conversationId, skip, take);
            
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting call history for conversation {ConversationId} and user {UserId}", conversationId, GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Rate the quality of a completed call
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="rating">Quality rating (1-5)</param>
    /// <returns>Operation result</returns>
    [HttpPost("{callId}/rate")]
    public async Task<ActionResult<VideoCallResponse>> RateCall(string callId, [FromQuery] int rating)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new VideoCallResponse { Success = false, Message = "User not authenticated" });
            }

            var success = await _videoCallService.RateCallAsync(userId, callId, rating);
            
            if (!success)
            {
                return BadRequest(new VideoCallResponse { Success = false, Message = "Unable to rate call - call not found or not completed" });
            }

            _logger.LogInformation("User {UserId} rated video call {CallId} with {Rating} stars", userId, callId, rating);
            
            return Ok(new VideoCallResponse 
            { 
                Success = true, 
                Message = "Call rated successfully" 
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new VideoCallResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating video call {CallId} for user {UserId}", callId, GetUserId());
            return StatusCode(500, new VideoCallResponse { Success = false, Message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get call statistics for analytics
    /// </summary>
    /// <param name="fromDate">Start date for statistics (ISO 8601 format)</param>
    /// <param name="toDate">End date for statistics (ISO 8601 format)</param>
    /// <returns>Call statistics data</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetCallStatistics([FromQuery] string fromDate, [FromQuery] string toDate)
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!DateTimeOffset.TryParse(fromDate, out var from) || !DateTimeOffset.TryParse(toDate, out var to))
            {
                return BadRequest("Invalid date format. Use ISO 8601 format (e.g., 2023-01-01T00:00:00Z)");
            }

            var statistics = await _videoCallService.GetCallStatisticsAsync(userId, from, to);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting call statistics for user {UserId}", GetUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get the current user's ID from the JWT token
    /// </summary>
    private string? GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}