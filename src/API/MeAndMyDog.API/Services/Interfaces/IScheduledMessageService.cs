using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for scheduled message operations
/// </summary>
public interface IScheduledMessageService
{
    /// <summary>
    /// Schedule a message for future delivery
    /// </summary>
    /// <param name="userId">User ID scheduling the message</param>
    /// <param name="request">Message scheduling details</param>
    /// <returns>Scheduled message response</returns>
    Task<ScheduledMessageResponse> ScheduleMessageAsync(string userId, ScheduleMessageRequest request);

    /// <summary>
    /// Update a scheduled message
    /// </summary>
    /// <param name="userId">User ID updating the message</param>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <param name="request">Update details</param>
    /// <returns>Updated scheduled message response</returns>
    Task<ScheduledMessageResponse> UpdateScheduledMessageAsync(string userId, string scheduledMessageId, UpdateScheduledMessageRequest request);

    /// <summary>
    /// Cancel a scheduled message
    /// </summary>
    /// <param name="userId">User ID cancelling the message</param>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>True if successfully cancelled</returns>
    Task<bool> CancelScheduledMessageAsync(string userId, string scheduledMessageId);

    /// <summary>
    /// Pause a recurring scheduled message
    /// </summary>
    /// <param name="userId">User ID pausing the message</param>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>True if successfully paused</returns>
    Task<bool> PauseScheduledMessageAsync(string userId, string scheduledMessageId);

    /// <summary>
    /// Resume a paused recurring scheduled message
    /// </summary>
    /// <param name="userId">User ID resuming the message</param>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>True if successfully resumed</returns>
    Task<bool> ResumeScheduledMessageAsync(string userId, string scheduledMessageId);

    /// <summary>
    /// Get a scheduled message by ID
    /// </summary>
    /// <param name="userId">User ID requesting the message</param>
    /// <param name="scheduledMessageId">Scheduled message ID</param>
    /// <returns>Scheduled message details or null if not found</returns>
    Task<ScheduledMessageDto?> GetScheduledMessageAsync(string userId, string scheduledMessageId);

    /// <summary>
    /// Get scheduled messages for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="conversationId">Optional conversation filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="includeRecurring">Whether to include recurring messages</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of scheduled messages</returns>
    Task<List<ScheduledMessageDto>> GetUserScheduledMessagesAsync(string userId, string? conversationId = null, 
        ScheduledMessageStatus? status = null, bool includeRecurring = true, int skip = 0, int take = 50);

    /// <summary>
    /// Get scheduled messages for a conversation
    /// </summary>
    /// <param name="userId">User ID requesting messages</param>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of scheduled messages</returns>
    Task<List<ScheduledMessageDto>> GetConversationScheduledMessagesAsync(string userId, string conversationId, 
        ScheduledMessageStatus? status = null, int skip = 0, int take = 50);

    /// <summary>
    /// Process pending scheduled messages (called by background service)
    /// </summary>
    /// <returns>Number of messages processed</returns>
    Task<int> ProcessPendingMessagesAsync();

    /// <summary>
    /// Process recurring messages (called by background service)
    /// </summary>
    /// <returns>Number of recurring messages processed</returns>
    Task<int> ProcessRecurringMessagesAsync();

    /// <summary>
    /// Retry failed scheduled messages (called by background service)
    /// </summary>
    /// <returns>Number of messages retried</returns>
    Task<int> RetryFailedMessagesAsync();

    /// <summary>
    /// Clean up expired scheduled messages
    /// </summary>
    /// <param name="olderThanDays">Clean up messages older than specified days</param>
    /// <returns>Number of messages cleaned up</returns>
    Task<int> CleanupExpiredMessagesAsync(int olderThanDays = 30);

    /// <summary>
    /// Calculate next occurrence for a recurring message
    /// </summary>
    /// <param name="currentOccurrence">Current occurrence time</param>
    /// <param name="recurrencePattern">Recurrence pattern</param>
    /// <param name="timeZone">Timezone for calculation</param>
    /// <returns>Next occurrence time or null if no more occurrences</returns>
    Task<DateTimeOffset?> CalculateNextOccurrenceAsync(DateTimeOffset currentOccurrence, 
        RecurrencePatternDto recurrencePattern, string? timeZone = null);

    /// <summary>
    /// Validate recurrence pattern
    /// </summary>
    /// <param name="pattern">Recurrence pattern to validate</param>
    /// <returns>Validation result</returns>
    Task<RecurrenceValidationResult> ValidateRecurrencePatternAsync(RecurrencePatternDto pattern);

    /// <summary>
    /// Preview next occurrences for a recurring message
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="recurrencePattern">Recurrence pattern</param>
    /// <param name="timeZone">Timezone</param>
    /// <param name="count">Number of occurrences to preview</param>
    /// <returns>List of next occurrence dates</returns>
    Task<List<DateTimeOffset>> PreviewRecurrenceAsync(DateTimeOffset startDate, 
        RecurrencePatternDto recurrencePattern, string? timeZone = null, int count = 10);

    /// <summary>
    /// Get scheduled message statistics for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Scheduled message statistics</returns>
    Task<ScheduledMessageStatistics> GetScheduledMessageStatisticsAsync(string userId, 
        DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
}