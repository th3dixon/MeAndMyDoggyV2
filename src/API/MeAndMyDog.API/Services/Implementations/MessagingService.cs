using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for messaging operations
/// </summary>
public class MessagingService : IMessagingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagingService> _logger;
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Initialize the messaging service
    /// </summary>
    public MessagingService(ApplicationDbContext context, ILogger<MessagingService> logger, IEncryptionService encryptionService)
    {
        _context = context;
        _logger = logger;
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public async Task<MessageDto> SendMessageAsync(string conversationId, string senderId, string content,
        MessageType messageType = MessageType.Text, string? parentMessageId = null, List<string>? mentionedUserIds = null)
    {
        try
        {
            // Validate conversation and user participation
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found", nameof(conversationId));
            }

            if (!conversation.Participants.Any(p => p.UserId == senderId))
            {
                throw new UnauthorizedAccessException("User is not a participant in this conversation");
            }

            // Validate message content
            if (string.IsNullOrWhiteSpace(content) && messageType == MessageType.Text)
            {
                throw new ArgumentException("Text message content cannot be empty", nameof(content));
            }

            if (content?.Length > 4000)
            {
                throw new ArgumentException("Message content cannot exceed 4000 characters", nameof(content));
            }

            // Validate parent message if provided
            if (!string.IsNullOrEmpty(parentMessageId))
            {
                var parentExists = await _context.Messages
                    .AnyAsync(m => m.Id == parentMessageId && m.ConversationId == conversationId && !m.IsDeleted);
                
                if (!parentExists)
                {
                    throw new ArgumentException("Parent message not found", nameof(parentMessageId));
                }
            }

            // Create message entity
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                SenderId = senderId,
                MessageType = EnumConverter.ToString(messageType),
                Content = content?.Trim() ?? string.Empty,
                ParentMessageId = parentMessageId,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = EnumConverter.ToString(MessageStatus.Sent),
                Mentions = mentionedUserIds?.Count > 0 ? JsonSerializer.Serialize(mentionedUserIds) : null
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
            var otherParticipants = conversation.Participants.Where(p => p.UserId != senderId).ToList();
            foreach (var participant in otherParticipants)
            {
                participant.UnreadCount++;
            }

            await _context.SaveChangesAsync();

            // Get sender information
            var sender = await _context.Users.FindAsync(senderId);

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

            _logger.LogInformation("Message {MessageId} sent successfully in conversation {ConversationId} by user {UserId}", 
                message.Id, conversationId, senderId);

            return messageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId} by user {UserId}", 
                conversationId, senderId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageDto> SendEncryptedMessageAsync(string conversationId, string senderId, string content, 
        MessageType messageType = MessageType.Text, string? parentMessageId = null, bool useEndToEndEncryption = true)
    {
        try
        {
            // Create encryption request
            var encryptionRequest = new EncryptMessageRequest
            {
                ConversationId = conversationId,
                PlainTextContent = content,
                MessageType = EnumConverter.ToString(messageType),
                ParentMessageId = parentMessageId,
                UseEndToEndEncryption = useEndToEndEncryption
            };

            // Encrypt the message using encryption service
            var encryptionResponse = await _encryptionService.EncryptMessageAsync(senderId, encryptionRequest);

            if (!encryptionResponse.Success)
            {
                throw new InvalidOperationException($"Failed to encrypt message: {encryptionResponse.Message}");
            }

            // Get the created message and return as DTO
            var message = await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == encryptionResponse.MessageId);

            if (message == null)
            {
                throw new InvalidOperationException("Encrypted message was created but could not be retrieved");
            }

            var messageDto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender?.UserName ?? "Unknown",
                MessageType = EnumConverter.ToMessageType(message.MessageType),
                Content = "🔒 Encrypted message", // Don't expose encrypted content
                CreatedAt = message.CreatedAt,
                Status = EnumConverter.ToMessageStatus(message.Status),
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsEncrypted = true // Add flag to indicate encryption
            };

            _logger.LogInformation("Encrypted message {MessageId} sent successfully in conversation {ConversationId} by user {UserId}", 
                message.Id, conversationId, senderId);

            return messageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending encrypted message in conversation {ConversationId} by user {UserId}", 
                conversationId, senderId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageDto>> GetMessagesAsync(string conversationId, string userId, int skip = 0, int take = 50)
    {
        try
        {
            // Validate user access to conversation
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("User does not have access to this conversation");
            }

            // Validate pagination parameters
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
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
                        AttachmentType = EnumConverter.ToAttachmentType(a.AttachmentType),
                        Width = a.Width,
                        Height = a.Height,
                        Duration = a.Duration
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

            _logger.LogDebug("Retrieved {Count} messages for conversation {ConversationId}", 
                messages.Count, conversationId);

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageDto?> GetMessageAsync(string messageId, string userId)
    {
        try
        {
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
                return null;
            }

            // Check if user has access to the conversation
            if (!message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                throw new UnauthorizedAccessException("User does not have access to this message");
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
                    AttachmentType = EnumConverter.ToAttachmentType(a.AttachmentType),
                    Width = a.Width,
                    Height = a.Height,
                    Duration = a.Duration
                }).ToList(),
                Reactions = message.Reactions.GroupBy(r => r.Reaction)
                    .Select(g => new MessageReactionDto
                    {
                        Reaction = g.Key,
                        Count = g.Count(),
                        UserIds = g.Select(r => r.UserId).ToList()
                    }).ToList()
            };

            return messageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageDto?> UpdateMessageAsync(string messageId, string userId, string newContent)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                return null;
            }

            // Only sender can edit their message
            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("Only the message sender can edit this message");
            }

            // Check if message is too old to edit (24 hours)
            if (message.CreatedAt < DateTimeOffset.UtcNow.AddHours(-24))
            {
                throw new InvalidOperationException("Messages older than 24 hours cannot be edited");
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(newContent))
            {
                throw new ArgumentException("Message content cannot be empty", nameof(newContent));
            }

            if (newContent.Length > 4000)
            {
                throw new ArgumentException("Message content cannot exceed 4000 characters", nameof(newContent));
            }

            // Store edit history if needed
            if (!message.IsEdited)
            {
                var editHistory = new List<object>
                {
                    new { Content = message.Content, EditedAt = message.CreatedAt }
                };
                message.EditHistory = JsonSerializer.Serialize(editHistory);
            }

            // Update message
            message.Content = newContent.Trim();
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

            _logger.LogInformation("Message {MessageId} edited successfully by user {UserId}", messageId, userId);

            return messageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteMessageAsync(string messageId, string userId)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                return false;
            }

            // Only sender can delete their message
            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("Only the message sender can delete this message");
            }

            // Soft delete the message
            message.IsDeleted = true;
            message.DeletedAt = DateTimeOffset.UtcNow;
            message.DeletedBy = userId;

            // Update conversation message count
            var conversation = message.Conversation;
            conversation.MessageCount = Math.Max(0, conversation.MessageCount - 1);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Message {MessageId} deleted successfully by user {UserId}", messageId, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsReadAsync(string messageId, string userId, string? deviceInfo = null)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

            if (message == null || !message.Conversation.Participants.Any(p => p.UserId == userId))
            {
                return false;
            }

            // Create read receipt if it doesn't exist
            var existingReceipt = await _context.MessageReadReceipts
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);

            if (existingReceipt == null)
            {
                var readReceipt = new MessageReadReceipt
                {
                    MessageId = messageId,
                    UserId = userId,
                    ReadAt = DateTimeOffset.UtcNow,
                    DeviceInfo = deviceInfo
                };

                _context.MessageReadReceipts.Add(readReceipt);

                // Update participant's read status
                var participant = await _context.ConversationParticipants
                    .FirstOrDefaultAsync(cp => cp.ConversationId == message.ConversationId && cp.UserId == userId);

                if (participant != null)
                {
                    participant.LastReadMessageId = messageId;
                    participant.LastReadAt = DateTimeOffset.UtcNow;
                    participant.UnreadCount = 0; // Reset unread count
                }

                await _context.SaveChangesAsync();

                _logger.LogDebug("Message {MessageId} marked as read by user {UserId}", messageId, userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message {MessageId} as read", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ToggleReactionAsync(string messageId, string userId, string reaction)
    {
        try
        {
            // Validate message exists and user has access
            var hasAccess = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(m => m.Id == messageId && !m.IsDeleted)
                .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId))
                .AnyAsync();

            if (!hasAccess)
            {
                return false;
            }

            // Check if reaction already exists
            var existingReaction = await _context.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId && mr.Reaction == reaction);

            if (existingReaction != null)
            {
                // Remove existing reaction (toggle behavior)
                _context.MessageReactions.Remove(existingReaction);
                _logger.LogDebug("Reaction {Reaction} removed from message {MessageId} by user {UserId}", 
                    reaction, messageId, userId);
            }
            else
            {
                // Add new reaction
                var newReaction = new MessageReaction
                {
                    MessageId = messageId,
                    UserId = userId,
                    Reaction = reaction,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.MessageReactions.Add(newReaction);
                _logger.LogDebug("Reaction {Reaction} added to message {MessageId} by user {UserId}", 
                    reaction, messageId, userId);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling reaction for message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountAsync(string conversationId, string userId)
    {
        try
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            return participant?.UnreadCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for conversation {ConversationId}", conversationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageSearchResponse> SearchMessagesAsync(string userId, string query, string? conversationId = null, int skip = 0, int take = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                throw new ArgumentException("Search query must be at least 3 characters long", nameof(query));
            }

            // Validate pagination parameters
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 50);

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
                .Skip(skip)
                .Take(take)
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

            return new MessageSearchResponse
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
            _logger.LogError(ex, "Error searching messages with query '{Query}'", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageAttachmentDto>> GetMessageAttachmentsAsync(string messageId, string userId)
    {
        try
        {
            // Validate user access
            var hasAccess = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(m => m.Id == messageId && !m.IsDeleted)
                .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId))
                .AnyAsync();

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("User does not have access to this message");
            }

            var attachments = await _context.MessageAttachments
                .Where(a => a.MessageId == messageId)
                .Select(a => new MessageAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ThumbnailUrl = a.ThumbnailUrl,
                    FileSize = a.FileSize,
                    MimeType = a.MimeType,
                    AttachmentType = EnumConverter.ToAttachmentType(a.AttachmentType),
                    Width = a.Width,
                    Height = a.Height,
                    Duration = a.Duration
                })
                .ToListAsync();

            return attachments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attachments for message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageAttachmentDto?> AddMessageAttachmentAsync(string messageId, string userId, MessageAttachmentDto attachment)
    {
        try
        {
            // Validate message exists and user is sender
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId && !m.IsDeleted);

            if (message == null)
            {
                return null;
            }

            var messageAttachment = new MessageAttachment
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = messageId,
                AttachmentType = EnumConverter.ToString(attachment.AttachmentType),
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                ThumbnailUrl = attachment.ThumbnailUrl,
                FileSize = attachment.FileSize,
                MimeType = attachment.MimeType,
                Width = attachment.Width,
                Height = attachment.Height,
                Duration = attachment.Duration,
                UploadedAt = DateTimeOffset.UtcNow
            };

            _context.MessageAttachments.Add(messageAttachment);
            await _context.SaveChangesAsync();

            var attachmentDto = new MessageAttachmentDto
            {
                Id = messageAttachment.Id,
                FileName = messageAttachment.FileName,
                FileUrl = messageAttachment.FileUrl,
                ThumbnailUrl = messageAttachment.ThumbnailUrl,
                FileSize = messageAttachment.FileSize,
                MimeType = messageAttachment.MimeType,
                AttachmentType = EnumConverter.ToAttachmentType(messageAttachment.AttachmentType),
                Width = messageAttachment.Width,
                Height = messageAttachment.Height,
                Duration = messageAttachment.Duration
            };

            _logger.LogInformation("Attachment {AttachmentId} added to message {MessageId}", 
                messageAttachment.Id, messageId);

            return attachmentDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding attachment to message {MessageId}", messageId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<MessageDeliveryStatus?> GetMessageDeliveryStatusAsync(string messageId, string userId)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Participants)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId && !m.IsDeleted);

            if (message == null)
            {
                return null;
            }

            var readReceipts = await _context.MessageReadReceipts
                .Include(r => r.User)
                .Where(r => r.MessageId == messageId)
                .ToListAsync();

            var recipients = message.Conversation.Participants
                .Where(p => p.UserId != userId)
                .Select(p => new RecipientStatus
                {
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "Unknown",
                    IsDelivered = true, // Assume delivered for now
                    IsRead = readReceipts.Any(r => r.UserId == p.UserId),
                    ReadAt = readReceipts.FirstOrDefault(r => r.UserId == p.UserId)?.ReadAt,
                    DeviceInfo = readReceipts.FirstOrDefault(r => r.UserId == p.UserId)?.DeviceInfo
                })
                .ToList();

            return new MessageDeliveryStatus
            {
                MessageId = messageId,
                Status = EnumConverter.ToMessageStatus(message.Status),
                SentAt = message.CreatedAt,
                Recipients = recipients
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery status for message {MessageId}", messageId);
            throw;
        }
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