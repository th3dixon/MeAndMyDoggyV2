using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for messaging operations
/// </summary>
public interface IMessagingService
{
    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    /// <param name="conversationId">Target conversation ID</param>
    /// <param name="senderId">User ID of sender</param>
    /// <param name="content">Message content</param>
    /// <param name="messageType">Type of message</param>
    /// <param name="parentMessageId">Optional parent message for replies</param>
    /// <param name="mentionedUserIds">Optional list of mentioned user IDs</param>
    /// <returns>The created message</returns>
    Task<MessageDto> SendMessageAsync(string conversationId, string senderId, string content, 
        MessageType messageType = MessageType.Text, string? parentMessageId = null, List<string>? mentionedUserIds = null);

    /// <summary>
    /// Send an encrypted message to a conversation
    /// </summary>
    /// <param name="conversationId">Target conversation ID</param>
    /// <param name="senderId">User ID of sender</param>
    /// <param name="content">Plain text message content to encrypt</param>
    /// <param name="messageType">Type of message</param>
    /// <param name="parentMessageId">Optional parent message for replies</param>
    /// <param name="useEndToEndEncryption">Whether to use end-to-end encryption</param>
    /// <returns>The created encrypted message</returns>
    Task<MessageDto> SendEncryptedMessageAsync(string conversationId, string senderId, string content, 
        MessageType messageType = MessageType.Text, string? parentMessageId = null, bool useEndToEndEncryption = true);

    /// <summary>
    /// Get messages from a conversation with pagination
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID requesting messages (for access control)</param>
    /// <param name="skip">Number of messages to skip</param>
    /// <param name="take">Number of messages to take</param>
    /// <returns>List of messages</returns>
    Task<List<MessageDto>> GetMessagesAsync(string conversationId, string userId, int skip = 0, int take = 50);

    /// <summary>
    /// Get a specific message by ID
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID requesting message (for access control)</param>
    /// <returns>Message details or null if not found/no access</returns>
    Task<MessageDto?> GetMessageAsync(string messageId, string userId);

    /// <summary>
    /// Update message content (edit message)
    /// </summary>
    /// <param name="messageId">Message ID to update</param>
    /// <param name="userId">User ID attempting to edit</param>
    /// <param name="newContent">New message content</param>
    /// <returns>Updated message or null if not found/no permission</returns>
    Task<MessageDto?> UpdateMessageAsync(string messageId, string userId, string newContent);

    /// <summary>
    /// Delete a message (soft delete)
    /// </summary>
    /// <param name="messageId">Message ID to delete</param>
    /// <param name="userId">User ID attempting to delete</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteMessageAsync(string messageId, string userId);

    /// <summary>
    /// Mark message as read by user
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID marking as read</param>
    /// <param name="deviceInfo">Optional device information</param>
    /// <returns>True if successfully marked as read</returns>
    Task<bool> MarkAsReadAsync(string messageId, string userId, string? deviceInfo = null);

    /// <summary>
    /// Add reaction to a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID adding reaction</param>
    /// <param name="reaction">Reaction emoji or identifier</param>
    /// <returns>True if reaction added/removed successfully</returns>
    Task<bool> ToggleReactionAsync(string messageId, string userId, string reaction);

    /// <summary>
    /// Get unread message count for user in a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>Number of unread messages</returns>
    Task<int> GetUnreadCountAsync(string conversationId, string userId);

    /// <summary>
    /// Search messages within conversations accessible to user
    /// </summary>
    /// <param name="userId">User ID performing search</param>
    /// <param name="query">Search query</param>
    /// <param name="conversationId">Optional conversation ID to limit search</param>
    /// <param name="skip">Number of results to skip</param>
    /// <param name="take">Number of results to take</param>
    /// <returns>Search results</returns>
    Task<MessageSearchResponse> SearchMessagesAsync(string userId, string query, string? conversationId = null, int skip = 0, int take = 20);

    /// <summary>
    /// Get message attachments for a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID requesting attachments (for access control)</param>
    /// <returns>List of message attachments</returns>
    Task<List<MessageAttachmentDto>> GetMessageAttachmentsAsync(string messageId, string userId);

    /// <summary>
    /// Add attachment to a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID adding attachment</param>
    /// <param name="attachment">Attachment details</param>
    /// <returns>Created attachment or null if failed</returns>
    Task<MessageAttachmentDto?> AddMessageAttachmentAsync(string messageId, string userId, MessageAttachmentDto attachment);

    /// <summary>
    /// Get message delivery status
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="userId">User ID requesting status (must be sender)</param>
    /// <returns>Message delivery status details</returns>
    Task<MessageDeliveryStatus?> GetMessageDeliveryStatusAsync(string messageId, string userId);
}