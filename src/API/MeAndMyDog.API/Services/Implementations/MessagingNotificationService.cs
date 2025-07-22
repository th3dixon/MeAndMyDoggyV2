using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for integrating messaging with push notifications
/// </summary>
public class MessagingNotificationService : IMessagingNotificationService
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagingNotificationService> _logger;

    /// <summary>
    /// Initialize the messaging notification service
    /// </summary>
    public MessagingNotificationService(
        IPushNotificationService pushNotificationService,
        ApplicationDbContext context,
        ILogger<MessagingNotificationService> logger)
    {
        _pushNotificationService = pushNotificationService;
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> NotifyNewMessageAsync(MessageDto message, string conversationTitle)
    {
        try
        {
            // Get other participants in the conversation
            var otherParticipants = await _context.ConversationParticipants
                .Include(cp => cp.User)
                .Where(cp => cp.ConversationId == message.ConversationId && 
                            cp.UserId != message.SenderId)
                .ToListAsync();

            if (!otherParticipants.Any())
            {
                return true; // No one to notify
            }

            var notification = new SendPushNotificationRequest
            {
                NotificationType = NotificationType.NewMessage.ToString(),
                Title = $"New message in {conversationTitle}",
                Body = GetMessagePreview(message.Content, message.MessageType.ToString()),
                IconUrl = "/images/message-icon.png",
                Sound = "message",
                Badge = 1,
                ActionUrl = $"/conversations/{message.ConversationId}",
                Priority = "normal",
                CustomData = new Dictionary<string, object>
                {
                    ["conversationId"] = message.ConversationId,
                    ["messageId"] = message.Id,
                    ["senderId"] = message.SenderId,
                    ["senderName"] = message.SenderName,
                    ["messageType"] = message.MessageType.ToString()
                }
            };

            var targetUserIds = otherParticipants.Select(p => p.UserId).ToList();
            var response = await _pushNotificationService.SendNotificationToUsersAsync(
                targetUserIds, notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("New message notification sent for message {MessageId}", message.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send new message notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending new message notification for message {MessageId}", message.Id);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyNewVoiceMessageAsync(VoiceMessageDto voiceMessage, string conversationTitle)
    {
        try
        {
            // Get other participants in the conversation
            var otherParticipants = await _context.ConversationParticipants
                .Include(cp => cp.User)
                .Where(cp => cp.ConversationId == voiceMessage.ConversationId && 
                            cp.UserId != voiceMessage.SenderId)
                .ToListAsync();

            if (!otherParticipants.Any())
            {
                return true; // No one to notify
            }

            var durationText = voiceMessage.Duration.HasValue 
                ? $" ({TimeSpan.FromSeconds(voiceMessage.Duration.Value):mm\\:ss})"
                : "";

            var notification = new SendPushNotificationRequest
            {
                NotificationType = NotificationType.VoiceMessage.ToString(),
                Title = $"Voice message in {conversationTitle}",
                Body = $"{voiceMessage.SenderName} sent a voice message{durationText}",
                IconUrl = "/images/voice-icon.png",
                Sound = "voice_message",
                Badge = 1,
                ActionUrl = $"/conversations/{voiceMessage.ConversationId}",
                Priority = "normal",
                CustomData = new Dictionary<string, object>
                {
                    ["conversationId"] = voiceMessage.ConversationId,
                    ["voiceMessageId"] = voiceMessage.Id,
                    ["senderId"] = voiceMessage.SenderId,
                    ["senderName"] = voiceMessage.SenderName,
                    ["duration"] = voiceMessage.Duration ?? 0
                }
            };

            var targetUserIds = otherParticipants.Select(p => p.UserId).ToList();
            var response = await _pushNotificationService.SendNotificationToUsersAsync(
                targetUserIds, notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Voice message notification sent for voice message {VoiceMessageId}", voiceMessage.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send voice message notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending voice message notification for voice message {VoiceMessageId}", voiceMessage.Id);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyIncomingCallAsync(VideoCallSessionDto callSession, string callerName)
    {
        try
        {
            if (string.IsNullOrEmpty(callSession.RecipientId))
            {
                return false;
            }

            var notification = new SendPushNotificationRequest
            {
                TargetUserId = callSession.RecipientId,
                NotificationType = NotificationType.IncomingCall.ToString(),
                Title = "Incoming call",
                Body = $"{callerName} is calling you",
                IconUrl = "/images/call-icon.png",
                Sound = "ringtone",
                Badge = 1,
                ActionUrl = $"/calls/{callSession.Id}",
                Priority = "high", // High priority for incoming calls
                CustomData = new Dictionary<string, object>
                {
                    ["callSessionId"] = callSession.Id,
                    ["callerId"] = callSession.InitiatorId,
                    ["callerName"] = callerName,
                    ["callType"] = "video"
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Incoming call notification sent for call session {CallSessionId}", callSession.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send incoming call notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incoming call notification for call session {CallSessionId}", callSession.Id);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyMissedCallAsync(VideoCallSessionDto callSession, string callerName)
    {
        try
        {
            if (string.IsNullOrEmpty(callSession.RecipientId))
            {
                return false;
            }

            var notification = new SendPushNotificationRequest
            {
                TargetUserId = callSession.RecipientId,
                NotificationType = NotificationType.MissedCall.ToString(),
                Title = "Missed call",
                Body = $"Missed call from {callerName}",
                IconUrl = "/images/missed-call-icon.png",
                Sound = "default",
                Badge = 1,
                ActionUrl = $"/conversations/{callSession.ConversationId}",
                Priority = "normal",
                CustomData = new Dictionary<string, object>
                {
                    ["callSessionId"] = callSession.Id,
                    ["callerId"] = callSession.InitiatorId,
                    ["callerName"] = callerName,
                    ["callType"] = "video",
                    ["conversationId"] = callSession.ConversationId ?? ""
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Missed call notification sent for call session {CallSessionId}", callSession.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send missed call notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending missed call notification for call session {CallSessionId}", callSession.Id);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyMessageMentionAsync(MessageDto message, string mentionedUserId, string conversationTitle)
    {
        try
        {
            // Check if the mentioned user allows mention notifications
            var isAllowed = await _pushNotificationService.IsNotificationAllowedAsync(
                mentionedUserId, NotificationType.MessageMention);
            
            if (!isAllowed)
            {
                _logger.LogDebug("Mention notification blocked by user preferences for user {UserId}", mentionedUserId);
                return true; // Not an error, just blocked by preferences
            }

            var notification = new SendPushNotificationRequest
            {
                TargetUserId = mentionedUserId,
                NotificationType = NotificationType.MessageMention.ToString(),
                Title = $"You were mentioned in {conversationTitle}",
                Body = $"{message.SenderName}: {GetMessagePreview(message.Content, message.MessageType.ToString())}",
                IconUrl = "/images/mention-icon.png",
                Sound = "mention",
                Badge = 1,
                ActionUrl = $"/conversations/{message.ConversationId}",
                Priority = "high", // High priority for mentions
                CustomData = new Dictionary<string, object>
                {
                    ["conversationId"] = message.ConversationId,
                    ["messageId"] = message.Id,
                    ["senderId"] = message.SenderId,
                    ["senderName"] = message.SenderName,
                    ["mentionedUserId"] = mentionedUserId
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Mention notification sent for message {MessageId}", message.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send mention notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending mention notification for message {MessageId}", message.Id);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyBookingRequestAsync(string serviceProviderId, string petOwnerName, string serviceName)
    {
        try
        {
            var notification = new SendPushNotificationRequest
            {
                TargetUserId = serviceProviderId,
                NotificationType = NotificationType.BookingRequest.ToString(),
                Title = "New booking request",
                Body = $"{petOwnerName} has requested {serviceName}",
                IconUrl = "/images/booking-icon.png",
                Sound = "booking",
                Badge = 1,
                ActionUrl = "/bookings/pending",
                Priority = "normal",
                CustomData = new Dictionary<string, object>
                {
                    ["petOwnerName"] = petOwnerName,
                    ["serviceName"] = serviceName,
                    ["notificationType"] = "booking_request"
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Booking request notification sent to service provider {ServiceProviderId}", serviceProviderId);
            }
            else
            {
                _logger.LogWarning("Failed to send booking request notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking request notification to service provider {ServiceProviderId}", serviceProviderId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyBookingConfirmedAsync(string petOwnerId, string providerName, string serviceName, DateTimeOffset appointmentDateTime)
    {
        try
        {
            var appointmentDateString = appointmentDateTime.ToString("MMM d, yyyy 'at' h:mm tt");

            var notification = new SendPushNotificationRequest
            {
                TargetUserId = petOwnerId,
                NotificationType = NotificationType.BookingConfirmed.ToString(),
                Title = "Booking confirmed",
                Body = $"Your {serviceName} appointment with {providerName} is confirmed for {appointmentDateString}",
                IconUrl = "/images/confirmed-icon.png",
                Sound = "confirmation",
                Badge = 1,
                ActionUrl = "/bookings",
                Priority = "normal",
                CustomData = new Dictionary<string, object>
                {
                    ["providerName"] = providerName,
                    ["serviceName"] = serviceName,
                    ["appointmentDateTime"] = appointmentDateTime.ToString("O"),
                    ["notificationType"] = "booking_confirmed"
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Booking confirmation notification sent to pet owner {PetOwnerId}", petOwnerId);
            }
            else
            {
                _logger.LogWarning("Failed to send booking confirmation notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking confirmation notification to pet owner {PetOwnerId}", petOwnerId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> NotifyBookingReminderAsync(string userId, string serviceName, DateTimeOffset appointmentDateTime, string reminderType)
    {
        try
        {
            var appointmentDateString = appointmentDateTime.ToString("MMM d, yyyy 'at' h:mm tt");
            
            var notification = new SendPushNotificationRequest
            {
                TargetUserId = userId,
                NotificationType = NotificationType.BookingReminder.ToString(),
                Title = $"Appointment reminder ({reminderType})",
                Body = $"Your {serviceName} appointment is scheduled for {appointmentDateString}",
                IconUrl = "/images/reminder-icon.png",
                Sound = "reminder",
                Badge = 1,
                ActionUrl = "/bookings",
                Priority = "high", // High priority for reminders
                CustomData = new Dictionary<string, object>
                {
                    ["serviceName"] = serviceName,
                    ["appointmentDateTime"] = appointmentDateTime.ToString("O"),
                    ["reminderType"] = reminderType,
                    ["notificationType"] = "booking_reminder"
                }
            };

            var response = await _pushNotificationService.SendNotificationAsync(notification, "system");

            if (response.Success)
            {
                _logger.LogDebug("Booking reminder notification sent to user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to send booking reminder notification: {Message}", response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking reminder notification to user {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Get a preview of message content for notifications
    /// </summary>
    private static string GetMessagePreview(string content, string messageType)
    {
        const int maxLength = 100;

        return messageType.ToLowerInvariant() switch
        {
            "image" => "📷 Image",
            "file" => "📎 File",
            "audio" => "🎵 Audio",
            "video" => "🎥 Video",
            "location" => "📍 Location",
            "contact" => "👤 Contact",
            _ => content.Length > maxLength ? content.Substring(0, maxLength) + "..." : content
        };
    }
}