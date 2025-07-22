using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Security.Claims;
using System.Text.Json;
using System.Collections.Concurrent;

namespace MeAndMyDog.API.Hubs;

/// <summary>
/// SignalR hub for real-time messaging functionality
/// </summary>
[Authorize]
public class MessagingHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagingHub> _logger;
    private readonly IVideoCallService _videoCallService;
    private readonly IVoiceMessageService _voiceMessageService;
    
    // Static dictionaries to track connections and typing status
    private static readonly ConcurrentDictionary<string, HashSet<string>> _conversationConnections = new();
    private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
    private static readonly ConcurrentDictionary<string, Dictionary<string, DateTimeOffset>> _typingUsers = new();
    
    // Static dictionaries to track video calls
    private static readonly ConcurrentDictionary<string, HashSet<string>> _callConnections = new();
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _callParticipants = new();

    /// <summary>
    /// Initialize the messaging hub
    /// </summary>
    public MessagingHub(ApplicationDbContext context, ILogger<MessagingHub> logger, IVideoCallService videoCallService, IVoiceMessageService voiceMessageService)
    {
        _context = context;
        _logger = logger;
        _videoCallService = videoCallService;
        _voiceMessageService = voiceMessageService;
    }

    /// <summary>
    /// Handle new client connections
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            // Track user connections
            _userConnections.AddOrUpdate(userId, 
                new HashSet<string> { Context.ConnectionId },
                (key, existing) => { existing.Add(Context.ConnectionId); return existing; });

            // Update user's online status
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Notify contacts that user is online
            await NotifyUserStatusChange(userId, true);
            
            _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Handle client disconnections
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            // Remove connection from tracking
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    _userConnections.TryRemove(userId, out _);
                    
                    // User is fully offline, notify contacts
                    await NotifyUserStatusChange(userId, false);
                }
            }

            // Clean up conversation connections
            foreach (var kvp in _conversationConnections.ToList())
            {
                var conversationId = kvp.Key;
                var conversationConnections = kvp.Value;
                
                if (conversationConnections.Remove(Context.ConnectionId) && conversationConnections.Count == 0)
                {
                    _conversationConnections.TryRemove(conversationId, out _);
                }
            }

            // Clean up typing indicators
            foreach (var kvp in _typingUsers.ToList())
            {
                var conversationId = kvp.Key;
                var typingUsers = kvp.Value;
                
                if (typingUsers.ContainsKey(userId))
                {
                    typingUsers.Remove(userId);
                    
                    // Notify that user stopped typing
                    await Clients.Group($"conversation_{conversationId}").SendAsync("UserStoppedTyping", new
                    {
                        ConversationId = conversationId,
                        UserId = userId
                    });
                    
                    if (typingUsers.Count == 0)
                    {
                        _typingUsers.TryRemove(conversationId, out _);
                    }
                }
            }

            _logger.LogInformation("User {UserId} disconnected from connection {ConnectionId}", userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="content">Message content</param>
    /// <param name="messageType">Type of message</param>
    /// <param name="parentMessageId">Optional parent message for replies</param>
    [HubMethodName("SendMessage")]
    public async Task SendMessage(string conversationId, string content, MessageType messageType = MessageType.Text, string? parentMessageId = null)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            // Validate conversation and user participation
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                await Clients.Caller.SendAsync("Error", "Conversation not found");
                return;
            }

            if (!conversation.Participants.Any(p => p.UserId == userId))
            {
                await Clients.Caller.SendAsync("Error", "User is not a participant in this conversation");
                return;
            }

            // Validate message content
            if (string.IsNullOrWhiteSpace(content) && messageType == MessageType.Text)
            {
                await Clients.Caller.SendAsync("Error", "Message content cannot be empty");
                return;
            }

            if (content.Length > 4000)
            {
                await Clients.Caller.SendAsync("Error", "Message content cannot exceed 4000 characters");
                return;
            }

            // Create message entity
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                SenderId = userId,
                MessageType = EnumConverter.ToString(messageType),
                Content = content?.Trim() ?? string.Empty,
                ParentMessageId = parentMessageId,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(MessageStatus.Sent)
            };

            _context.Messages.Add(message);

            // Update conversation metadata
            conversation.LastMessageId = message.Id;
            conversation.LastMessageAt = message.CreatedAt;
            conversation.LastMessagePreview = message.Content.Length > 200 
                ? message.Content.Substring(0, 200) + "..." 
                : message.Content;
            conversation.MessageCount++;
            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            // Update unread counts for other participants
            var otherParticipants = conversation.Participants.Where(p => p.UserId != userId).ToList();
            foreach (var participant in otherParticipants)
            {
                participant.UnreadCount++;
            }

            await _context.SaveChangesAsync();

            // Get sender information
            var sender = await _context.Users.FindAsync(userId);
            
            // Create message DTO for broadcast
            var messageDto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = sender?.UserName ?? "Unknown",
                MessageType = EnumConverter.ToMessageType(message.MessageType),
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                Status = EnumConverter.ToMessageStatus(message.Status),
                IsEdited = false
            };

            // Broadcast message to all conversation participants
            await Clients.Group($"conversation_{conversationId}").SendAsync("MessageReceived", messageDto);

            // Send unread count updates to participants
            foreach (var participant in otherParticipants)
            {
                if (_userConnections.ContainsKey(participant.UserId))
                {
                    await Clients.Group($"user_{participant.UserId}").SendAsync("UnreadCountUpdated", new
                    {
                        ConversationId = conversationId,
                        UnreadCount = participant.UnreadCount
                    });
                }
            }

            // Clear typing indicator for sender
            if (_typingUsers.TryGetValue(conversationId, out var typingUsers) && typingUsers.ContainsKey(userId))
            {
                typingUsers.Remove(userId);
                await Clients.Group($"conversation_{conversationId}").SendAsync("UserStoppedTyping", new
                {
                    ConversationId = conversationId,
                    UserId = userId
                });
            }

            _logger.LogInformation("Message {MessageId} sent successfully in conversation {ConversationId} by user {UserId}", 
                message.Id, conversationId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId} by user {UserId}", 
                conversationId, Context.UserIdentifier);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    /// <summary>
    /// Join a conversation to receive real-time updates
    /// </summary>
    /// <param name="conversationId">Conversation ID to join</param>
    [HubMethodName("JoinConversation")]
    public async Task JoinConversation(string conversationId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            // Verify user is a participant
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                await Clients.Caller.SendAsync("Error", "Access denied to this conversation");
                return;
            }

            // Add connection to conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            
            // Track connection for this conversation
            _conversationConnections.AddOrUpdate(conversationId,
                new HashSet<string> { Context.ConnectionId },
                (key, existing) => { existing.Add(Context.ConnectionId); return existing; });

            await Clients.Caller.SendAsync("JoinedConversation", new { ConversationId = conversationId });

            _logger.LogDebug("User {UserId} joined conversation {ConversationId}", userId, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining conversation {ConversationId} for user {UserId}", 
                conversationId, Context.UserIdentifier);
            await Clients.Caller.SendAsync("Error", "Failed to join conversation");
        }
    }

    /// <summary>
    /// Leave a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID to leave</param>
    [HubMethodName("LeaveConversation")]
    public async Task LeaveConversation(string conversationId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Remove from SignalR group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");

            // Remove from tracking
            if (_conversationConnections.TryGetValue(conversationId, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    _conversationConnections.TryRemove(conversationId, out _);
                }
            }

            // Clear typing status if user was typing
            if (_typingUsers.TryGetValue(conversationId, out var typingUsers) && typingUsers.ContainsKey(userId))
            {
                typingUsers.Remove(userId);
                await Clients.Group($"conversation_{conversationId}").SendAsync("UserStoppedTyping", new
                {
                    ConversationId = conversationId,
                    UserId = userId
                });
            }

            await Clients.Caller.SendAsync("LeftConversation", new { ConversationId = conversationId });

            _logger.LogDebug("User {UserId} left conversation {ConversationId}", userId, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving conversation {ConversationId} for user {UserId}", 
                conversationId, Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Update typing indicator
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="isTyping">Whether user is currently typing</param>
    [HubMethodName("TypingIndicator")]
    public async Task TypingIndicator(string conversationId, bool isTyping)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify user is a participant
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                return;
            }

            var typingUsers = _typingUsers.GetOrAdd(conversationId, _ => new Dictionary<string, DateTimeOffset>());

            if (isTyping)
            {
                // User started typing
                typingUsers[userId] = DateTimeOffset.UtcNow;
                
                await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserStartedTyping", new
                {
                    ConversationId = conversationId,
                    UserId = userId
                });
            }
            else
            {
                // User stopped typing
                typingUsers.Remove(userId);
                
                await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserStoppedTyping", new
                {
                    ConversationId = conversationId,
                    UserId = userId
                });
            }

            _logger.LogDebug("User {UserId} typing status updated in conversation {ConversationId}: {IsTyping}", 
                userId, conversationId, isTyping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating typing indicator for conversation {ConversationId} by user {UserId}", 
                conversationId, Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Mark messages as read
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="messageId">Last read message ID</param>
    [HubMethodName("MarkAsRead")]
    public async Task MarkAsRead(string conversationId, string messageId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify message exists and user has access
            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ConversationId == conversationId);

            if (message == null || !message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return;
            }

            // Update participant's read status
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant != null)
            {
                participant.LastReadMessageId = messageId;
                participant.LastReadAt = DateTimeOffset.UtcNow;
                participant.UnreadCount = 0; // Reset unread count
                
                await _context.SaveChangesAsync();

                // Create read receipt
                var existingReceipt = await _context.MessageReadReceipts
                    .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);

                if (existingReceipt == null)
                {
                    var readReceipt = new MessageReadReceipt
                    {
                        MessageId = messageId,
                        UserId = userId,
                        ReadAt = DateTimeOffset.UtcNow,
                        DeviceInfo = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString()
                    };

                    _context.MessageReadReceipts.Add(readReceipt);
                    await _context.SaveChangesAsync();
                }

                // Notify message sender about read receipt
                await Clients.Group($"user_{message.SenderId}").SendAsync("MessageRead", new
                {
                    MessageId = messageId,
                    ConversationId = conversationId,
                    ReadBy = userId,
                    ReadAt = participant.LastReadAt
                });

                // Update unread count for user
                await Clients.Group($"user_{userId}").SendAsync("UnreadCountUpdated", new
                {
                    ConversationId = conversationId,
                    UnreadCount = 0
                });

                _logger.LogDebug("User {UserId} marked message {MessageId} as read in conversation {ConversationId}", 
                    userId, messageId, conversationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read for user {UserId}", Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Update message status (for delivery confirmations)
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="status">New message status</param>
    [HubMethodName("UpdateMessageStatus")]
    public async Task UpdateMessageStatus(string messageId, MessageStatus status)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null || message.SenderId != userId)
            {
                return;
            }

            message.Status = EnumConverter.ToString(status);
            await _context.SaveChangesAsync();

            // Notify participants about status update
            await Clients.Group($"conversation_{message.ConversationId}").SendAsync("MessageStatusUpdated", new
            {
                MessageId = messageId,
                Status = status
            });

            _logger.LogDebug("Message {MessageId} status updated to {Status}", messageId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message status for message {MessageId}", messageId);
        }
    }

    /// <summary>
    /// Notify contacts about user status change (online/offline)
    /// </summary>
    private async Task NotifyUserStatusChange(string userId, bool isOnline)
    {
        try
        {
            // Get all conversations where this user is a participant
            var conversationIds = await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ConversationId)
                .ToListAsync();

            // Notify all participants in those conversations
            foreach (var conversationId in conversationIds)
            {
                await Clients.Group($"conversation_{conversationId}").SendAsync("UserStatusChanged", new
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    LastSeen = DateTimeOffset.UtcNow
                });
            }

            _logger.LogDebug("User {UserId} status changed to {Status}", userId, isOnline ? "online" : "offline");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying user status change for user {UserId}", userId);
        }
    }

    // ============================================
    // VIDEO CALLING SIGNALR METHODS
    // ============================================

    /// <summary>
    /// Join a video call room for real-time signaling
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    [HubMethodName("JoinVideoCall")]
    public async Task JoinVideoCall(string callId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("VideoCallError", "User not authenticated");
                return;
            }

            // Verify call exists and user is authorized
            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null)
            {
                await Clients.Caller.SendAsync("VideoCallError", "Video call not found or access denied");
                return;
            }

            // Add to call group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"videocall_{callId}");
            
            // Track connection for this call
            _callConnections.AddOrUpdate(callId,
                new HashSet<string> { Context.ConnectionId },
                (key, existing) => { existing.Add(Context.ConnectionId); return existing; });

            // Track participant
            _callParticipants.AddOrUpdate(callId,
                new Dictionary<string, string> { { userId, Context.ConnectionId } },
                (key, existing) => { existing[userId] = Context.ConnectionId; return existing; });

            // Notify other participants that user joined
            await Clients.OthersInGroup($"videocall_{callId}").SendAsync("ParticipantJoined", new
            {
                CallId = callId,
                UserId = userId,
                ParticipantInfo = call.Participants.FirstOrDefault(p => p.UserId == userId)
            });

            await Clients.Caller.SendAsync("VideoCallJoined", new { CallId = callId });

            _logger.LogInformation("User {UserId} joined video call {CallId} SignalR room", userId, callId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining video call {CallId} for user {UserId}", callId, Context.UserIdentifier);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to join video call");
        }
    }

    /// <summary>
    /// Leave a video call room
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    [HubMethodName("LeaveVideoCall")]
    public async Task LeaveVideoCall(string callId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Remove from call group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"videocall_{callId}");

            // Remove from tracking
            if (_callConnections.TryGetValue(callId, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    _callConnections.TryRemove(callId, out _);
                }
            }

            if (_callParticipants.TryGetValue(callId, out var participants))
            {
                participants.Remove(userId);
                if (participants.Count == 0)
                {
                    _callParticipants.TryRemove(callId, out _);
                }
            }

            // Notify other participants that user left
            await Clients.OthersInGroup($"videocall_{callId}").SendAsync("ParticipantLeft", new
            {
                CallId = callId,
                UserId = userId
            });

            await Clients.Caller.SendAsync("VideoCallLeft", new { CallId = callId });

            _logger.LogInformation("User {UserId} left video call {CallId} SignalR room", userId, callId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving video call {CallId} for user {UserId}", callId, Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Send WebRTC signaling messages between participants
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="targetUserId">Target participant user ID</param>
    /// <param name="signalingData">WebRTC signaling data</param>
    [HubMethodName("SendSignalingMessage")]
    public async Task SendSignalingMessage(string callId, string targetUserId, object signalingData)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("VideoCallError", "User not authenticated");
                return;
            }

            // Verify call exists and user is participant
            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null || !call.Participants.Any(p => p.UserId == userId))
            {
                await Clients.Caller.SendAsync("VideoCallError", "Access denied to video call");
                return;
            }

            // Verify target user is also a participant
            if (!call.Participants.Any(p => p.UserId == targetUserId))
            {
                await Clients.Caller.SendAsync("VideoCallError", "Target user is not in this call");
                return;
            }

            // Send signaling message to target user
            if (_callParticipants.TryGetValue(callId, out var participants) && 
                participants.TryGetValue(targetUserId, out var targetConnectionId))
            {
                await Clients.Client(targetConnectionId).SendAsync("SignalingMessageReceived", new
                {
                    CallId = callId,
                    FromUserId = userId,
                    SignalingData = signalingData
                });

                _logger.LogDebug("Signaling message sent from {FromUserId} to {ToUserId} in call {CallId}", 
                    userId, targetUserId, callId);
            }
            else
            {
                await Clients.Caller.SendAsync("VideoCallError", "Target user is not connected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending signaling message in call {CallId} from {FromUserId} to {ToUserId}", 
                callId, Context.UserIdentifier, targetUserId);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to send signaling message");
        }
    }

    /// <summary>
    /// Broadcast call status updates to participants
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="status">New call status</param>
    [HubMethodName("UpdateCallStatus")]
    public async Task UpdateCallStatus(string callId, CallStatus status)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify user has permission to update call status
            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null || call.InitiatorId != userId)
            {
                await Clients.Caller.SendAsync("VideoCallError", "Permission denied to update call status");
                return;
            }

            // Broadcast status update to all participants
            await Clients.Group($"videocall_{callId}").SendAsync("CallStatusUpdated", new
            {
                CallId = callId,
                Status = status,
                UpdatedBy = userId,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation("Call {CallId} status updated to {Status} by user {UserId}", callId, status, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating call status for call {CallId}", callId);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to update call status");
        }
    }

    /// <summary>
    /// Broadcast participant settings updates (video/audio on/off, screen sharing)
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="participantUpdate">Participant settings update</param>
    [HubMethodName("UpdateParticipantSettings")]
    public async Task UpdateParticipantSettings(string callId, object participantUpdate)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify user is participant in the call
            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null || !call.Participants.Any(p => p.UserId == userId))
            {
                await Clients.Caller.SendAsync("VideoCallError", "Access denied to video call");
                return;
            }

            // Broadcast participant update to all other participants
            await Clients.OthersInGroup($"videocall_{callId}").SendAsync("ParticipantSettingsUpdated", new
            {
                CallId = callId,
                UserId = userId,
                Settings = participantUpdate,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogDebug("Participant settings updated for user {UserId} in call {CallId}", userId, callId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant settings for user {UserId} in call {CallId}", 
                Context.UserIdentifier, callId);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to update participant settings");
        }
    }

    /// <summary>
    /// Send call invitation notifications
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="invitedUserIds">List of user IDs to invite</param>
    [HubMethodName("SendCallInvitation")]
    public async Task SendCallInvitation(string callId, List<string> invitedUserIds)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify call exists and user is the initiator
            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null || call.InitiatorId != userId)
            {
                await Clients.Caller.SendAsync("VideoCallError", "Permission denied to send call invitations");
                return;
            }

            // Send invitation to each invited user
            foreach (var invitedUserId in invitedUserIds.Distinct())
            {
                if (_userConnections.ContainsKey(invitedUserId))
                {
                    await Clients.Group($"user_{invitedUserId}").SendAsync("CallInvitationReceived", new
                    {
                        CallId = callId,
                        FromUserId = userId,
                        FromUserName = call.InitiatorName,
                        ConversationId = call.ConversationId,
                        CallType = "video",
                        InvitedAt = DateTimeOffset.UtcNow
                    });

                    _logger.LogInformation("Call invitation sent from {FromUserId} to {ToUserId} for call {CallId}", 
                        userId, invitedUserId, callId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending call invitations for call {CallId}", callId);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to send call invitations");
        }
    }

    /// <summary>
    /// Handle call responses (accept/reject)
    /// </summary>
    /// <param name="callId">Video call session ID</param>
    /// <param name="response">Accept or reject</param>
    /// <param name="reason">Optional reason for rejection</param>
    [HubMethodName("RespondToCall")]
    public async Task RespondToCall(string callId, string response, string? reason = null)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var call = await _videoCallService.GetCallAsync(userId, callId);
            if (call == null)
            {
                await Clients.Caller.SendAsync("VideoCallError", "Video call not found");
                return;
            }

            // Process the response
            bool success = false;
            if (response.ToLower() == "accept")
            {
                success = await _videoCallService.AcceptCallAsync(userId, callId);
            }
            else if (response.ToLower() == "reject")
            {
                success = await _videoCallService.RejectCallAsync(userId, callId, reason);
            }

            if (success)
            {
                // Notify call initiator and other participants
                await Clients.Group($"videocall_{callId}").SendAsync("CallResponseReceived", new
                {
                    CallId = callId,
                    UserId = userId,
                    Response = response,
                    Reason = reason,
                    RespondedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation("User {UserId} {Response} call {CallId}", userId, response, callId);
            }
            else
            {
                await Clients.Caller.SendAsync("VideoCallError", $"Failed to {response} call");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to call {CallId} for user {UserId}", callId, Context.UserIdentifier);
            await Clients.Caller.SendAsync("VideoCallError", "Failed to respond to call");
        }
    }

    // ============================================
    // VOICE MESSAGE SIGNALR METHODS
    // ============================================

    /// <summary>
    /// Notify participants about voice message processing status
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <param name="status">Processing status</param>
    [HubMethodName("NotifyVoiceMessageProcessing")]
    public async Task NotifyVoiceMessageProcessing(string voiceMessageId, string status)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Get voice message to find the conversation
            var voiceMessage = await _voiceMessageService.GetVoiceMessageAsync(userId, voiceMessageId);
            if (voiceMessage == null)
            {
                await Clients.Caller.SendAsync("VoiceMessageError", "Voice message not found");
                return;
            }

            // Get conversation ID from the message
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == voiceMessage.MessageId);

            if (message != null)
            {
                // Notify all conversation participants about processing status
                await Clients.Group($"conversation_{message.ConversationId}").SendAsync("VoiceMessageProcessingUpdate", new
                {
                    VoiceMessageId = voiceMessageId,
                    MessageId = voiceMessage.MessageId,
                    Status = status,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogDebug("Voice message {VoiceMessageId} processing status updated: {Status}", voiceMessageId, status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying voice message processing for {VoiceMessageId}", voiceMessageId);
            await Clients.Caller.SendAsync("VoiceMessageError", "Failed to update processing status");
        }
    }

    /// <summary>
    /// Notify about voice message transcription completion
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <param name="transcriptionText">Transcription text</param>
    [HubMethodName("NotifyTranscriptionComplete")]
    public async Task NotifyTranscriptionComplete(string voiceMessageId, string transcriptionText)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Get voice message to find the conversation
            var voiceMessage = await _voiceMessageService.GetVoiceMessageAsync(userId, voiceMessageId);
            if (voiceMessage == null)
            {
                return;
            }

            // Get conversation ID from the message
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == voiceMessage.MessageId);

            if (message != null)
            {
                // Notify all conversation participants about transcription completion
                await Clients.Group($"conversation_{message.ConversationId}").SendAsync("VoiceMessageTranscriptionComplete", new
                {
                    VoiceMessageId = voiceMessageId,
                    MessageId = voiceMessage.MessageId,
                    TranscriptionText = transcriptionText,
                    CompletedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation("Transcription completed for voice message {VoiceMessageId}", voiceMessageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying transcription completion for {VoiceMessageId}", voiceMessageId);
        }
    }

    /// <summary>
    /// Track voice message playback in real-time
    /// </summary>
    /// <param name="voiceMessageId">Voice message ID</param>
    /// <param name="playbackPosition">Current playback position in seconds</param>
    /// <param name="isPlaying">Whether currently playing</param>
    [HubMethodName("UpdateVoiceMessagePlayback")]
    public async Task UpdateVoiceMessagePlayback(string voiceMessageId, double playbackPosition, bool isPlaying)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Get voice message to find the conversation
            var voiceMessage = await _voiceMessageService.GetVoiceMessageAsync(userId, voiceMessageId);
            if (voiceMessage == null)
            {
                return;
            }

            // Get conversation ID from the message
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == voiceMessage.MessageId);

            if (message != null)
            {
                // Notify other participants about playback status (read receipt equivalent)
                await Clients.OthersInGroup($"conversation_{message.ConversationId}").SendAsync("VoiceMessagePlaybackUpdate", new
                {
                    VoiceMessageId = voiceMessageId,
                    MessageId = voiceMessage.MessageId,
                    UserId = userId,
                    PlaybackPosition = playbackPosition,
                    IsPlaying = isPlaying,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogDebug("Voice message {VoiceMessageId} playback updated by user {UserId}: position={Position}, playing={IsPlaying}", 
                    voiceMessageId, userId, playbackPosition, isPlaying);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating voice message playback for {VoiceMessageId} by user {UserId}", 
                voiceMessageId, Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Notify when voice message upload starts
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="tempMessageId">Temporary message ID for tracking</param>
    [HubMethodName("StartVoiceMessageUpload")]
    public async Task StartVoiceMessageUpload(string conversationId, string tempMessageId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Verify user is participant in conversation
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                await Clients.Caller.SendAsync("VoiceMessageError", "Access denied to conversation");
                return;
            }

            // Notify other participants that user is uploading a voice message
            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("VoiceMessageUploadStarted", new
            {
                ConversationId = conversationId,
                UserId = userId,
                TempMessageId = tempMessageId,
                StartedAt = DateTimeOffset.UtcNow
            });

            _logger.LogDebug("Voice message upload started by user {UserId} in conversation {ConversationId}", userId, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting voice message upload for user {UserId} in conversation {ConversationId}", 
                Context.UserIdentifier, conversationId);
            await Clients.Caller.SendAsync("VoiceMessageError", "Failed to start voice message upload");
        }
    }

    /// <summary>
    /// Update voice message upload progress
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="tempMessageId">Temporary message ID</param>
    /// <param name="progress">Upload progress percentage (0-100)</param>
    [HubMethodName("UpdateVoiceMessageUploadProgress")]
    public async Task UpdateVoiceMessageUploadProgress(string conversationId, string tempMessageId, int progress)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Notify other participants about upload progress
            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("VoiceMessageUploadProgress", new
            {
                ConversationId = conversationId,
                UserId = userId,
                TempMessageId = tempMessageId,
                Progress = progress,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogDebug("Voice message upload progress updated to {Progress}% by user {UserId}", progress, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating voice message upload progress for user {UserId}", Context.UserIdentifier);
        }
    }

    /// <summary>
    /// Background task to clean up stale typing indicators
    /// </summary>
    private static readonly Timer _typingCleanupTimer = new Timer(CleanupStaleTypingIndicators, null, 
        TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

    private static void CleanupStaleTypingIndicators(object? state)
    {
        try
        {
            var cutoff = DateTimeOffset.UtcNow.AddMinutes(-2); // Clean up typing indicators older than 2 minutes
            
            foreach (var conversationId in _typingUsers.Keys.ToList())
            {
                if (_typingUsers.TryGetValue(conversationId, out var typingUsers))
                {
                    var staleUsers = typingUsers.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();
                    
                    foreach (var staleUserId in staleUsers)
                    {
                        typingUsers.Remove(staleUserId);
                        
                        // Note: We can't send SignalR messages from a static context
                        // This cleanup is primarily for memory management
                        // Clients should implement their own timeout logic
                    }

                    if (typingUsers.Count == 0)
                    {
                        _typingUsers.TryRemove(conversationId, out _);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log errors if logger was available in static context
            // For now, silently continue
        }
    }
}