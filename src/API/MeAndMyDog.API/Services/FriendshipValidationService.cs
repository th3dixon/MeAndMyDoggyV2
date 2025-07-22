using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Services;

/// <summary>
/// Service for validating friendship relationships in messaging contexts
/// </summary>
public class FriendshipValidationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FriendshipValidationService> _logger;

    /// <summary>
    /// Initializes a new instance of the FriendshipValidationService
    /// </summary>
    public FriendshipValidationService(ApplicationDbContext context, ILogger<FriendshipValidationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Check if two users are friends (accepted friendship status)
    /// </summary>
    /// <param name="userId1">First user ID</param>
    /// <param name="userId2">Second user ID</param>
    /// <returns>True if users are friends</returns>
    public async Task<bool> AreFriendsAsync(string userId1, string userId2)
    {
        if (userId1 == userId2) return true; // User can always message themselves

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => 
                ((f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                 (f.RequesterId == userId2 && f.ReceiverId == userId1)) &&
                f.Status == FriendshipStatus.Accepted);

        return friendship != null;
    }

    /// <summary>
    /// Check if a user is a service provider
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <returns>True if user is a service provider</returns>
    public async Task<bool> IsServiceProviderAsync(string userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.UserType == UserType.ServiceProvider || user?.UserType == UserType.Both;
    }

    /// <summary>
    /// Validate if users can be added to a conversation based on friendship rules
    /// Rules:
    /// 1. Service providers can be messaged by anyone (from search/booking context)
    /// 2. Regular users can only be added if they are friends with the person adding them
    /// 3. Group conversations require all participants to be friends with each other
    /// </summary>
    /// <param name="initiatingUserId">User who is creating conversation or adding participants</param>
    /// <param name="targetUserIds">Users to be added to conversation</param>
    /// <param name="existingParticipantIds">Existing conversation participants (for group validation)</param>
    /// <param name="isFromServiceContext">True if conversation initiated from service provider search/booking</param>
    /// <returns>Validation result with details</returns>
    public async Task<ConversationValidationResult> ValidateConversationParticipantsAsync(
        string initiatingUserId, 
        List<string> targetUserIds, 
        List<string> existingParticipantIds = null, 
        bool isFromServiceContext = false)
    {
        try
        {
            existingParticipantIds ??= new List<string>();
            var result = new ConversationValidationResult { IsValid = true };

            // Check each target user
            foreach (var targetUserId in targetUserIds)
            {
                // Skip self
                if (targetUserId == initiatingUserId) continue;

                var targetUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == targetUserId);

                if (targetUser == null)
                {
                    result.IsValid = false;
                    result.Errors.Add($"User not found: {targetUserId}");
                    continue;
                }

                // Rule 1: Service providers can be messaged by anyone from service context
                if (isFromServiceContext && (targetUser.UserType == UserType.ServiceProvider || targetUser.UserType == UserType.Both))
                {
                    result.ValidatedUsers.Add(targetUserId);
                    continue;
                }

                // Rule 2: For regular users, check friendship with initiating user
                if (!await AreFriendsAsync(initiatingUserId, targetUserId))
                {
                    result.IsValid = false;
                    result.Errors.Add($"You must be friends with {targetUser.DisplayName} to add them to a conversation. Use their friend code to send a friend request first.");
                    continue;
                }

                // Rule 3: For group conversations, ensure new user is friends with all existing participants
                if (existingParticipantIds.Any())
                {
                    var friendshipChecks = new List<Task<bool>>();
                    foreach (var existingParticipantId in existingParticipantIds)
                    {
                        if (existingParticipantId != initiatingUserId && existingParticipantId != targetUserId)
                        {
                            friendshipChecks.Add(AreFriendsAsync(targetUserId, existingParticipantId));
                        }
                    }

                    if (friendshipChecks.Any())
                    {
                        var friendshipResults = await Task.WhenAll(friendshipChecks);
                        if (!friendshipResults.All(r => r))
                        {
                            result.IsValid = false;
                            result.Errors.Add($"{targetUser.DisplayName} must be friends with all existing group members to be added.");
                            continue;
                        }
                    }
                }

                result.ValidatedUsers.Add(targetUserId);
            }

            if (result.IsValid)
            {
                _logger.LogInformation("Conversation validation passed for user {InitiatingUserId} adding {TargetCount} participants", 
                    initiatingUserId, targetUserIds.Count);
            }
            else
            {
                _logger.LogWarning("Conversation validation failed for user {InitiatingUserId}: {Errors}", 
                    initiatingUserId, string.Join("; ", result.Errors));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating conversation participants for user {InitiatingUserId}", initiatingUserId);
            return new ConversationValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "An error occurred while validating conversation participants" }
            };
        }
    }

    /// <summary>
    /// Check if a user can create a direct conversation with another user
    /// </summary>
    /// <param name="userId1">First user ID</param>
    /// <param name="userId2">Second user ID</param>
    /// <param name="isFromServiceContext">True if from service provider context</param>
    /// <returns>True if conversation can be created</returns>
    public async Task<bool> CanCreateDirectConversationAsync(string userId1, string userId2, bool isFromServiceContext = false)
    {
        var validation = await ValidateConversationParticipantsAsync(
            userId1, 
            new List<string> { userId2 }, 
            null, 
            isFromServiceContext);
        
        return validation.IsValid;
    }

    /// <summary>
    /// Get friendship status between two users
    /// </summary>
    /// <param name="userId1">First user ID</param>
    /// <param name="userId2">Second user ID</param>
    /// <returns>Friendship status</returns>
    public async Task<FriendshipStatusInfo> GetFriendshipStatusAsync(string userId1, string userId2)
    {
        if (userId1 == userId2) return new FriendshipStatusInfo { Status = "Self", CanMessage = true };

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => 
                (f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                (f.RequesterId == userId2 && f.ReceiverId == userId1));

        if (friendship == null)
        {
            return new FriendshipStatusInfo { Status = "None", CanMessage = false };
        }

        return new FriendshipStatusInfo
        {
            Status = friendship.Status,
            CanMessage = friendship.IsAccepted,
            IsSentByUser = friendship.RequesterId == userId1,
            FriendshipId = friendship.Id
        };
    }
}

/// <summary>
/// Result of conversation participant validation
/// </summary>
public class ConversationValidationResult
{
    /// <summary>
    /// Whether validation passed
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of error messages if validation failed
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of user IDs that passed validation
    /// </summary>
    public List<string> ValidatedUsers { get; set; } = new();
}

/// <summary>
/// Friendship status information between two users
/// </summary>
public class FriendshipStatusInfo
{
    /// <summary>
    /// Current friendship status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether users can message each other
    /// </summary>
    public bool CanMessage { get; set; }

    /// <summary>
    /// Whether the friendship request was sent by the first user
    /// </summary>
    public bool IsSentByUser { get; set; }

    /// <summary>
    /// Friendship ID if exists
    /// </summary>
    public Guid? FriendshipId { get; set; }
}