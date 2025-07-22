using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for scheduling a message
/// </summary>
public class ScheduleMessageRequest
{
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
    public string MessageType { get; set; } = "Text";

    /// <summary>
    /// Message content (required if no template)
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Template variables (if using template)
    /// </summary>
    public Dictionary<string, string>? TemplateVariables { get; set; }

    /// <summary>
    /// When to send the message
    /// </summary>
    [Required]
    public DateTimeOffset ScheduledAt { get; set; }

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Recurrence pattern for repeating messages
    /// </summary>
    public RecurrencePatternDto? RecurrencePattern { get; set; }
}