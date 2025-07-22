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

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing messaging operations between users
/// </summary>
[ApiController]
[Route("api/v1/messaging")]
[Authorize]
public class MessagingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagingController> _logger;
    private readonly IMessagingService _messagingService;

    /// <summary>
    /// Initializes a new instance of MessagingController
    /// </summary>
    public MessagingController(ApplicationDbContext context, ILogger<MessagingController> logger, IMessagingService messagingService)
    {
        _context = context;
        _logger = logger;
        _messagingService = messagingService;
    }

    /// <summary>
    /// Send a message in a conversation
    /// </summary>
    /// <param name="request">Message details</param>
    /// <returns>The sent message</returns>
    [HttpPost("send")]
    [ProducesResponseType(typeof(MessageDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
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

            // Validate conversation exists and user is participant
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            // Check if user is participant in the conversation
            if (!conversation.Participants.Any(p => p.UserId == userId))
            {
                return Forbid("User is not a participant in this conversation");
            }

            // Create new message
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = request.ConversationId,
                SenderId = userId,
                MessageType = EnumConverter.ToString(request.MessageType),
                Content = request.Content?.Trim() ?? string.Empty,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(MessageStatus.Sent)
            };

            // Input validation
            if (string.IsNullOrWhiteSpace(message.Content) && request.MessageType == MessageType.Text)
            {
                return BadRequest("Text messages cannot be empty");
            }

            if (message.Content.Length > 4000)
            {
                return BadRequest("Message content cannot exceed 4000 characters");
            }

            _context.Messages.Add(message);

            // Update conversation last message info
            conversation.LastMessageId = message.Id;
            conversation.LastMessageAt = message.CreatedAt;
            conversation.LastMessagePreview = message.Content.Length > 200 
                ? message.Content.Substring(0, 200) + "..." 
                : message.Content;
            conversation.MessageCount++;
            conversation.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            // Get sender information for response
            var sender = await _context.Users.FindAsync(userId);
            
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
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt
            };

            _logger.LogInformation("Message {MessageId} sent successfully in conversation {ConversationId}", 
                message.Id, request.ConversationId);

            return CreatedAtAction(nameof(GetMessage), new { messageId = message.Id }, messageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId}", request.ConversationId);
            return StatusCode(500, "An error occurred while sending the message");
        }
    }

    /// <summary>
    /// Send an encrypted message in a conversation
    /// </summary>
    /// <param name="request">Encrypted message details</param>
    /// <returns>The sent encrypted message</returns>
    [HttpPost("send-encrypted")]
    [ProducesResponseType(typeof(MessageDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SendEncryptedMessage([FromBody] SendEncryptedMessageRequest request)
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

            // Validate message content
            if (string.IsNullOrWhiteSpace(request.Content) && request.MessageType == MessageType.Text)
            {
                return BadRequest("Text messages cannot be empty");
            }

            if (request.Content?.Length > 4000)
            {
                return BadRequest("Message content cannot exceed 4000 characters");
            }

            // Send encrypted message using the messaging service
            var messageDto = await _messagingService.SendEncryptedMessageAsync(
                request.ConversationId,
                userId,
                request.Content!,
                request.MessageType,
                request.ParentMessageId,
                request.UseEndToEndEncryption);

            _logger.LogInformation("Encrypted message {MessageId} sent successfully in conversation {ConversationId}", 
                messageDto.Id, request.ConversationId);

            return CreatedAtAction(nameof(GetMessage), new { messageId = messageDto.Id }, messageDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized access attempt to send encrypted message: {Message}", ex.Message);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid argument for encrypted message: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation for encrypted message: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending encrypted message in conversation {ConversationId}", request.ConversationId);
            return StatusCode(500, "An error occurred while sending the encrypted message");
        }
    }

    /// <summary>
    /// Get user's conversations
    /// </summary>
    /// <param name="type">Filter by conversation type (optional)</param>
    /// <param name="archived">Include archived conversations (default: false)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 50)</param>
    /// <returns>List of conversations</returns>
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(ConversationListResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetConversations(
        [FromQuery] ConversationType? type = null,
        [FromQuery] bool archived = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Min(Math.Max(1, pageSize), 50);

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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    ConversationType = EnumConverter.ToConversationType(c.ConversationType),
                    Title = c.Title ?? GetConversationTitle(c, userId),
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    LastMessageAt = c.LastMessageAt,
                    LastMessagePreview = c.LastMessagePreview,
                    MessageCount = c.MessageCount,
                    UnreadCount = c.Participants.First(p => p.UserId == userId).UnreadCount,
                    Participants = c.Participants.Select(p => new ParticipantDto
                    {
                        UserId = p.UserId,
                        UserName = p.User.UserName ?? "Unknown",
                        Role = EnumConverter.ToConversationRole(p.Role),
                        JoinedAt = p.JoinedAt,
                        LastReadAt = p.LastReadAt
                    }).ToList(),
                    IsArchived = c.Participants.First(p => p.UserId == userId).IsArchived,
                    IsPinned = c.Participants.First(p => p.UserId == userId).IsPinned,
                    IsMuted = c.Participants.First(p => p.UserId == userId).IsMuted
                })
                .ToListAsync();

            var response = new ConversationListResponse
            {
                Conversations = conversations,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasMore = totalCount > page * pageSize,
                UnreadTotal = conversations.Sum(c => c.UnreadCount)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while retrieving conversations");
        }
    }

    /// <summary>
    /// Get messages from a specific conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <returns>List of messages</returns>
    [HttpGet("conversations/{conversationId}/messages")]
    [ProducesResponseType(typeof(MessageListResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetConversationMessages(
        string conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate conversation exists and user is participant
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                return NotFound("Conversation not found or access denied");
            }

            // Validate pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Min(Math.Max(1, pageSize), 100);

            var totalCount = await _context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .CountAsync();

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName ?? "Unknown",
                    MessageType = EnumConverter.ToMessageType(m.MessageType),
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    Status = EnumConverter.ToMessageStatus(m.Status),
                    IsEdited = m.IsEdited,
                    EditedAt = m.EditedAt,
                    Attachments = m.Attachments.Select(a => new MessageAttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl,
                        ThumbnailUrl = a.ThumbnailUrl,
                        FileSize = a.FileSize,
                        MimeType = a.MimeType,
                        AttachmentType = EnumConverter.ToAttachmentType(a.AttachmentType)
                    }).ToList(),
                    Reactions = m.Reactions.GroupBy(r => r.Reaction)
                        .Select(g => new MessageReactionDto
                        {
                            Reaction = g.Key,
                            Count = g.Count(),
                            UserIds = g.Select(r => r.UserId).ToList()
                        }).ToList()
                })
                .ToListAsync();

            // Reverse to get chronological order (oldest first)
            messages.Reverse();

            var response = new MessageListResponse
            {
                Messages = messages,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasMore = totalCount > page * pageSize
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while retrieving messages");
        }
    }

    /// <summary>
    /// Get a specific message by ID
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>Message details</returns>
    [HttpGet("messages/{messageId}")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMessage(string messageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Where(m => m.Id == messageId && !m.IsDeleted)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound("Message not found");
            }

            // Check if user is participant in the conversation
            if (!message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return Forbid("Access denied");
            }

            var messageDto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName ?? "Unknown",
                MessageType = EnumConverter.ToMessageType(message.MessageType),
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                Status = EnumConverter.ToMessageStatus(message.Status),
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                Attachments = message.Attachments.Select(a => new MessageAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ThumbnailUrl = a.ThumbnailUrl,
                    FileSize = a.FileSize,
                    MimeType = a.MimeType,
                    AttachmentType = EnumConverter.ToAttachmentType(a.AttachmentType)
                }).ToList(),
                Reactions = message.Reactions.GroupBy(r => r.Reaction)
                    .Select(g => new MessageReactionDto
                    {
                        Reaction = g.Key,
                        Count = g.Count(),
                        UserIds = g.Select(r => r.UserId).ToList()
                    }).ToList()
            };

            return Ok(messageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message {MessageId}", messageId);
            return StatusCode(500, "An error occurred while retrieving the message");
        }
    }

    /// <summary>
    /// Edit a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="request">New message content</param>
    /// <returns>Updated message</returns>
    [HttpPut("messages/{messageId}")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> EditMessage(string messageId, [FromBody] EditMessageRequest request)
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

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                return NotFound("Message not found");
            }

            // Only sender can edit their message
            if (message.SenderId != userId)
            {
                return Forbid("Only the message sender can edit this message");
            }

            // Check if message is too old to edit (24 hours)
            if (message.CreatedAt < DateTimeOffset.UtcNow.AddHours(-24))
            {
                return BadRequest("Messages older than 24 hours cannot be edited");
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Message content cannot be empty");
            }

            if (request.Content?.Length > 4000)
            {
                return BadRequest("Message content cannot exceed 4000 characters");
            }

            // Update message
            message.Content = request.Content?.Trim() ?? string.Empty;
            message.IsEdited = true;
            message.EditedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var messageDto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName ?? "Unknown",
                MessageType = EnumConverter.ToMessageType(message.MessageType),
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                Status = EnumConverter.ToMessageStatus(message.Status),
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt
            };

            _logger.LogInformation("Message {MessageId} edited successfully", messageId);

            return Ok(messageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing message {MessageId}", messageId);
            return StatusCode(500, "An error occurred while editing the message");
        }
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("messages/{messageId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                return NotFound("Message not found");
            }

            // Only sender can delete their message
            if (message.SenderId != userId)
            {
                return Forbid("Only the message sender can delete this message");
            }

            // Soft delete the message
            message.IsDeleted = true;
            message.DeletedAt = DateTimeOffset.UtcNow;
            message.DeletedBy = userId;

            // Update conversation message count
            var conversation = message.Conversation;
            conversation.MessageCount = Math.Max(0, conversation.MessageCount - 1);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Message {MessageId} deleted successfully", messageId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
            return StatusCode(500, "An error occurred while deleting the message");
        }
    }

    /// <summary>
    /// Add a reaction to a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="request">Reaction details</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("messages/{messageId}/reactions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddMessageReaction(string messageId, [FromBody] AddReactionRequest request)
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

            // Check if message exists and user has access
            var hasAccess = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(m => m.Id == messageId && !m.IsDeleted)
                .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId))
                .AnyAsync();

            if (!hasAccess)
            {
                return NotFound("Message not found or access denied");
            }

            // Check if reaction already exists
            var existingReaction = await _context.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId && mr.Reaction == request.Reaction);

            if (existingReaction != null)
            {
                // Remove existing reaction (toggle behavior)
                _context.MessageReactions.Remove(existingReaction);
            }
            else
            {
                // Add new reaction
                var reaction = new MessageReaction
                {
                    MessageId = messageId,
                    UserId = userId,
                    Reaction = request.Reaction,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.MessageReactions.Add(reaction);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Reaction {Reaction} toggled for message {MessageId} by user {UserId}", 
                request.Reaction, messageId, userId);

            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reaction to message {MessageId}", messageId);
            return StatusCode(500, "An error occurred while adding the reaction");
        }
    }

    /// <summary>
    /// Get unread message count for a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <returns>Unread count</returns>
    [HttpGet("conversations/{conversationId}/unread-count")]
    [ProducesResponseType(typeof(UnreadCountResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUnreadCount(string conversationId)
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

            var response = new UnreadCountResponse
            {
                ConversationId = conversationId,
                UnreadCount = participant.UnreadCount
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for conversation {ConversationId}", conversationId);
            return StatusCode(500, "An error occurred while getting unread count");
        }
    }

    /// <summary>
    /// Search messages within conversations
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="conversationId">Optional conversation ID to limit search</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 50)</param>
    /// <returns>Search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(MessageSearchResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchMessages(
        [FromQuery] string query,
        [FromQuery] string? conversationId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                return BadRequest("Search query must be at least 3 characters long");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Min(Math.Max(1, pageSize), 50);

            var searchQuery = _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(m => !m.IsDeleted)
                .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId))
                .Where(m => m.Content.Contains(query));

            if (!string.IsNullOrEmpty(conversationId))
            {
                searchQuery = searchQuery.Where(m => m.ConversationId == conversationId);
            }

            var totalCount = await searchQuery.CountAsync();

            var results = await searchQuery
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MessageSearchResultDto
                {
                    MessageId = m.Id,
                    ConversationId = m.ConversationId,
                    ConversationTitle = m.Conversation.Title ?? "Conversation",
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName ?? "Unknown",
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    Snippet = GetSearchSnippet(m.Content, query)
                })
                .ToListAsync();

            var response = new MessageSearchResponse
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
            _logger.LogError(ex, "Error searching messages with query '{Query}'", query);
            return StatusCode(500, "An error occurred while searching messages");
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
                .FirstOrDefault(p => p.UserId != currentUserId);
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

    /// <summary>
    /// Helper method to create search snippet with highlighted query
    /// </summary>
    private static string GetSearchSnippet(string content, string query)
    {
        const int snippetLength = 150;
        
        if (content.Length <= snippetLength)
        {
            return content;
        }

        var queryIndex = content.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (queryIndex == -1)
        {
            return content.Substring(0, Math.Min(snippetLength, content.Length)) + "...";
        }

        var start = Math.Max(0, queryIndex - 50);
        var end = Math.Min(content.Length, start + snippetLength);
        
        var snippet = content.Substring(start, end - start);
        
        if (start > 0)
        {
            snippet = "..." + snippet;
        }
        if (end < content.Length)
        {
            snippet += "...";
        }

        return snippet;
    }
}