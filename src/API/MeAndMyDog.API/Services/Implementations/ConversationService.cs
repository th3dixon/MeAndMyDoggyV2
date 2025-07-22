using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for conversation management
/// </summary>
public class ConversationService : IConversationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationService> _logger;
    private readonly FriendshipValidationService _friendshipValidation;

    /// <summary>
    /// Initialize the conversation service
    /// </summary>
    public ConversationService(
        ApplicationDbContext context, 
        ILogger<ConversationService> logger,
        FriendshipValidationService friendshipValidation)
    {
        _context = context;
        _logger = logger;
        _friendshipValidation = friendshipValidation;
    }

    /// <inheritdoc />
    public async Task<ConversationDto> CreateConversationAsync(string creatorId, List<string> participantIds, 
        ConversationType conversationType, string? title = null, string? description = null, string? imageUrl = null)
    {
        try
        {
            // Validate participant IDs exist
            var validParticipants = await _context.Users
                .Where(u => participantIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            if (validParticipants.Count != participantIds.Count)
            {
                throw new ArgumentException("One or more participant IDs are invalid");
            }

            // Add creator to participants if not already included
            if (!validParticipants.Contains(creatorId))
            {
                validParticipants.Add(creatorId);
            }

            // For direct conversations, ensure only 2 participants
            if (conversationType == ConversationType.Direct && validParticipants.Count != 2)
            {
                throw new ArgumentException("Direct conversations must have exactly 2 participants");
            }

            // Check if direct conversation already exists between these users
            if (conversationType == ConversationType.Direct)
            {
                var existingConversation = await _context.Conversations
                    .Include(c => c.Participants)
                        .ThenInclude(p => p.User)
                    .Where(c => c.ConversationType == EnumConverter.ToString(ConversationType.Direct))
                    .Where(c => c.Participants.Count == 2)
                    .Where(c => c.Participants.All(p => validParticipants.Contains(p.UserId)))
                    .FirstOrDefaultAsync();

                if (existingConversation != null)
                {
                    // Return existing conversation
                    return await MapConversationToDto(existingConversation, creatorId);
                }
            }

            // Create new conversation
            var conversation = new Conversation
            {
                Id = Guid.NewGuid().ToString(),
                ConversationType = EnumConverter.ToString(conversationType),
                Title = title?.Trim(),
                Description = description?.Trim(),
                ImageUrl = imageUrl?.Trim(),
                CreatedBy = creatorId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                MessageCount = 0
            };

            _context.Conversations.Add(conversation);

            // Add participants
            var participants = validParticipants.Select(participantId => new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = participantId,
                Role = participantId == creatorId ? EnumConverter.ToString(ConversationRole.Owner) : EnumConverter.ToString(ConversationRole.Member),
                JoinedAt = DateTimeOffset.UtcNow,
                UnreadCount = 0,
                IsArchived = false,
                IsPinned = false,
                IsMuted = false
            }).ToList();

            _context.ConversationParticipants.AddRange(participants);
            await _context.SaveChangesAsync();

            // Load full conversation with participants for response
            var createdConversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .FirstAsync(c => c.Id == conversation.Id);

            var conversationDto = await MapConversationToDto(createdConversation, creatorId);

            _logger.LogInformation("Conversation {ConversationId} created successfully by user {UserId}", 
                conversation.Id, creatorId);

            return conversationDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation for user {UserId}", creatorId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto?> GetConversationAsync(string conversationId, string userId)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return null;
            }

            return await MapConversationToDto(conversation, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationListResponse> GetUserConversationsAsync(string userId, ConversationType? type = null, 
        bool archived = false, int skip = 0, int take = 20)
    {
        try
        {
            // Validate pagination parameters
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 50);

            var query = _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .Where(c => c.Participants.First(p => p.UserId == userId).IsArchived == archived);

            if (type.HasValue)
            {
                query = query.Where(c => c.ConversationType == EnumConverter.ToString(type.Value));
            }

            var totalCount = await query.CountAsync();

            var conversations = await query
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var conversationDtos = new List<ConversationDto>();
            foreach (var conversation in conversations)
            {
                conversationDtos.Add(await MapConversationToDto(conversation, userId));
            }

            var response = new ConversationListResponse
            {
                Conversations = conversationDtos,
                TotalCount = totalCount,
                Page = (skip / take) + 1,
                PageSize = take,
                HasMore = totalCount > skip + take,
                UnreadTotal = conversationDtos.Sum(c => c.UnreadCount)
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto?> UpdateConversationAsync(string conversationId, string userId, 
        string? title = null, string? description = null, string? imageUrl = null)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && 
                    (p.Role == EnumConverter.ToString(ConversationRole.Owner) || p.Role == EnumConverter.ToString(ConversationRole.Admin))))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return null;
            }

            // Update allowed fields
            if (!string.IsNullOrEmpty(title))
            {
                conversation.Title = title.Trim();
            }

            if (description != null)
            {
                conversation.Description = string.IsNullOrEmpty(description) ? null : description.Trim();
            }

            if (imageUrl != null)
            {
                conversation.ImageUrl = string.IsNullOrEmpty(imageUrl) ? null : imageUrl.Trim();
            }

            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var conversationDto = await MapConversationToDto(conversation, userId);

            _logger.LogInformation("Conversation {ConversationId} updated successfully by user {UserId}", 
                conversationId, userId);

            return conversationDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto?> AddParticipantAsync(string conversationId, string userId, string newParticipantId)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && 
                    (p.Role == EnumConverter.ToString(ConversationRole.Owner) || p.Role == EnumConverter.ToString(ConversationRole.Admin))))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return null;
            }

            // Cannot add participants to direct conversations
            if (conversation.ConversationType == EnumConverter.ToString(ConversationType.Direct))
            {
                throw new InvalidOperationException("Cannot add participants to direct conversations");
            }

            // Check if user to add exists
            var userToAdd = await _context.Users.FindAsync(newParticipantId);
            if (userToAdd == null)
            {
                throw new ArgumentException("User to add does not exist");
            }

            // Check if user is already a participant
            var existingParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == newParticipantId);
            if (existingParticipant != null)
            {
                if (existingParticipant.LeftAt == null)
                {
                    throw new InvalidOperationException("User is already a participant in this conversation");
                }
                
                // Re-add user who previously left
                existingParticipant.LeftAt = null;
                existingParticipant.JoinedAt = DateTimeOffset.UtcNow;
                existingParticipant.Role = EnumConverter.ToString(ConversationRole.Member);
                existingParticipant.UnreadCount = 0;
            }
            else
            {
                // Add new participant
                var newParticipant = new ConversationParticipant
                {
                    ConversationId = conversationId,
                    UserId = newParticipantId,
                    Role = EnumConverter.ToString(ConversationRole.Member),
                    JoinedAt = DateTimeOffset.UtcNow,
                    UnreadCount = 0,
                    IsArchived = false,
                    IsPinned = false,
                    IsMuted = false
                };

                _context.ConversationParticipants.Add(newParticipant);
                conversation.Participants.Add(newParticipant);
            }

            conversation.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            var conversationDto = await MapConversationToDto(conversation, userId);

            _logger.LogInformation("User {NewUserId} added to conversation {ConversationId} by user {UserId}", 
                newParticipantId, conversationId, userId);

            return conversationDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant to conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto?> RemoveParticipantAsync(string conversationId, string userId, string participantToRemoveId)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return null;
            }

            // Cannot remove participants from direct conversations
            if (conversation.ConversationType == EnumConverter.ToString(ConversationType.Direct))
            {
                throw new InvalidOperationException("Cannot remove participants from direct conversations");
            }

            var currentUserParticipant = conversation.Participants.First(p => p.UserId == userId);
            var participantToRemove = conversation.Participants.FirstOrDefault(p => p.UserId == participantToRemoveId);

            if (participantToRemove == null || participantToRemove.LeftAt != null)
            {
                return null;
            }

            // Check permissions: owners and admins can remove members, users can remove themselves
            bool canRemove = currentUserParticipant.Role == EnumConverter.ToString(ConversationRole.Owner) ||
                           currentUserParticipant.Role == EnumConverter.ToString(ConversationRole.Admin) ||
                           userId == participantToRemoveId;

            if (!canRemove)
            {
                throw new UnauthorizedAccessException("Insufficient permissions to remove this participant");
            }

            // Cannot remove the owner (they must transfer ownership first)
            if (participantToRemove.Role == EnumConverter.ToString(ConversationRole.Owner) && userId != participantToRemoveId)
            {
                throw new InvalidOperationException("Cannot remove the conversation owner");
            }

            // Mark participant as left (soft removal)
            participantToRemove.LeftAt = DateTimeOffset.UtcNow;
            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var conversationDto = await MapConversationToDto(conversation, userId);

            _logger.LogInformation("User {ParticipantUserId} removed from conversation {ConversationId} by user {UserId}", 
                participantToRemoveId, conversationId, userId);

            return conversationDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant from conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateParticipantRoleAsync(string conversationId, string userId, string participantId, ConversationRole newRole)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.Role == EnumConverter.ToString(ConversationRole.Owner)))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return false;
            }

            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == participantId && p.LeftAt == null);
            if (participant == null)
            {
                return false;
            }

            // Cannot change owner role unless transferring ownership
            if (participant.Role == EnumConverter.ToString(ConversationRole.Owner))
            {
                throw new InvalidOperationException("Use transfer ownership method to change owner role");
            }

            participant.Role = EnumConverter.ToString(newRole);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {ParticipantId} role updated to {NewRole} in conversation {ConversationId}", 
                participantId, newRole, conversationId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant role in conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetArchiveStatusAsync(string conversationId, string userId, bool isArchived)
    {
        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.IsArchived = isArchived;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} archive status set to {IsArchived} for user {UserId}", 
                conversationId, isArchived, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting archive status for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetPinStatusAsync(string conversationId, string userId, bool isPinned)
    {
        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.IsPinned = isPinned;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} pin status set to {IsPinned} for user {UserId}", 
                conversationId, isPinned, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting pin status for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetMuteStatusAsync(string conversationId, string userId, bool isMuted, DateTimeOffset? mutedUntil = null)
    {
        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.IsMuted = isMuted;
            participant.MutedUntil = mutedUntil;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} mute status set to {IsMuted} for user {UserId}", 
                conversationId, isMuted, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting mute status for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationSearchResponse> SearchConversationsAsync(string userId, string query, ConversationType? type = null, 
        int skip = 0, int take = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                throw new ArgumentException("Search query must be at least 2 characters long");
            }

            // Validate pagination parameters
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 50);

            var searchQuery = _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.LeftAt == null))
                .Where(c => c.Title!.Contains(query) || 
                           c.Participants.Any(p => p.User.UserName!.Contains(query)));

            if (type.HasValue)
            {
                searchQuery = searchQuery.Where(c => c.ConversationType == EnumConverter.ToString(type.Value));
            }

            var totalCount = await searchQuery.CountAsync();

            var results = await searchQuery
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(c => new ConversationSearchResultDto
                {
                    ConversationId = c.Id,
                    Title = c.Title ?? GetConversationTitle(c, userId),
                    ConversationType = EnumConverter.ToConversationType(c.ConversationType),
                    LastMessageAt = c.LastMessageAt,
                    ParticipantCount = c.Participants.Count(p => p.LeftAt == null),
                    UnreadCount = c.Participants.First(p => p.UserId == userId).UnreadCount
                })
                .ToListAsync();

            return new ConversationSearchResponse
            {
                Results = results,
                TotalCount = totalCount,
                Page = (skip / take) + 1,
                PageSize = take,
                HasMore = totalCount > skip + take,
                Query = query
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations with query '{Query}'", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<ParticipantDto>> GetConversationParticipantsAsync(string conversationId, string userId)
    {
        try
        {
            // Verify user has access to the conversation
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("User does not have access to this conversation");
            }

            var participants = await _context.ConversationParticipants
                .Include(p => p.User)
                .Where(p => p.ConversationId == conversationId && p.LeftAt == null)
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "Unknown",
                    Role = EnumConverter.ToConversationRole(p.Role),
                    JoinedAt = p.JoinedAt,
                    LastReadAt = p.LastReadAt
                })
                .ToListAsync();

            return participants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting participants for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(string conversationId, string userId, ConversationRole requiredRole = ConversationRole.Member)
    {
        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId && cp.LeftAt == null);

            if (participant == null)
            {
                return false;
            }

            // Check role hierarchy: Owner > Admin > Member
            var requiredRoleString = EnumConverter.ToString(requiredRole);
            var adminRoleString = EnumConverter.ToString(ConversationRole.Admin);
            var ownerRoleString = EnumConverter.ToString(ConversationRole.Owner);
            var memberRoleString = EnumConverter.ToString(ConversationRole.Member);
            
            return requiredRoleString switch
            {
                var role when role == memberRoleString => true, // Any participant can perform member actions
                var role when role == adminRoleString => participant.Role == adminRoleString || participant.Role == ownerRoleString,
                var role when role == ownerRoleString => participant.Role == ownerRoleString,
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permissions for conversation {ConversationId}", conversationId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto> GetOrCreateDirectConversationAsync(string userId1, string userId2)
    {
        try
        {
            // Check if direct conversation already exists between these users
            var existingConversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.ConversationType == EnumConverter.ToString(ConversationType.Direct))
                .Where(c => c.Participants.Count == 2)
                .Where(c => c.Participants.Any(p => p.UserId == userId1) && 
                           c.Participants.Any(p => p.UserId == userId2))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return await MapConversationToDto(existingConversation, userId1);
            }

            // Create new direct conversation
            return await CreateConversationAsync(userId1, new List<string> { userId2 }, ConversationType.Direct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or creating direct conversation between users {UserId1} and {UserId2}", 
                userId1, userId2);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteConversationAsync(string conversationId, string userId)
    {
        try
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.Role == EnumConverter.ToString(ConversationRole.Owner)))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return false;
            }

            // For now, we'll do a soft delete by marking all participants as left
            // In the future, we might want to add a IsDeleted flag to Conversation
            foreach (var participant in conversation.Participants)
            {
                participant.LeftAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} deleted by user {UserId}", conversationId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationStatistics?> GetConversationStatisticsAsync(string conversationId, string userId)
    {
        try
        {
            // Verify user has access
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return null;
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                return null;
            }

            var messages = conversation.Messages.Where(m => !m.IsDeleted).ToList();
            var activeParticipants = conversation.Participants.Where(p => p.LeftAt == null).ToList();

            // Calculate statistics
            var thirtyDaysAgo = DateTimeOffset.UtcNow.AddDays(-30);
            var recentMessages = messages.Where(m => m.CreatedAt >= thirtyDaysAgo).Count();
            var averageMessagesPerDay = recentMessages / 30.0;

            // Find most active participant
            var participantActivity = messages
                .GroupBy(m => m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    MessageCount = g.Count(),
                    LastActivity = g.Max(m => m.CreatedAt)
                })
                .OrderByDescending(p => p.MessageCount)
                .FirstOrDefault();

            ParticipantActivity? mostActiveParticipant = null;
            if (participantActivity != null)
            {
                var participant = activeParticipants.FirstOrDefault(p => p.UserId == participantActivity.UserId);
                if (participant != null)
                {
                    mostActiveParticipant = new ParticipantActivity
                    {
                        UserId = participant.UserId,
                        UserName = participant.User.UserName ?? "Unknown",
                        MessageCount = participantActivity.MessageCount,
                        MessagePercentage = (double)participantActivity.MessageCount / messages.Count * 100,
                        LastActivityAt = participantActivity.LastActivity
                    };
                }
            }

            // Message type breakdown
            var messageTypeBreakdown = messages
                .GroupBy(m => m.MessageType)
                .ToDictionary(g => EnumConverter.ToMessageType(g.Key), g => g.Count());

            return new ConversationStatistics
            {
                ConversationId = conversationId,
                TotalMessages = messages.Count,
                ActiveParticipants = activeParticipants.Count,
                CreatedAt = conversation.CreatedAt,
                LastMessageAt = conversation.LastMessageAt,
                AverageMessagesPerDay = averageMessagesPerDay,
                MostActiveParticipant = mostActiveParticipant,
                MessageTypeBreakdown = messageTypeBreakdown
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <summary>
    /// Helper method to map conversation entity to DTO
    /// </summary>
    private async Task<ConversationDto> MapConversationToDto(Conversation conversation, string currentUserId)
    {
        var currentParticipant = conversation.Participants.First(p => p.UserId == currentUserId);

        return new ConversationDto
        {
            Id = conversation.Id,
            ConversationType = EnumConverter.ToConversationType(conversation.ConversationType),
            Title = conversation.Title ?? GetConversationTitle(conversation, currentUserId),
            Description = conversation.Description,
            ImageUrl = conversation.ImageUrl,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            LastMessagePreview = conversation.LastMessagePreview,
            MessageCount = conversation.MessageCount,
            UnreadCount = currentParticipant.UnreadCount,
            Participants = conversation.Participants
                .Where(p => p.LeftAt == null)
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "Unknown",
                    Role = EnumConverter.ToConversationRole(p.Role),
                    JoinedAt = p.JoinedAt,
                    LastReadAt = p.LastReadAt
                }).ToList(),
            IsArchived = currentParticipant.IsArchived,
            IsPinned = currentParticipant.IsPinned,
            IsMuted = currentParticipant.IsMuted
        };
    }

    /// <inheritdoc />
    public async Task<ConversationDto> CreateConversationWithValidationAsync(string creatorId, List<string> participantIds, 
        ConversationType conversationType, bool isFromServiceContext = false, string? title = null, string? description = null, string? imageUrl = null)
    {
        try
        {
            _logger.LogInformation("Creating conversation with validation for user {CreatorId}, participants: {ParticipantIds}, serviceContext: {IsFromServiceContext}", 
                creatorId, string.Join(",", participantIds), isFromServiceContext);

            // Validate friendship relationships
            var validation = await _friendshipValidation.ValidateConversationParticipantsAsync(
                creatorId, participantIds, null, isFromServiceContext);

            if (!validation.IsValid)
            {
                var errorMessage = string.Join("; ", validation.Errors);
                _logger.LogWarning("Conversation creation failed validation: {Errors}", errorMessage);
                throw new UnauthorizedAccessException($"Cannot create conversation: {errorMessage}");
            }

            // Proceed with conversation creation using validated participants
            return await CreateConversationAsync(creatorId, validation.ValidatedUsers, conversationType, title, description, imageUrl);
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation with validation for user {CreatorId}", creatorId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto?> AddParticipantWithValidationAsync(string conversationId, string userId, string newParticipantId, bool isFromServiceContext = false)
    {
        try
        {
            _logger.LogInformation("Adding participant {NewParticipantId} to conversation {ConversationId} by user {UserId}, serviceContext: {IsFromServiceContext}", 
                newParticipantId, conversationId, userId, isFromServiceContext);

            // Get existing participants
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                _logger.LogWarning("Conversation {ConversationId} not found", conversationId);
                return null;
            }

            var existingParticipantIds = conversation.Participants
                .Where(p => p.LeftAt == null)
                .Select(p => p.UserId)
                .ToList();

            // Validate friendship relationships
            var validation = await _friendshipValidation.ValidateConversationParticipantsAsync(
                userId, new List<string> { newParticipantId }, existingParticipantIds, isFromServiceContext);

            if (!validation.IsValid)
            {
                var errorMessage = string.Join("; ", validation.Errors);
                _logger.LogWarning("Add participant failed validation: {Errors}", errorMessage);
                throw new UnauthorizedAccessException($"Cannot add participant: {errorMessage}");
            }

            // Proceed with adding participant
            return await AddParticipantAsync(conversationId, userId, newParticipantId);
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant with validation to conversation {ConversationId}", conversationId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ConversationDto> GetOrCreateDirectConversationWithValidationAsync(string userId1, string userId2, bool isFromServiceContext = false)
    {
        try
        {
            _logger.LogInformation("Getting/creating direct conversation between {UserId1} and {UserId2}, serviceContext: {IsFromServiceContext}", 
                userId1, userId2, isFromServiceContext);

            // Check if direct conversation already exists
            var existingConversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => 
                    c.ConversationType == EnumConverter.ToString(ConversationType.Direct) &&
                    c.Participants.Count(p => p.LeftAt == null) == 2 &&
                    c.Participants.Any(p => p.UserId == userId1 && p.LeftAt == null) &&
                    c.Participants.Any(p => p.UserId == userId2 && p.LeftAt == null));

            if (existingConversation != null)
            {
                return await MapConversationToDto(existingConversation, userId1);
            }

            // Validate friendship for new conversation
            var validation = await _friendshipValidation.ValidateConversationParticipantsAsync(
                userId1, new List<string> { userId2 }, null, isFromServiceContext);

            if (!validation.IsValid)
            {
                var errorMessage = string.Join("; ", validation.Errors);
                _logger.LogWarning("Direct conversation creation failed validation: {Errors}", errorMessage);
                throw new UnauthorizedAccessException($"Cannot create direct conversation: {errorMessage}");
            }

            // Create new direct conversation
            return await CreateConversationAsync(userId1, new List<string> { userId2 }, ConversationType.Direct);
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting/creating direct conversation with validation between {UserId1} and {UserId2}", userId1, userId2);
            throw;
        }
    }

    /// <summary>
    /// Helper method to get conversation title for display
    /// </summary>
    private static string GetConversationTitle(Conversation conversation, string currentUserId)
    {
        if (!string.IsNullOrEmpty(conversation.Title))
        {
            return conversation.Title;
        }

        // For direct conversations, use the other participant's name
        if (conversation.ConversationType == EnumConverter.ToString(ConversationType.Direct))
        {
            var otherParticipant = conversation.Participants
                .FirstOrDefault(p => p.UserId != currentUserId && p.LeftAt == null);
            return otherParticipant?.User?.UserName ?? "Direct Message";
        }

        var conversationType = EnumConverter.ToConversationType(conversation.ConversationType);
        return conversationType switch
        {
            ConversationType.Group => "Group Chat",
            ConversationType.Booking => "Service Booking",
            ConversationType.System => "System Messages",
            _ => "Conversation"
        };
    }
}