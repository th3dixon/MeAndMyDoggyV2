using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for integrating messaging with push notifications
/// </summary>
public interface IMessagingNotificationService
{
    /// <summary>
    /// Send push notification for new message
    /// </summary>
    /// <param name="message">Message that was sent</param>
    /// <param name="conversationTitle">Title of the conversation</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyNewMessageAsync(MessageDto message, string conversationTitle);

    /// <summary>
    /// Send push notification for new voice message
    /// </summary>
    /// <param name="voiceMessage">Voice message that was sent</param>
    /// <param name="conversationTitle">Title of the conversation</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyNewVoiceMessageAsync(VoiceMessageDto voiceMessage, string conversationTitle);

    /// <summary>
    /// Send push notification for incoming call
    /// </summary>
    /// <param name="callSession">Video call session</param>
    /// <param name="callerName">Name of the caller</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyIncomingCallAsync(VideoCallSessionDto callSession, string callerName);

    /// <summary>
    /// Send push notification for missed call
    /// </summary>
    /// <param name="callSession">Video call session</param>
    /// <param name="callerName">Name of the caller</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyMissedCallAsync(VideoCallSessionDto callSession, string callerName);

    /// <summary>
    /// Send push notification for message mention
    /// </summary>
    /// <param name="message">Message containing the mention</param>
    /// <param name="mentionedUserId">User ID who was mentioned</param>
    /// <param name="conversationTitle">Title of the conversation</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyMessageMentionAsync(MessageDto message, string mentionedUserId, string conversationTitle);

    /// <summary>
    /// Send push notification for new booking request
    /// </summary>
    /// <param name="serviceProviderId">Service provider ID</param>
    /// <param name="petOwnerName">Name of the pet owner</param>
    /// <param name="serviceName">Name of the service</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyBookingRequestAsync(string serviceProviderId, string petOwnerName, string serviceName);

    /// <summary>
    /// Send push notification for booking confirmation
    /// </summary>
    /// <param name="petOwnerId">Pet owner ID</param>
    /// <param name="providerName">Name of the service provider</param>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="appointmentDateTime">Appointment date and time</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyBookingConfirmedAsync(string petOwnerId, string providerName, string serviceName, DateTimeOffset appointmentDateTime);

    /// <summary>
    /// Send push notification for booking reminder
    /// </summary>
    /// <param name="userId">User ID to notify</param>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="appointmentDateTime">Appointment date and time</param>
    /// <param name="reminderType">Type of reminder (1 hour, 1 day, etc.)</param>
    /// <returns>True if notification was sent successfully</returns>
    Task<bool> NotifyBookingReminderAsync(string userId, string serviceName, DateTimeOffset appointmentDateTime, string reminderType);
}