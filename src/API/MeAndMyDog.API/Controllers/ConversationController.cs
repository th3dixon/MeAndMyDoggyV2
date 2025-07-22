using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing conversations between users
/// </summary>
[ApiController]
[Route("api/v1/conversations")]
[Authorize]
public class ConversationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationController> _logger;

    /// <summary>
    /// Initializes a new instance of ConversationController
    /// </summary>
    public ConversationController(ApplicationDbContext context, ILogger<ConversationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    /// <param name="request">Conversation creation details</param>
    /// <returns>The created conversation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate participant IDs exist
            var validParticipants = await _context.Users
                .Where(u => request.ParticipantIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            if (validParticipants.Count != request.ParticipantIds.Count)
            {
                return BadRequest("One or more participant IDs are invalid");
            }

            // Add creator to participants if not already included
            if (!validParticipants.Contains(userId))
            {
                validParticipants.Add(userId);
            }

            // For direct conversations, ensure only 2 participants
            if (request.ConversationType == ConversationType.Direct && validParticipants.Count != 2)
            {
                return BadRequest("Direct conversations must have exactly 2 participants");
            }

            // Check if direct conversation already exists between these users
            if (request.ConversationType == ConversationType.Direct)
            {
                var existingConversation = await _context.Conversations
                    .Include(c => c.Participants)
                    .Where(c => c.ConversationType == EnumConverter.ToString(ConversationType.Direct))
                    .Where(c => c.Participants.Count == 2)
                    .Where(c => c.Participants.All(p => validParticipants.Contains(p.UserId)))
                    .FirstOrDefaultAsync();

                if (existingConversation != null)
                {
                    // Return existing conversation
                    var existingDto = await MapConversationToDto(existingConversation, userId);
                    return Ok(existingDto);
                }
            }

            // Create new conversation
            var conversation = new Conversation
            {
                Id = Guid.NewGuid().ToString(),
                ConversationType = EnumConverter.ToString(request.ConversationType),
                Title = request.Title?.Trim(),
                Description = request.Description?.Trim(),
                ImageUrl = request.ImageUrl?.Trim(),
                CreatedBy = userId,
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
                Role = EnumConverter.ToString(participantId == userId ? ConversationRole.Owner : ConversationRole.Member),
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

            var conversationDto = await MapConversationToDto(createdConversation, userId);

            _logger.LogInformation("Conversation {ConversationId} created successfully by user {UserId}", 
                conversation.Id, userId);

            return CreatedAtAction(nameof(GetConversation), new { conversationId = conversation.Id }, conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while creating the conversation");
        }
    }

    /// <summary>
    /// Get a specific conversation by ID
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <returns>Conversation details</returns>
    [HttpGet("{conversationId}")]
    [ProducesResponseType(typeof(ConversationDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversation(string conversationId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            var conversationDto = await MapConversationToDto(conversation, userId);

            return Ok(conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while retrieving the conversation");
        }
    }

    /// <summary>
    /// Update conversation details
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Update details</param>
    /// <returns>Updated conversation</returns>
    [HttpPut("{conversationId}")]
    [ProducesResponseType(typeof(ConversationDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateConversation(string conversationId, [FromBody] UpdateConversationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && (p.Role == EnumConverter.ToString(ConversationRole.Owner) || p.Role == EnumConverter.ToString(ConversationRole.Admin))))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound("Conversation not found or insufficient permissions");
            }

            // Update allowed fields
            if (!string.IsNullOrEmpty(request.Title))
            {
                conversation.Title = request.Title.Trim();
            }

            if (request.Description != null)
            {
                conversation.Description = string.IsNullOrEmpty(request.Description) ? null : request.Description.Trim();
            }

            if (request.ImageUrl != null)
            {
                conversation.ImageUrl = string.IsNullOrEmpty(request.ImageUrl) ? null : request.ImageUrl.Trim();
            }

            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var conversationDto = await MapConversationToDto(conversation, userId);

            _logger.LogInformation("Conversation {ConversationId} updated successfully by user {UserId}", 
                conversationId, userId);

            return Ok(conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while updating the conversation");
        }
    }

    /// <summary>
    /// Add a participant to a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Participant details</param>
    /// <returns>Updated conversation</returns>
    [HttpPost("{conversationId}/participants")]
    [ProducesResponseType(typeof(ConversationDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddParticipant(string conversationId, [FromBody] AddParticipantRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId && (p.Role == EnumConverter.ToString(ConversationRole.Owner) || p.Role == EnumConverter.ToString(ConversationRole.Admin))))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound("Conversation not found or insufficient permissions");
            }

            // Cannot add participants to direct conversations
            if (conversation.ConversationType == EnumConverter.ToString(ConversationType.Direct))
            {
                return BadRequest("Cannot add participants to direct conversations");
            }

            // Check if user to add exists
            var userToAdd = await _context.Users.FindAsync(request.UserId);
            if (userToAdd == null)
            {
                return BadRequest("User to add does not exist");
            }

            // Check if user is already a participant
            var existingParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == request.UserId);
            if (existingParticipant != null)
            {
                if (existingParticipant.LeftAt == null)
                {
                    return BadRequest("User is already a participant in this conversation");
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
                    UserId = request.UserId,
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
                request.UserId, conversationId, userId);

            return Ok(conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant to conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while adding the participant");
        }
    }

    /// <summary>
    /// Remove a participant from a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="participantUserId">User ID to remove</param>
    /// <returns>Updated conversation</returns>
    [HttpDelete("{conversationId}/participants/{participantUserId}")]
    [ProducesResponseType(typeof(ConversationDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveParticipant(string conversationId, string participantUserId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Where(c => c.Id == conversationId)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            // Cannot remove participants from direct conversations
            if (conversation.ConversationType == EnumConverter.ToString(ConversationType.Direct))
            {
                return BadRequest("Cannot remove participants from direct conversations");
            }

            var currentUserParticipant = conversation.Participants.First(p => p.UserId == userId);
            var participantToRemove = conversation.Participants.FirstOrDefault(p => p.UserId == participantUserId);

            if (participantToRemove == null || participantToRemove.LeftAt != null)
            {
                return NotFound("Participant not found in this conversation");
            }

            // Check permissions: owners and admins can remove members, users can remove themselves
            bool canRemove = currentUserParticipant.Role == EnumConverter.ToString(ConversationRole.Owner) ||
                           currentUserParticipant.Role == EnumConverter.ToString(ConversationRole.Admin) ||
                           userId == participantUserId;

            if (!canRemove)
            {
                return Forbid("Insufficient permissions to remove this participant");
            }

            // Cannot remove the owner (they must transfer ownership first)
            if (participantToRemove.Role == EnumConverter.ToString(ConversationRole.Owner) && userId != participantUserId)
            {
                return BadRequest("Cannot remove the conversation owner");
            }

            // Mark participant as left (soft removal)
            participantToRemove.LeftAt = DateTimeOffset.UtcNow;
            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var conversationDto = await MapConversationToDto(conversation, userId);

            _logger.LogInformation("User {ParticipantUserId} removed from conversation {ConversationId} by user {UserId}", 
                participantUserId, conversationId, userId);

            return Ok(conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant from conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while removing the participant");
        }
    }

    /// <summary>
    /// Search conversations by title or participant name
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="type">Filter by conversation type</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ConversationSearchResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchConversations(
        [FromQuery] string query,
        [FromQuery] ConversationType? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Search query must be at least 2 characters long");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Min(Math.Max(1, pageSize), 50);

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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            var response = new ConversationSearchResponse
            {
                Results = results,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasMore = totalCount > page * pageSize,
                Query = query
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations with query '{Query}'", query);
            return StatusCode(500, "An error occurred while searching conversations");
        }
    }

    /// <summary>
    /// Archive/unarchive a conversation for the current user
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Archive request details</param>
    /// <returns>Success confirmation</returns>
    [HttpPut("{conversationId}/archive")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetArchiveStatus(string conversationId, [FromBody] SetArchiveStatusRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            participant.IsArchived = request.IsArchived;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} archive status set to {IsArchived} for user {UserId}", 
                conversationId, request.IsArchived, userId);

            return Ok(new { Success = true, IsArchived = request.IsArchived });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting archive status for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while updating archive status");
        }
    }

    /// <summary>
    /// Pin/unpin a conversation for the current user
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Pin request details</param>
    /// <returns>Success confirmation</returns>
    [HttpPut("{conversationId}/pin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetPinStatus(string conversationId, [FromBody] SetPinStatusRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            participant.IsPinned = request.IsPinned;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} pin status set to {IsPinned} for user {UserId}", 
                conversationId, request.IsPinned, userId);

            return Ok(new { Success = true, IsPinned = request.IsPinned });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting pin status for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while updating pin status");
        }
    }

    /// <summary>
    /// Mute/unmute a conversation for the current user
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Mute request details</param>
    /// <returns>Success confirmation</returns>
    [HttpPut("{conversationId}/mute")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetMuteStatus(string conversationId, [FromBody] SetMuteStatusRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
            {
                return NotFound("Conversation not found or access denied");
            }

            participant.IsMuted = request.IsMuted;
            participant.MutedUntil = request.MutedUntil;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Conversation {ConversationId} mute status set to {IsMuted} for user {UserId}", 
                conversationId, request.IsMuted, userId);

            return Ok(new { Success = true, IsMuted = request.IsMuted, MutedUntil = request.MutedUntil });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting mute status for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while updating mute status");
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