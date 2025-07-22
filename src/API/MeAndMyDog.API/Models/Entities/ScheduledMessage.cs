using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a message scheduled for future delivery
/// </summary>
public class ScheduledMessage
{
    /// <summary>
    /// Scheduled message unique identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who scheduled the message
    /// </summary>
    [Required]
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Target conversation ID
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Message template ID (optional)
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// Message type
    /// </summary>
    [Required]
    [StringLength(50)]
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Message content (resolved template or direct content)
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Original template content before variable substitution
    /// </summary>
    public string? TemplateContent { get; set; }

    /// <summary>
    /// Template variables used for substitution (JSON)
    /// </summary>
    public string? TemplateVariables { get; set; }

    /// <summary>
    /// When to send the message
    /// </summary>
    public DateTimeOffset ScheduledAt { get; set; }

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    [StringLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Recurrence pattern (JSON) for repeating messages
    /// </summary>
    public string? RecurrencePattern { get; set; }

    /// <summary>
    /// Whether message should repeat
    /// </summary>
    public bool IsRecurring { get; set; } = false;

    /// <summary>
    /// Next occurrence for recurring messages
    /// </summary>
    public DateTimeOffset? NextOccurrence { get; set; }

    /// <summary>
    /// End date for recurring messages
    /// </summary>
    public DateTimeOffset? RecurrenceEndDate { get; set; }

    /// <summary>
    /// Maximum number of occurrences
    /// </summary>
    public int? MaxOccurrences { get; set; }

    /// <summary>
    /// Current occurrence count
    /// </summary>
    public int OccurrenceCount { get; set; } = 0;

    /// <summary>
    /// Scheduled message status
    /// </summary>
    [Required]
    [StringLength(50)]
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
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of send attempts
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Next retry time if failed
    /// </summary>
    public DateTimeOffset? NextRetryAt { get; set; }

    /// <summary>
    /// When scheduled message was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to sender
    /// </summary>
    public ApplicationUser Sender { get; set; } = null!;

    /// <summary>
    /// Navigation property to conversation
    /// </summary>
    public Conversation Conversation { get; set; } = null!;

    /// <summary>
    /// Navigation property to template
    /// </summary>
    public MessageTemplate? Template { get; set; }

    /// <summary>
    /// Navigation property to sent message
    /// </summary>
    public Message? SentMessage { get; set; }
}