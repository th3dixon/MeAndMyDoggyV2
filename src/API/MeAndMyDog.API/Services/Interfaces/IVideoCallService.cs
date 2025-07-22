using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for video call management
/// </summary>
public interface IVideoCallService
{
    /// <summary>
    /// Start a new video call in a conversation
    /// </summary>
    /// <param name="userId">ID of the user starting the call</param>
    /// <param name="request">Call start request details</param>
    /// <returns>Video call session data</returns>
    Task<VideoCallDto> StartCallAsync(string userId, StartVideoCallRequest request);
    
    /// <summary>
    /// Join an existing video call
    /// </summary>
    /// <param name="userId">ID of the user joining the call</param>
    /// <param name="request">Join request details</param>
    /// <returns>Video call session data with participant info</returns>
    Task<VideoCallDto?> JoinCallAsync(string userId, JoinVideoCallRequest request);
    
    /// <summary>
    /// Leave a video call
    /// </summary>
    /// <param name="userId">ID of the user leaving the call</param>
    /// <param name="callId">ID of the video call session</param>
    /// <returns>True if successfully left the call</returns>
    Task<bool> LeaveCallAsync(string userId, string callId);
    
    /// <summary>
    /// End a video call (only by initiator)
    /// </summary>
    /// <param name="userId">ID of the user ending the call</param>
    /// <param name="callId">ID of the video call session</param>
    /// <returns>True if successfully ended the call</returns>
    Task<bool> EndCallAsync(string userId, string callId);
    
    /// <summary>
    /// Accept a video call invitation
    /// </summary>
    /// <param name="userId">ID of the user accepting the call</param>
    /// <param name="callId">ID of the video call session</param>
    /// <returns>True if successfully accepted the call</returns>
    Task<bool> AcceptCallAsync(string userId, string callId);
    
    /// <summary>
    /// Reject a video call invitation
    /// </summary>
    /// <param name="userId">ID of the user rejecting the call</param>
    /// <param name="callId">ID of the video call session</param>
    /// <param name="reason">Reason for rejecting the call</param>
    /// <returns>True if successfully rejected the call</returns>
    Task<bool> RejectCallAsync(string userId, string callId, string? reason = null);
    
    /// <summary>
    /// Update participant settings during a call
    /// </summary>
    /// <param name="userId">ID of the user updating settings</param>
    /// <param name="request">Update request details</param>
    /// <returns>Updated participant data</returns>
    Task<VideoCallParticipantDto?> UpdateParticipantAsync(string userId, UpdateCallParticipantRequest request);
    
    /// <summary>
    /// Handle WebRTC signaling messages
    /// </summary>
    /// <param name="userId">ID of the user sending the signal</param>
    /// <param name="request">Signaling request details</param>
    /// <returns>True if signal was processed successfully</returns>
    Task<bool> HandleSignalingAsync(string userId, WebRTCSignalingRequest request);
    
    /// <summary>
    /// Get details of a video call session
    /// </summary>
    /// <param name="userId">ID of the user requesting call details</param>
    /// <param name="callId">ID of the video call session</param>
    /// <returns>Video call session data or null if not found/unauthorized</returns>
    Task<VideoCallDto?> GetCallAsync(string userId, string callId);
    
    /// <summary>
    /// Get active video calls for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <returns>List of active video calls</returns>
    Task<List<VideoCallDto>> GetActiveCallsAsync(string userId);
    
    /// <summary>
    /// Get call history for a conversation
    /// </summary>
    /// <param name="userId">ID of the user requesting history</param>
    /// <param name="conversationId">ID of the conversation</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>List of historical video calls</returns>
    Task<List<VideoCallDto>> GetCallHistoryAsync(string userId, string conversationId, int skip = 0, int take = 20);
    
    /// <summary>
    /// Rate the quality of a completed call
    /// </summary>
    /// <param name="userId">ID of the user rating the call</param>
    /// <param name="callId">ID of the video call session</param>
    /// <param name="rating">Quality rating (1-5 stars)</param>
    /// <returns>True if rating was saved successfully</returns>
    Task<bool> RateCallAsync(string userId, string callId, int rating);
    
    /// <summary>
    /// Get call statistics for analytics
    /// </summary>
    /// <param name="userId">ID of the user requesting statistics</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Call statistics data</returns>
    Task<object> GetCallStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate);
}