using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for conversation management
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Create a new conversation
    /// </summary>
    /// <param name="creatorId">User ID creating the conversation</param>
    /// <param name="participantIds">List of participant user IDs</param>
    /// <param name="conversationType">Type of conversation</param>
    /// <param name="title">Optional conversation title</param>
    /// <param name="description">Optional conversation description</param>
    /// <param name="imageUrl">Optional conversation image URL</param>
    /// <returns>Created conversation</returns>
    Task<ConversationDto> CreateConversationAsync(string creatorId, List<string> participantIds, 
        ConversationType conversationType, string? title = null, string? description = null, string? imageUrl = null);

    /// <summary>
    /// Create a new conversation with friendship validation
    /// </summary>
    /// <param name="creatorId">User ID creating the conversation</param>
    /// <param name="participantIds">List of participant user IDs</param>
    /// <param name="conversationType">Type of conversation</param>
    /// <param name="isFromServiceContext">True if creating from service provider context</param>
    /// <param name="title">Optional conversation title</param>
    /// <param name="description">Optional conversation description</param>
    /// <param name="imageUrl">Optional conversation image URL</param>
    /// <returns>Created conversation</returns>
    Task<ConversationDto> CreateConversationWithValidationAsync(string creatorId, List<string> participantIds, 
        ConversationType conversationType, bool isFromServiceContext = false, string? title = null, string? description = null, string? imageUrl = null);

    /// <summary>
    /// Get a conversation by ID
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID requesting conversation (for access control)</param>
    /// <returns>Conversation details or null if not found/no access</returns>
    Task<ConversationDto?> GetConversationAsync(string conversationId, string userId);

    /// <summary>
    /// Get conversations for a user with filtering and pagination
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="type">Optional conversation type filter</param>
    /// <param name="archived">Include archived conversations</param>
    /// <param name="skip">Number of conversations to skip</param>
    /// <param name="take">Number of conversations to take</param>
    /// <returns>List of user's conversations</returns>
    Task<ConversationListResponse> GetUserConversationsAsync(string userId, ConversationType? type = null, 
        bool archived = false, int skip = 0, int take = 20);

    /// <summary>
    /// Update conversation details
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to update</param>
    /// <param name="title">New title (null to keep current)</param>
    /// <param name="description">New description (null to keep current)</param>
    /// <param name="imageUrl">New image URL (null to keep current)</param>
    /// <returns>Updated conversation or null if not found/no permission</returns>
    Task<ConversationDto?> UpdateConversationAsync(string conversationId, string userId, 
        string? title = null, string? description = null, string? imageUrl = null);

    /// <summary>
    /// Add participant to conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to add participant</param>
    /// <param name="newParticipantId">User ID to add as participant</param>
    /// <returns>Updated conversation or null if failed</returns>
    Task<ConversationDto?> AddParticipantAsync(string conversationId, string userId, string newParticipantId);

    /// <summary>
    /// Add participant to conversation with friendship validation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to add participant</param>
    /// <param name="newParticipantId">User ID to add as participant</param>
    /// <param name="isFromServiceContext">True if adding from service context</param>
    /// <returns>Updated conversation or null if failed</returns>
    Task<ConversationDto?> AddParticipantWithValidationAsync(string conversationId, string userId, string newParticipantId, bool isFromServiceContext = false);

    /// <summary>
    /// Remove participant from conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to remove participant</param>
    /// <param name="participantToRemoveId">User ID to remove</param>
    /// <returns>Updated conversation or null if failed</returns>
    Task<ConversationDto?> RemoveParticipantAsync(string conversationId, string userId, string participantToRemoveId);

    /// <summary>
    /// Update participant role in conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to update role</param>
    /// <param name="participantId">Participant user ID to update</param>
    /// <param name="newRole">New role for participant</param>
    /// <returns>True if role updated successfully</returns>
    Task<bool> UpdateParticipantRoleAsync(string conversationId, string userId, string participantId, ConversationRole newRole);

    /// <summary>
    /// Set archive status for conversation (per user)
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="isArchived">Archive status</param>
    /// <returns>True if status updated successfully</returns>
    Task<bool> SetArchiveStatusAsync(string conversationId, string userId, bool isArchived);

    /// <summary>
    /// Set pin status for conversation (per user)
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="isPinned">Pin status</param>
    /// <returns>True if status updated successfully</returns>
    Task<bool> SetPinStatusAsync(string conversationId, string userId, bool isPinned);

    /// <summary>
    /// Set mute status for conversation (per user)
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="isMuted">Mute status</param>
    /// <param name="mutedUntil">Optional time when mute should be lifted</param>
    /// <returns>True if status updated successfully</returns>
    Task<bool> SetMuteStatusAsync(string conversationId, string userId, bool isMuted, DateTimeOffset? mutedUntil = null);

    /// <summary>
    /// Search conversations by title or participant name
    /// </summary>
    /// <param name="userId">User ID performing search</param>
    /// <param name="query">Search query</param>
    /// <param name="type">Optional conversation type filter</param>
    /// <param name="skip">Number of results to skip</param>
    /// <param name="take">Number of results to take</param>
    /// <returns>Search results</returns>
    Task<ConversationSearchResponse> SearchConversationsAsync(string userId, string query, ConversationType? type = null, 
        int skip = 0, int take = 20);

    /// <summary>
    /// Get conversation participants
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID requesting participants (for access control)</param>
    /// <returns>List of participants</returns>
    Task<List<ParticipantDto>> GetConversationParticipantsAsync(string conversationId, string userId);

    /// <summary>
    /// Check if user has permission to perform action on conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="requiredRole">Minimum required role</param>
    /// <returns>True if user has permission</returns>
    Task<bool> HasPermissionAsync(string conversationId, string userId, ConversationRole requiredRole = ConversationRole.Member);

    /// <summary>
    /// Get or create direct conversation between two users
    /// </summary>
    /// <param name="userId1">First user ID</param>
    /// <param name="userId2">Second user ID</param>
    /// <returns>Existing or newly created direct conversation</returns>
    Task<ConversationDto> GetOrCreateDirectConversationAsync(string userId1, string userId2);

    /// <summary>
    /// Get or create direct conversation between two users with friendship validation
    /// </summary>
    /// <param name="userId1">First user ID</param>
    /// <param name="userId2">Second user ID</param>
    /// <param name="isFromServiceContext">True if from service provider context</param>
    /// <returns>Existing or newly created direct conversation</returns>
    Task<ConversationDto> GetOrCreateDirectConversationWithValidationAsync(string userId1, string userId2, bool isFromServiceContext = false);

    /// <summary>
    /// Delete conversation (only for owners)
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID attempting to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteConversationAsync(string conversationId, string userId);

    /// <summary>
    /// Get conversation statistics
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID requesting statistics (for access control)</param>
    /// <returns>Conversation statistics</returns>
    Task<ConversationStatistics?> GetConversationStatisticsAsync(string conversationId, string userId);
}