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
/// Service implementation for scheduled message operations
/// </summary>
public class ScheduledMessageService : IScheduledMessageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ScheduledMessageService> _logger;
    private readonly IMessageTemplateService _templateService;
    private readonly IMessagingService _messagingService;

    /// <summary>
    /// Initialize the scheduled message service
    /// </summary>
    public ScheduledMessageService(
        ApplicationDbContext context,
        ILogger<ScheduledMessageService> logger,
        IMessageTemplateService templateService,
        IMessagingService messagingService)
    {
        _context = context;
        _logger = logger;
        _templateService = templateService;
        _messagingService = messagingService;
    }

    /// <inheritdoc />
    public async Task<ScheduledMessageResponse> ScheduleMessageAsync(string userId, ScheduleMessageRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrEmpty(request.TemplateId) && string.IsNullOrEmpty(request.Content))
            {
                return new ScheduledMessageResponse
                {
                    Success = false,
                    Message = "Either template ID or content must be provided"
                };
            }

            if (request.ScheduledAt <= DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return new ScheduledMessageResponse
                {
                    Success = false,
                    Message = "Scheduled time must be at least 1 minute in the future"
                };
            }

            // Validate conversation access
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == request.ConversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return new ScheduledMessageResponse
                {
                    Success = false,
                    Message = "Access denied to conversation"
                };
            }

            string resolvedContent = request.Content ?? string.Empty;
            string? templateContent = null;
            
            // Process template if provided
            if (!string.IsNullOrEmpty(request.TemplateId))
            {
                var template = await _templateService.GetTemplateAsync(userId, request.TemplateId);
                if (template == null)
                {
                    return new ScheduledMessageResponse
                    {
                        Success = false,
                        Message = "Template not found"
                    };
                }

                templateContent = template.Content;
                resolvedContent = await _templateService.ProcessTemplateAsync(
                    template.Content, request.TemplateVariables ?? new Dictionary<string, string>());
            }

            // Validate recurrence pattern if provided
            if (request.RecurrencePattern != null)
            {
                var recurrenceValidation = await ValidateRecurrencePatternAsync(request.RecurrencePattern);
                if (!recurrenceValidation.IsValid)
                {
                    return new ScheduledMessageResponse
                    {
                        Success = false,
                        Message = $"Invalid recurrence pattern: {string.Join(", ", recurrenceValidation.Errors)}"
                    };
                }
            }

            // Calculate next occurrence for recurring messages
            DateTimeOffset? nextOccurrence = null;
            if (request.RecurrencePattern != null)
            {
                nextOccurrence = await CalculateNextOccurrenceAsync(
                    request.ScheduledAt, request.RecurrencePattern, request.TimeZone);
            }

            // Create scheduled message
            var scheduledMessage = new ScheduledMessage
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = userId,
                ConversationId = request.ConversationId,
                TemplateId = request.TemplateId,
                MessageType = EnumConverter.ToString(
                    Enum.TryParse<MessageType>(request.MessageType, true, out var msgType) 
                    ? msgType : MessageType.Text),
                Content = resolvedContent,
                TemplateContent = templateContent,
                TemplateVariables = request.TemplateVariables?.Any() == true 
                    ? JsonSerializer.Serialize(request.TemplateVariables) : null,
                ScheduledAt = request.ScheduledAt,
                TimeZone = request.TimeZone,
                RecurrencePattern = request.RecurrencePattern != null 
                    ? JsonSerializer.Serialize(request.RecurrencePattern) : null,
                IsRecurring = request.RecurrencePattern != null,
                NextOccurrence = nextOccurrence,
                RecurrenceEndDate = request.RecurrencePattern?.EndDate,
                MaxOccurrences = request.RecurrencePattern?.MaxOccurrences,
                OccurrenceCount = 0,
                Status = EnumConverter.ToString(ScheduledMessageStatus.Pending),
                CreatedAt = DateTimeOffset.UtcNow,
                AttemptCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.ScheduledMessages.Add(scheduledMessage);
            await _context.SaveChangesAsync();

            var scheduledMessageDto = await MapToScheduledMessageDtoAsync(scheduledMessage);

            _logger.LogInformation("Message scheduled successfully: {ScheduledMessageId} for {ScheduledAt}", 
                scheduledMessage.Id, request.ScheduledAt);

            return new ScheduledMessageResponse
            {
                Success = true,
                Message = "Message scheduled successfully",
                ScheduledMessageId = scheduledMessage.Id,
                ScheduledMessage = scheduledMessageDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling message for user {UserId}", userId);
            return new ScheduledMessageResponse
            {
                Success = false,
                Message = "An error occurred while scheduling the message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<ScheduledMessageResponse> UpdateScheduledMessageAsync(string userId, string scheduledMessageId, UpdateScheduledMessageRequest request)
    {
        try
        {
            var scheduledMessage = await _context.ScheduledMessages
                .FirstOrDefaultAsync(sm => sm.Id == scheduledMessageId && sm.SenderId == userId);

            if (scheduledMessage == null)
            {
                return new ScheduledMessageResponse
                {
                    Success = false,
                    Message = "Scheduled message not found"
                };
            }

            var currentStatus = EnumConverter.ToScheduledMessageStatus(scheduledMessage.Status);
            if (currentStatus != ScheduledMessageStatus.Pending)
            {
                return new ScheduledMessageResponse
                {
                    Success = false,
                    Message = "Only pending messages can be updated"
                };
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Content))
            {
                scheduledMessage.Content = request.Content;
                scheduledMessage.TemplateContent = null; // Clear template reference if content is directly provided
                scheduledMessage.TemplateVariables = null;
                scheduledMessage.TemplateId = null;
            }

            if (request.ScheduledAt.HasValue)
            {
                if (request.ScheduledAt.Value <= DateTimeOffset.UtcNow.AddMinutes(1))
                {
                    return new ScheduledMessageResponse
                    {
                        Success = false,
                        Message = "Scheduled time must be at least 1 minute in the future"
                    };
                }
                scheduledMessage.ScheduledAt = request.ScheduledAt.Value;
            }

            if (!string.IsNullOrEmpty(request.TimeZone))
            {
                scheduledMessage.TimeZone = request.TimeZone;
            }

            if (request.RecurrencePattern != null)
            {
                var recurrenceValidation = await ValidateRecurrencePatternAsync(request.RecurrencePattern);
                if (!recurrenceValidation.IsValid)
                {
                    return new ScheduledMessageResponse
                    {
                        Success = false,
                        Message = $"Invalid recurrence pattern: {string.Join(", ", recurrenceValidation.Errors)}"
                    };
                }

                scheduledMessage.RecurrencePattern = JsonSerializer.Serialize(request.RecurrencePattern);
                scheduledMessage.IsRecurring = true;
                scheduledMessage.NextOccurrence = await CalculateNextOccurrenceAsync(
                    scheduledMessage.ScheduledAt, request.RecurrencePattern, scheduledMessage.TimeZone);
                scheduledMessage.RecurrenceEndDate = request.RecurrencePattern.EndDate;
                scheduledMessage.MaxOccurrences = request.RecurrencePattern.MaxOccurrences;
            }

            scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            var scheduledMessageDto = await MapToScheduledMessageDtoAsync(scheduledMessage);

            _logger.LogInformation("Scheduled message {ScheduledMessageId} updated successfully", scheduledMessageId);

            return new ScheduledMessageResponse
            {
                Success = true,
                Message = "Scheduled message updated successfully",
                ScheduledMessageId = scheduledMessageId,
                ScheduledMessage = scheduledMessageDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating scheduled message {ScheduledMessageId}", scheduledMessageId);
            return new ScheduledMessageResponse
            {
                Success = false,
                Message = "An error occurred while updating the scheduled message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> CancelScheduledMessageAsync(string userId, string scheduledMessageId)
    {
        try
        {
            var scheduledMessage = await _context.ScheduledMessages
                .FirstOrDefaultAsync(sm => sm.Id == scheduledMessageId && sm.SenderId == userId);

            if (scheduledMessage == null)
            {
                return false;
            }

            var currentStatus = EnumConverter.ToScheduledMessageStatus(scheduledMessage.Status);
            if (currentStatus != ScheduledMessageStatus.Pending && currentStatus != ScheduledMessageStatus.Paused)
            {
                return false;
            }

            scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Cancelled);
            scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled message {ScheduledMessageId} cancelled", scheduledMessageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling scheduled message {ScheduledMessageId}", scheduledMessageId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> PauseScheduledMessageAsync(string userId, string scheduledMessageId)
    {
        try
        {
            var scheduledMessage = await _context.ScheduledMessages
                .FirstOrDefaultAsync(sm => sm.Id == scheduledMessageId && sm.SenderId == userId);

            if (scheduledMessage == null)
            {
                return false;
            }

            var currentStatus = EnumConverter.ToScheduledMessageStatus(scheduledMessage.Status);
            if (currentStatus != ScheduledMessageStatus.Pending || !scheduledMessage.IsRecurring)
            {
                return false;
            }

            scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Paused);
            scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled message {ScheduledMessageId} paused", scheduledMessageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing scheduled message {ScheduledMessageId}", scheduledMessageId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ResumeScheduledMessageAsync(string userId, string scheduledMessageId)
    {
        try
        {
            var scheduledMessage = await _context.ScheduledMessages
                .FirstOrDefaultAsync(sm => sm.Id == scheduledMessageId && sm.SenderId == userId);

            if (scheduledMessage == null)
            {
                return false;
            }

            var currentStatus = EnumConverter.ToScheduledMessageStatus(scheduledMessage.Status);
            if (currentStatus != ScheduledMessageStatus.Paused)
            {
                return false;
            }

            scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Pending);
            scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled message {ScheduledMessageId} resumed", scheduledMessageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming scheduled message {ScheduledMessageId}", scheduledMessageId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<ScheduledMessageDto?> GetScheduledMessageAsync(string userId, string scheduledMessageId)
    {
        try
        {
            var scheduledMessage = await _context.ScheduledMessages
                .Include(sm => sm.Template)
                .Include(sm => sm.Conversation)
                .Include(sm => sm.Sender)
                .FirstOrDefaultAsync(sm => sm.Id == scheduledMessageId && sm.SenderId == userId);

            return scheduledMessage != null ? await MapToScheduledMessageDtoAsync(scheduledMessage) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled message {ScheduledMessageId}", scheduledMessageId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<ScheduledMessageDto>> GetUserScheduledMessagesAsync(string userId, string? conversationId = null, 
        ScheduledMessageStatus? status = null, bool includeRecurring = true, int skip = 0, int take = 50)
    {
        try
        {
            var query = _context.ScheduledMessages
                .Include(sm => sm.Template)
                .Include(sm => sm.Conversation)
                .Include(sm => sm.Sender)
                .Where(sm => sm.SenderId == userId);

            if (!string.IsNullOrEmpty(conversationId))
            {
                query = query.Where(sm => sm.ConversationId == conversationId);
            }

            if (status.HasValue)
            {
                var statusString = EnumConverter.ToString(status.Value);
                query = query.Where(sm => sm.Status == statusString);
            }

            if (!includeRecurring)
            {
                query = query.Where(sm => !sm.IsRecurring);
            }

            var scheduledMessages = await query
                .OrderByDescending(sm => sm.ScheduledAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var result = new List<ScheduledMessageDto>();
            foreach (var sm in scheduledMessages)
            {
                result.Add(await MapToScheduledMessageDtoAsync(sm));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled messages for user {UserId}", userId);
            return new List<ScheduledMessageDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<ScheduledMessageDto>> GetConversationScheduledMessagesAsync(string userId, string conversationId, 
        ScheduledMessageStatus? status = null, int skip = 0, int take = 50)
    {
        try
        {
            // Validate user access to conversation
            var hasAccess = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!hasAccess)
            {
                return new List<ScheduledMessageDto>();
            }

            var query = _context.ScheduledMessages
                .Include(sm => sm.Template)
                .Include(sm => sm.Conversation)
                .Include(sm => sm.Sender)
                .Where(sm => sm.ConversationId == conversationId);

            if (status.HasValue)
            {
                var statusString = EnumConverter.ToString(status.Value);
                query = query.Where(sm => sm.Status == statusString);
            }

            var scheduledMessages = await query
                .OrderByDescending(sm => sm.ScheduledAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var result = new List<ScheduledMessageDto>();
            foreach (var sm in scheduledMessages)
            {
                result.Add(await MapToScheduledMessageDtoAsync(sm));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled messages for conversation {ConversationId}", conversationId);
            return new List<ScheduledMessageDto>();
        }
    }

    /// <inheritdoc />
    public async Task<int> ProcessPendingMessagesAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var pendingMessages = await _context.ScheduledMessages
                .Include(sm => sm.Template)
                .Where(sm => sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Pending) &&
                           sm.ScheduledAt <= now)
                .ToListAsync();

            var processedCount = 0;
            foreach (var scheduledMessage in pendingMessages)
            {
                try
                {
                    await ProcessScheduledMessageAsync(scheduledMessage);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing scheduled message {ScheduledMessageId}", scheduledMessage.Id);
                    
                    // Mark as failed and set retry
                    scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Failed);
                    scheduledMessage.ErrorMessage = ex.Message;
                    scheduledMessage.AttemptCount++;
                    scheduledMessage.NextRetryAt = scheduledMessage.AttemptCount < 3 
                        ? DateTimeOffset.UtcNow.AddMinutes(Math.Pow(2, scheduledMessage.AttemptCount) * 5)
                        : null;
                    scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Processed {Count} pending scheduled messages", processedCount);
            }

            return processedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pending scheduled messages");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<int> ProcessRecurringMessagesAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var recurringMessages = await _context.ScheduledMessages
                .Where(sm => sm.IsRecurring &&
                           sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Sent) &&
                           sm.NextOccurrence.HasValue &&
                           sm.NextOccurrence <= now)
                .ToListAsync();

            var processedCount = 0;
            foreach (var scheduledMessage in recurringMessages)
            {
                try
                {
                    await ProcessRecurringMessageAsync(scheduledMessage);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing recurring message {ScheduledMessageId}", scheduledMessage.Id);
                }
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Processed {Count} recurring scheduled messages", processedCount);
            }

            return processedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing recurring scheduled messages");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<int> RetryFailedMessagesAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var failedMessages = await _context.ScheduledMessages
                .Where(sm => sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Failed) &&
                           sm.NextRetryAt.HasValue &&
                           sm.NextRetryAt <= now &&
                           sm.AttemptCount < 3)
                .ToListAsync();

            var retriedCount = 0;
            foreach (var scheduledMessage in failedMessages)
            {
                try
                {
                    scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Pending);
                    await ProcessScheduledMessageAsync(scheduledMessage);
                    retriedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrying scheduled message {ScheduledMessageId}", scheduledMessage.Id);
                    
                    scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Failed);
                    scheduledMessage.ErrorMessage = ex.Message;
                    scheduledMessage.AttemptCount++;
                    scheduledMessage.NextRetryAt = scheduledMessage.AttemptCount < 3 
                        ? DateTimeOffset.UtcNow.AddMinutes(Math.Pow(2, scheduledMessage.AttemptCount) * 5)
                        : null;
                }
            }

            if (retriedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Retried {Count} failed scheduled messages", retriedCount);
            }

            return retriedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying failed scheduled messages");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredMessagesAsync(int olderThanDays = 30)
    {
        try
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddDays(-olderThanDays);
            var expiredMessages = await _context.ScheduledMessages
                .Where(sm => sm.CreatedAt < cutoffDate &&
                           (sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Sent) ||
                            sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Cancelled) ||
                            sm.Status == EnumConverter.ToString(ScheduledMessageStatus.Expired)) &&
                           !sm.IsRecurring)
                .ToListAsync();

            if (expiredMessages.Any())
            {
                _context.ScheduledMessages.RemoveRange(expiredMessages);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} expired scheduled messages", expiredMessages.Count);
                return expiredMessages.Count;
            }

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired scheduled messages");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<DateTimeOffset?> CalculateNextOccurrenceAsync(DateTimeOffset currentOccurrence, 
        RecurrencePatternDto recurrencePattern, string? timeZone = null)
    {
        try
        {
            var recurrenceType = EnumConverter.ToRecurrenceType(recurrencePattern.Type);
            var interval = Math.Max(1, recurrencePattern.Interval);

            DateTimeOffset nextOccurrence = recurrenceType switch
            {
                RecurrenceType.Daily => currentOccurrence.AddDays(interval),
                RecurrenceType.Weekly => CalculateWeeklyOccurrence(currentOccurrence, recurrencePattern),
                RecurrenceType.Monthly => CalculateMonthlyOccurrence(currentOccurrence, recurrencePattern),
                RecurrenceType.Yearly => CalculateYearlyOccurrence(currentOccurrence, recurrencePattern),
                _ => currentOccurrence.AddDays(interval)
            };

            // Check if we've exceeded the end date
            if (recurrencePattern.EndDate.HasValue && nextOccurrence > recurrencePattern.EndDate.Value)
            {
                return null;
            }

            return nextOccurrence;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating next occurrence");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<RecurrenceValidationResult> ValidateRecurrencePatternAsync(RecurrencePatternDto pattern)
    {
        var result = new RecurrenceValidationResult();

        try
        {
            var recurrenceType = EnumConverter.ToRecurrenceType(pattern.Type);
            if (recurrenceType == RecurrenceType.None)
            {
                result.Errors.Add("Invalid recurrence type");
                return result;
            }

            if (pattern.Interval < 1)
            {
                result.Errors.Add("Interval must be at least 1");
            }

            if (pattern.MaxOccurrences.HasValue && pattern.MaxOccurrences < 1)
            {
                result.Errors.Add("Max occurrences must be at least 1");
            }

            if (pattern.EndDate.HasValue && pattern.EndDate < DateTimeOffset.UtcNow)
            {
                result.Errors.Add("End date must be in the future");
            }

            // Validate specific patterns
            switch (recurrenceType)
            {
                case RecurrenceType.Weekly:
                    if (pattern.DaysOfWeek?.Any() != true)
                    {
                        result.Errors.Add("Weekly recurrence requires at least one day of week");
                    }
                    break;

                case RecurrenceType.Monthly:
                    if (pattern.DayOfMonth.HasValue && (pattern.DayOfMonth < 1 || pattern.DayOfMonth > 31))
                    {
                        result.Errors.Add("Day of month must be between 1 and 31");
                    }
                    break;

                case RecurrenceType.Yearly:
                    if (pattern.Month.HasValue && (pattern.Month < 1 || pattern.Month > 12))
                    {
                        result.Errors.Add("Month must be between 1 and 12");
                    }
                    break;
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating recurrence pattern");
            result.Errors.Add("An error occurred during validation");
            result.IsValid = false;
            return result;
        }
    }

    /// <inheritdoc />
    public async Task<List<DateTimeOffset>> PreviewRecurrenceAsync(DateTimeOffset startDate, 
        RecurrencePatternDto recurrencePattern, string? timeZone = null, int count = 10)
    {
        var occurrences = new List<DateTimeOffset> { startDate };
        var currentDate = startDate;

        for (int i = 0; i < count - 1; i++)
        {
            var nextOccurrence = await CalculateNextOccurrenceAsync(currentDate, recurrencePattern, timeZone);
            if (nextOccurrence == null)
                break;

            occurrences.Add(nextOccurrence.Value);
            currentDate = nextOccurrence.Value;
        }

        return occurrences;
    }

    /// <inheritdoc />
    public async Task<ScheduledMessageStatistics> GetScheduledMessageStatisticsAsync(string userId, 
        DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            var messages = await _context.ScheduledMessages
                .Where(sm => sm.SenderId == userId &&
                           sm.CreatedAt >= fromDate &&
                           sm.CreatedAt <= toDate)
                .ToListAsync();

            var stats = new ScheduledMessageStatistics
            {
                TotalScheduled = messages.Count,
                TotalSent = messages.Count(m => m.Status == EnumConverter.ToString(ScheduledMessageStatus.Sent)),
                TotalPending = messages.Count(m => m.Status == EnumConverter.ToString(ScheduledMessageStatus.Pending)),
                TotalFailed = messages.Count(m => m.Status == EnumConverter.ToString(ScheduledMessageStatus.Failed)),
                TotalCancelled = messages.Count(m => m.Status == EnumConverter.ToString(ScheduledMessageStatus.Cancelled)),
                TotalRecurring = messages.Count(m => m.IsRecurring)
            };

            stats.SuccessRate = stats.TotalScheduled > 0 
                ? (decimal)stats.TotalSent / stats.TotalScheduled * 100 
                : 0;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled message statistics for user {UserId}", userId);
            return new ScheduledMessageStatistics();
        }
    }

    /// <summary>
    /// Process a scheduled message by sending it
    /// </summary>
    private async Task ProcessScheduledMessageAsync(ScheduledMessage scheduledMessage)
    {
        scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Processing);
        scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;

        try
        {
            var messageType = EnumConverter.ToMessageType(scheduledMessage.MessageType);
            var messageDto = await _messagingService.SendMessageAsync(
                scheduledMessage.ConversationId,
                scheduledMessage.SenderId,
                scheduledMessage.Content,
                messageType);

            scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Sent);
            scheduledMessage.SentAt = DateTimeOffset.UtcNow;
            scheduledMessage.SentMessageId = messageDto.Id;
            scheduledMessage.OccurrenceCount++;

            // Update template usage if applicable
            if (!string.IsNullOrEmpty(scheduledMessage.TemplateId) && _templateService is MessageTemplateService templateService)
            {
                await templateService.UpdateTemplateUsageAsync(scheduledMessage.TemplateId);
            }

            // Calculate next occurrence for recurring messages
            if (scheduledMessage.IsRecurring)
            {
                var recurrencePattern = JsonSerializer.Deserialize<RecurrencePatternDto>(scheduledMessage.RecurrencePattern!);
                scheduledMessage.NextOccurrence = await CalculateNextOccurrenceAsync(
                    scheduledMessage.ScheduledAt, recurrencePattern!, scheduledMessage.TimeZone);
            }
        }
        catch (Exception ex)
        {
            scheduledMessage.Status = EnumConverter.ToString(ScheduledMessageStatus.Failed);
            scheduledMessage.ErrorMessage = ex.Message;
            scheduledMessage.AttemptCount++;
            scheduledMessage.NextRetryAt = scheduledMessage.AttemptCount < 3 
                ? DateTimeOffset.UtcNow.AddMinutes(Math.Pow(2, scheduledMessage.AttemptCount) * 5)
                : null;
            throw;
        }
        finally
        {
            scheduledMessage.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>
    /// Process the next occurrence of a recurring message
    /// </summary>
    private async Task ProcessRecurringMessageAsync(ScheduledMessage recurringMessage)
    {
        if (!recurringMessage.NextOccurrence.HasValue)
            return;

        // Check if we should continue (max occurrences)
        if (recurringMessage.MaxOccurrences.HasValue &&
            recurringMessage.OccurrenceCount >= recurringMessage.MaxOccurrences.Value)
        {
            return;
        }

        // Create new occurrence
        var newOccurrence = new ScheduledMessage
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = recurringMessage.SenderId,
            ConversationId = recurringMessage.ConversationId,
            TemplateId = recurringMessage.TemplateId,
            MessageType = recurringMessage.MessageType,
            Content = recurringMessage.Content,
            TemplateContent = recurringMessage.TemplateContent,
            TemplateVariables = recurringMessage.TemplateVariables,
            ScheduledAt = recurringMessage.NextOccurrence.Value,
            TimeZone = recurringMessage.TimeZone,
            RecurrencePattern = recurringMessage.RecurrencePattern,
            IsRecurring = false, // Individual occurrences are not recurring
            Status = EnumConverter.ToString(ScheduledMessageStatus.Pending),
            CreatedAt = DateTimeOffset.UtcNow,
            AttemptCount = 0,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.ScheduledMessages.Add(newOccurrence);

        // Calculate next occurrence for the recurring message
        var recurrencePattern = JsonSerializer.Deserialize<RecurrencePatternDto>(recurringMessage.RecurrencePattern!);
        recurringMessage.NextOccurrence = await CalculateNextOccurrenceAsync(
            recurringMessage.NextOccurrence.Value, recurrencePattern!, recurringMessage.TimeZone);
        
        recurringMessage.UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Calculate weekly recurrence
    /// </summary>
    private static DateTimeOffset CalculateWeeklyOccurrence(DateTimeOffset current, RecurrencePatternDto pattern)
    {
        if (pattern.DaysOfWeek?.Any() != true)
        {
            return current.AddDays(7 * pattern.Interval);
        }

        var daysOfWeek = pattern.DaysOfWeek.Select(d => Enum.Parse<DayOfWeek>(d)).OrderBy(d => d).ToList();
        var currentDayOfWeek = current.DayOfWeek;
        
        // Find next occurrence day in the same week
        var nextDay = daysOfWeek.FirstOrDefault(d => d > currentDayOfWeek);
        
        if (nextDay != default)
        {
            // Next occurrence is in the same week
            var daysToAdd = (int)nextDay - (int)currentDayOfWeek;
            return current.AddDays(daysToAdd);
        }
        else
        {
            // Next occurrence is in the following week(s)
            var firstDayNextWeek = daysOfWeek.First();
            var daysToAdd = 7 * pattern.Interval - (int)currentDayOfWeek + (int)firstDayNextWeek;
            return current.AddDays(daysToAdd);
        }
    }

    /// <summary>
    /// Calculate monthly recurrence
    /// </summary>
    private static DateTimeOffset CalculateMonthlyOccurrence(DateTimeOffset current, RecurrencePatternDto pattern)
    {
        var nextMonth = current.AddMonths(pattern.Interval);
        
        if (pattern.DayOfMonth.HasValue)
        {
            var targetDay = Math.Min(pattern.DayOfMonth.Value, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            return new DateTimeOffset(nextMonth.Year, nextMonth.Month, targetDay,
                current.Hour, current.Minute, current.Second, current.Offset);
        }
        
        return nextMonth;
    }

    /// <summary>
    /// Calculate yearly recurrence
    /// </summary>
    private static DateTimeOffset CalculateYearlyOccurrence(DateTimeOffset current, RecurrencePatternDto pattern)
    {
        var nextYear = current.AddYears(pattern.Interval);
        
        if (pattern.Month.HasValue)
        {
            var targetMonth = pattern.Month.Value;
            var targetDay = pattern.DayOfMonth ?? current.Day;
            targetDay = Math.Min(targetDay, DateTime.DaysInMonth(nextYear.Year, targetMonth));
            
            return new DateTimeOffset(nextYear.Year, targetMonth, targetDay,
                current.Hour, current.Minute, current.Second, current.Offset);
        }
        
        return nextYear;
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private async Task<ScheduledMessageDto> MapToScheduledMessageDtoAsync(ScheduledMessage scheduledMessage)
    {
        Dictionary<string, string>? templateVariables = null;
        if (!string.IsNullOrEmpty(scheduledMessage.TemplateVariables))
        {
            try
            {
                templateVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(scheduledMessage.TemplateVariables);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }

        RecurrencePatternDto? recurrencePattern = null;
        if (!string.IsNullOrEmpty(scheduledMessage.RecurrencePattern))
        {
            try
            {
                recurrencePattern = JsonSerializer.Deserialize<RecurrencePatternDto>(scheduledMessage.RecurrencePattern);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }

        MessageTemplateDto? template = null;
        if (scheduledMessage.Template != null)
        {
            template = await _templateService.GetTemplateAsync(scheduledMessage.SenderId, scheduledMessage.Template.Id);
        }

        return new ScheduledMessageDto
        {
            Id = scheduledMessage.Id,
            SenderId = scheduledMessage.SenderId,
            SenderName = scheduledMessage.Sender?.UserName ?? "Unknown",
            ConversationId = scheduledMessage.ConversationId,
            ConversationTitle = scheduledMessage.Conversation?.Title ?? "Unknown",
            TemplateId = scheduledMessage.TemplateId,
            Template = template,
            MessageType = scheduledMessage.MessageType,
            Content = scheduledMessage.Content,
            TemplateVariables = templateVariables,
            ScheduledAt = scheduledMessage.ScheduledAt,
            TimeZone = scheduledMessage.TimeZone,
            RecurrencePattern = recurrencePattern,
            IsRecurring = scheduledMessage.IsRecurring,
            NextOccurrence = scheduledMessage.NextOccurrence,
            Status = scheduledMessage.Status,
            CreatedAt = scheduledMessage.CreatedAt,
            SentAt = scheduledMessage.SentAt,
            SentMessageId = scheduledMessage.SentMessageId,
            ErrorMessage = scheduledMessage.ErrorMessage,
            AttemptCount = scheduledMessage.AttemptCount,
            OccurrenceCount = scheduledMessage.OccurrenceCount
        };
    }
}