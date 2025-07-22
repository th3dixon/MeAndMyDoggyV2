using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for video call management
/// </summary>
public class VideoCallService : IVideoCallService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VideoCallService> _logger;

    /// <summary>
    /// Initialize the video call service
    /// </summary>
    public VideoCallService(ApplicationDbContext context, ILogger<VideoCallService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<VideoCallDto> StartCallAsync(string userId, StartVideoCallRequest request)
    {
        try
        {
            // Validate conversation exists and user is participant
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);

            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found");
            }

            if (!conversation.Participants.Any(p => p.UserId == userId))
            {
                throw new UnauthorizedAccessException("User is not a participant in this conversation");
            }

            // Check if there's already an active call in this conversation
            var existingCall = await _context.VideoCallSessions
                .FirstOrDefaultAsync(v => v.ConversationId == request.ConversationId && 
                    (v.Status == EnumConverter.ToString(CallStatus.Pending) || 
                     v.Status == EnumConverter.ToString(CallStatus.Ringing) || 
                     v.Status == EnumConverter.ToString(CallStatus.Active)));

            if (existingCall != null)
            {
                throw new InvalidOperationException("There is already an active call in this conversation");
            }

            // Create new video call session
            var callSession = new VideoCallSession
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = request.ConversationId,
                InitiatorId = userId,
                StartTime = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(CallStatus.Pending),
                RoomId = GenerateRoomId(),
                IsRecorded = request.RecordCall,
                MaxParticipants = Math.Max(2, Math.Min(request.MaxParticipants, 10)), // Limit to 10 participants
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.VideoCallSessions.Add(callSession);

            // Add participants to the call
            var participantIds = request.ParticipantIds.Union(new[] { userId }).Distinct().ToList();
            
            foreach (var participantId in participantIds.Take(callSession.MaxParticipants))
            {
                // Validate participant exists and is in conversation
                if (conversation.Participants.Any(p => p.UserId == participantId))
                {
                    var callParticipant = new VideoCallParticipant
                    {
                        Id = Guid.NewGuid().ToString(),
                        VideoCallSessionId = callSession.Id,
                        UserId = participantId,
                        JoinedAt = DateTimeOffset.UtcNow,
                        VideoEnabled = true,
                        AudioEnabled = true,
                        ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Connecting),
                        CallAccepted = participantId == userId, // Initiator automatically accepts
                        PeerConnectionId = Guid.NewGuid().ToString()
                    };

                    _context.VideoCallParticipants.Add(callParticipant);
                    callSession.Participants.Add(callParticipant);
                }
            }

            // Update call status based on participants
            if (callSession.Participants.Count > 1)
            {
                callSession.Status = EnumConverter.ToString(CallStatus.Ringing);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Video call {CallId} started by user {UserId} in conversation {ConversationId}", 
                callSession.Id, userId, request.ConversationId);

            return await MapToDto(callSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting video call for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<VideoCallDto?> JoinCallAsync(string userId, JoinVideoCallRequest request)
    {
        try
        {
            var callSession = await _context.VideoCallSessions
                .Include(v => v.Participants)
                    .ThenInclude(p => p.User)
                .Include(v => v.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(v => v.Id == request.CallId);

            if (callSession == null)
            {
                return null;
            }

            // Check if user is authorized to join this call
            if (!callSession.Conversation.Participants.Any(p => p.UserId == userId))
            {
                throw new UnauthorizedAccessException("User is not authorized to join this call");
            }

            // Check if call is in a joinable state
            var callStatus = EnumConverter.ToCallStatus(callSession.Status);
            if (callStatus != CallStatus.Pending && callStatus != CallStatus.Ringing && callStatus != CallStatus.Active)
            {
                throw new InvalidOperationException("Call is not in a joinable state");
            }

            // Find or create participant
            var participant = callSession.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                // Check if call has room for more participants
                if (callSession.Participants.Count >= callSession.MaxParticipants)
                {
                    throw new InvalidOperationException("Call has reached maximum participant limit");
                }

                participant = new VideoCallParticipant
                {
                    Id = Guid.NewGuid().ToString(),
                    VideoCallSessionId = callSession.Id,
                    UserId = userId,
                    JoinedAt = DateTimeOffset.UtcNow,
                    VideoEnabled = request.VideoEnabled,
                    AudioEnabled = request.AudioEnabled,
                    ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Connected),
                    CallAccepted = true,
                    DeviceInfo = request.DeviceInfo,
                    PeerConnectionId = Guid.NewGuid().ToString()
                };

                _context.VideoCallParticipants.Add(participant);
                callSession.Participants.Add(participant);
            }
            else
            {
                // Update existing participant
                participant.JoinedAt = DateTimeOffset.UtcNow;
                participant.LeftAt = null;
                participant.VideoEnabled = request.VideoEnabled;
                participant.AudioEnabled = request.AudioEnabled;
                participant.ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Connected);
                participant.CallAccepted = true;
                participant.DeviceInfo = request.DeviceInfo;
            }

            // Update call status if needed
            if (callStatus == CallStatus.Pending || callStatus == CallStatus.Ringing)
            {
                callSession.Status = EnumConverter.ToString(CallStatus.Active);
                callSession.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} joined video call {CallId}", userId, request.CallId);

            return await MapToDto(callSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining video call {CallId} for user {UserId}", request.CallId, userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> LeaveCallAsync(string userId, string callId)
    {
        try
        {
            var callSession = await _context.VideoCallSessions
                .Include(v => v.Participants)
                .FirstOrDefaultAsync(v => v.Id == callId);

            if (callSession == null)
            {
                return false;
            }

            var participant = callSession.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                return false;
            }

            // Mark participant as left
            participant.LeftAt = DateTimeOffset.UtcNow;
            participant.ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Disconnected);

            // Check if this was the last active participant
            var activeParticipants = callSession.Participants.Count(p => p.LeftAt == null);
            if (activeParticipants <= 1)
            {
                // End the call if only one or no participants remain
                callSession.Status = EnumConverter.ToString(CallStatus.Ended);
                callSession.EndTime = DateTimeOffset.UtcNow;
                callSession.DurationSeconds = (int)(callSession.EndTime.Value - callSession.StartTime).TotalSeconds;
            }

            callSession.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} left video call {CallId}", userId, callId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving video call {CallId} for user {UserId}", callId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> EndCallAsync(string userId, string callId)
    {
        try
        {
            var callSession = await _context.VideoCallSessions
                .Include(v => v.Participants)
                .FirstOrDefaultAsync(v => v.Id == callId);

            if (callSession == null)
            {
                return false;
            }

            // Check if user is the initiator or has permission to end the call
            if (callSession.InitiatorId != userId)
            {
                throw new UnauthorizedAccessException("Only the call initiator can end the call");
            }

            // End the call
            callSession.Status = EnumConverter.ToString(CallStatus.Ended);
            callSession.EndTime = DateTimeOffset.UtcNow;
            callSession.DurationSeconds = (int)(callSession.EndTime.Value - callSession.StartTime).TotalSeconds;
            callSession.UpdatedAt = DateTimeOffset.UtcNow;

            // Mark all participants as left
            foreach (var participant in callSession.Participants.Where(p => p.LeftAt == null))
            {
                participant.LeftAt = callSession.EndTime.Value;
                participant.ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Disconnected);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Video call {CallId} ended by user {UserId}", callId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending video call {CallId} for user {UserId}", callId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AcceptCallAsync(string userId, string callId)
    {
        try
        {
            var participant = await _context.VideoCallParticipants
                .Include(p => p.VideoCallSession)
                .FirstOrDefaultAsync(p => p.VideoCallSessionId == callId && p.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.CallAccepted = true;
            participant.ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Connected);

            // Update call status if this was a pending call
            var callStatus = EnumConverter.ToCallStatus(participant.VideoCallSession.Status);
            if (callStatus == CallStatus.Ringing || callStatus == CallStatus.Pending)
            {
                participant.VideoCallSession.Status = EnumConverter.ToString(CallStatus.Active);
                participant.VideoCallSession.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} accepted video call {CallId}", userId, callId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting video call {CallId} for user {UserId}", callId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RejectCallAsync(string userId, string callId, string? reason = null)
    {
        try
        {
            var participant = await _context.VideoCallParticipants
                .Include(p => p.VideoCallSession)
                    .ThenInclude(v => v.Participants)
                .FirstOrDefaultAsync(p => p.VideoCallSessionId == callId && p.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.CallAccepted = false;
            participant.RejectionReason = reason;
            participant.LeftAt = DateTimeOffset.UtcNow;
            participant.ConnectionStatus = EnumConverter.ToString(ConnectionStatus.Disconnected);

            // Check if all participants have rejected the call
            var allRejected = participant.VideoCallSession.Participants
                .Where(p => p.UserId != participant.VideoCallSession.InitiatorId)
                .All(p => !p.CallAccepted || p.LeftAt != null);

            if (allRejected)
            {
                participant.VideoCallSession.Status = EnumConverter.ToString(CallStatus.Rejected);
                participant.VideoCallSession.EndTime = DateTimeOffset.UtcNow;
                participant.VideoCallSession.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} rejected video call {CallId} with reason: {Reason}", 
                userId, callId, reason ?? "No reason provided");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting video call {CallId} for user {UserId}", callId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<VideoCallParticipantDto?> UpdateParticipantAsync(string userId, UpdateCallParticipantRequest request)
    {
        try
        {
            var participant = await _context.VideoCallParticipants
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.VideoCallSessionId == request.CallId && p.UserId == userId);

            if (participant == null)
            {
                return null;
            }

            // Update participant settings
            if (request.VideoEnabled.HasValue)
                participant.VideoEnabled = request.VideoEnabled.Value;

            if (request.AudioEnabled.HasValue)
                participant.AudioEnabled = request.AudioEnabled.Value;

            if (request.ScreenSharing.HasValue)
                participant.ScreenSharing = request.ScreenSharing.Value;

            if (request.AudioLevel.HasValue)
                participant.AudioLevel = Math.Max(0, Math.Min(100, request.AudioLevel.Value));

            if (request.IsSpeaking.HasValue)
                participant.IsSpeaking = request.IsSpeaking.Value;

            if (request.NetworkQuality.HasValue)
                participant.NetworkQuality = EnumConverter.ToString(request.NetworkQuality.Value);

            await _context.SaveChangesAsync();

            _logger.LogDebug("Updated participant settings for user {UserId} in call {CallId}", userId, request.CallId);

            return MapParticipantToDto(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant settings for user {UserId} in call {CallId}", 
                userId, request.CallId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> HandleSignalingAsync(string userId, WebRTCSignalingRequest request)
    {
        try
        {
            var callSession = await _context.VideoCallSessions
                .FirstOrDefaultAsync(v => v.Id == request.CallId);

            if (callSession == null)
            {
                return false;
            }

            // Store signaling data based on type
            switch (request.Type.ToLower())
            {
                case "offer":
                    callSession.SdpOffer = request.Sdp;
                    break;
                    
                case "answer":
                    callSession.SdpAnswer = request.Sdp;
                    break;
                    
                case "ice-candidate":
                    // Store ICE candidates as JSON array
                    var candidates = new List<object>();
                    if (!string.IsNullOrEmpty(callSession.IceCandidates))
                    {
                        try
                        {
                            candidates = JsonSerializer.Deserialize<List<object>>(callSession.IceCandidates) ?? new List<object>();
                        }
                        catch
                        {
                            candidates = new List<object>();
                        }
                    }
                    
                    if (request.Candidate != null)
                    {
                        candidates.Add(request.Candidate);
                        callSession.IceCandidates = JsonSerializer.Serialize(candidates);
                    }
                    break;
            }

            callSession.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogDebug("Processed WebRTC signaling message of type {Type} for call {CallId}", 
                request.Type, request.CallId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling WebRTC signaling for call {CallId}", request.CallId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<VideoCallDto?> GetCallAsync(string userId, string callId)
    {
        try
        {
            var callSession = await _context.VideoCallSessions
                .Include(v => v.Participants)
                    .ThenInclude(p => p.User)
                .Include(v => v.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(v => v.Id == callId);

            if (callSession == null)
            {
                return null;
            }

            // Check if user has access to this call
            if (!callSession.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return null;
            }

            return await MapToDto(callSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting video call {CallId} for user {UserId}", callId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<VideoCallDto>> GetActiveCallsAsync(string userId)
    {
        try
        {
            var activeCalls = await _context.VideoCallSessions
                .Include(v => v.Participants)
                    .ThenInclude(p => p.User)
                .Include(v => v.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(v => v.Conversation.Participants.Any(p => p.UserId == userId) &&
                           (v.Status == EnumConverter.ToString(CallStatus.Pending) ||
                            v.Status == EnumConverter.ToString(CallStatus.Ringing) ||
                            v.Status == EnumConverter.ToString(CallStatus.Active)))
                .OrderByDescending(v => v.StartTime)
                .ToListAsync();

            var results = new List<VideoCallDto>();
            foreach (var call in activeCalls)
            {
                results.Add(await MapToDto(call));
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active calls for user {UserId}", userId);
            return new List<VideoCallDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<VideoCallDto>> GetCallHistoryAsync(string userId, string conversationId, int skip = 0, int take = 20)
    {
        try
        {
            // Validate user has access to conversation
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return new List<VideoCallDto>();
            }

            var callHistory = await _context.VideoCallSessions
                .Include(v => v.Participants)
                    .ThenInclude(p => p.User)
                .Where(v => v.ConversationId == conversationId &&
                           (v.Status == EnumConverter.ToString(CallStatus.Ended) ||
                            v.Status == EnumConverter.ToString(CallStatus.Cancelled) ||
                            v.Status == EnumConverter.ToString(CallStatus.Rejected)))
                .OrderByDescending(v => v.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var results = new List<VideoCallDto>();
            foreach (var call in callHistory)
            {
                results.Add(await MapToDto(call));
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting call history for conversation {ConversationId} and user {UserId}", 
                conversationId, userId);
            return new List<VideoCallDto>();
        }
    }

    /// <inheritdoc />
    public async Task<bool> RateCallAsync(string userId, string callId, int rating)
    {
        try
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            var callSession = await _context.VideoCallSessions
                .Include(v => v.Participants)
                .FirstOrDefaultAsync(v => v.Id == callId);

            if (callSession == null)
            {
                return false;
            }

            // Check if user participated in the call
            if (!callSession.Participants.Any(p => p.UserId == userId))
            {
                return false;
            }

            // Only allow rating completed calls
            var callStatus = EnumConverter.ToCallStatus(callSession.Status);
            if (callStatus != CallStatus.Ended)
            {
                return false;
            }

            callSession.QualityRating = rating;
            callSession.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} rated video call {CallId} with {Rating} stars", 
                userId, callId, rating);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating video call {CallId} for user {UserId}", callId, userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<object> GetCallStatisticsAsync(string userId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        try
        {
            var userCalls = await _context.VideoCallSessions
                .Include(v => v.Participants)
                .Include(v => v.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(v => v.Conversation.Participants.Any(p => p.UserId == userId) &&
                           v.StartTime >= fromDate && v.StartTime <= toDate)
                .ToListAsync();

            var statistics = new
            {
                TotalCalls = userCalls.Count,
                CompletedCalls = userCalls.Count(c => EnumConverter.ToCallStatus(c.Status) == CallStatus.Ended),
                TotalDuration = userCalls.Where(c => c.DurationSeconds.HasValue).Sum(c => c.DurationSeconds!.Value),
                AverageDuration = userCalls.Where(c => c.DurationSeconds.HasValue).Any() 
                    ? userCalls.Where(c => c.DurationSeconds.HasValue).Average(c => c.DurationSeconds!.Value) 
                    : 0,
                AverageRating = userCalls.Where(c => c.QualityRating.HasValue).Any()
                    ? userCalls.Where(c => c.QualityRating.HasValue).Average(c => c.QualityRating!.Value)
                    : (double?)null,
                CallsByStatus = userCalls.GroupBy(c => c.Status)
                    .ToDictionary(g => g.Key, g => g.Count()),
                CallsInitiated = userCalls.Count(c => c.InitiatorId == userId),
                CallsReceived = userCalls.Count(c => c.InitiatorId != userId),
                ScreenSharingUsage = userCalls.Count(c => c.ScreenSharingUsed),
                RecordedCalls = userCalls.Count(c => c.IsRecorded)
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting call statistics for user {UserId}", userId);
            return new { Error = "Failed to retrieve statistics" };
        }
    }

    /// <summary>
    /// Generate a unique room ID for the video call
    /// </summary>
    private static string GenerateRoomId()
    {
        return $"room_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}".Substring(0, 32);
    }

    /// <summary>
    /// Map VideoCallSession entity to DTO
    /// </summary>
    private async Task<VideoCallDto> MapToDto(VideoCallSession callSession)
    {
        var initiator = await _context.Users.FindAsync(callSession.InitiatorId);
        
        return new VideoCallDto
        {
            Id = callSession.Id,
            ConversationId = callSession.ConversationId,
            InitiatorId = callSession.InitiatorId,
            InitiatorName = initiator?.UserName ?? "Unknown",
            StartTime = callSession.StartTime,
            EndTime = callSession.EndTime,
            DurationSeconds = callSession.DurationSeconds,
            Status = EnumConverter.ToCallStatus(callSession.Status),
            RoomId = callSession.RoomId,
            QualityRating = callSession.QualityRating,
            IsRecorded = callSession.IsRecorded,
            RecordingUrl = callSession.RecordingUrl,
            ScreenSharingUsed = callSession.ScreenSharingUsed,
            MaxParticipants = callSession.MaxParticipants,
            NetworkQuality = string.IsNullOrEmpty(callSession.NetworkQuality) 
                ? null 
                : EnumConverter.ToNetworkQuality(callSession.NetworkQuality),
            CreatedAt = callSession.CreatedAt,
            Participants = callSession.Participants.Select(MapParticipantToDto).ToList()
        };
    }

    /// <summary>
    /// Map VideoCallParticipant entity to DTO
    /// </summary>
    private VideoCallParticipantDto MapParticipantToDto(VideoCallParticipant participant)
    {
        return new VideoCallParticipantDto
        {
            Id = participant.Id,
            UserId = participant.UserId,
            UserName = participant.User?.UserName ?? "Unknown",
            JoinedAt = participant.JoinedAt,
            LeftAt = participant.LeftAt,
            VideoEnabled = participant.VideoEnabled,
            AudioEnabled = participant.AudioEnabled,
            ScreenSharing = participant.ScreenSharing,
            ConnectionStatus = EnumConverter.ToConnectionStatus(participant.ConnectionStatus),
            NetworkQuality = string.IsNullOrEmpty(participant.NetworkQuality) 
                ? null 
                : EnumConverter.ToNetworkQuality(participant.NetworkQuality),
            CallAccepted = participant.CallAccepted,
            AudioLevel = participant.AudioLevel,
            IsSpeaking = participant.IsSpeaking,
            DeviceInfo = participant.DeviceInfo
        };
    }
}