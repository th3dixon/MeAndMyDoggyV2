using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for scheduled messages
/// </summary>
public class ScheduledMessageDto
{
    /// <summary>
    /// Scheduled message unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who scheduled the message
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Target conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation title
    /// </summary>
    public string ConversationTitle { get; set; } = string.Empty;

    /// <summary>
    /// Message template ID (optional)
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// Template information
    /// </summary>
    public MessageTemplateDto? Template { get; set; }

    /// <summary>
    /// Message type
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Message content (resolved)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Template variables used
    /// </summary>
    public Dictionary<string, string>? TemplateVariables { get; set; }

    /// <summary>
    /// When to send the message
    /// </summary>
    public DateTimeOffset ScheduledAt { get; set; }

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Recurrence information
    /// </summary>
    public RecurrencePatternDto? RecurrencePattern { get; set; }

    /// <summary>
    /// Whether message should repeat
    /// </summary>
    public bool IsRecurring { get; set; }

    /// <summary>
    /// Next occurrence for recurring messages
    /// </summary>
    public DateTimeOffset? NextOccurrence { get; set; }

    /// <summary>
    /// Scheduled message status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// When message was created/scheduled
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When message was actually sent
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// ID of the actual message that was sent
    /// </summary>
    public string? SentMessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of send attempts
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Current occurrence count
    /// </summary>
    public int OccurrenceCount { get; set; }
}