using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for scheduled message operations
/// </summary>
public class ScheduledMessageResponse
{
    /// <summary>
    /// Whether operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Scheduled message ID if created/updated
    /// </summary>
    public string? ScheduledMessageId { get; set; }

    /// <summary>
    /// Scheduled message data
    /// </summary>
    public ScheduledMessageDto? ScheduledMessage { get; set; }
}