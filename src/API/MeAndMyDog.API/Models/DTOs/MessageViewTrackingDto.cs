namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Message view tracking DTO
/// </summary>
public class MessageViewTrackingDto
{
    /// <summary>
    /// View tracking unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Self-destruct message ID
    /// </summary>
    public string SelfDestructMessageId { get; set; } = string.Empty;

    /// <summary>
    /// User who viewed the message
    /// </summary>
    public string ViewedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// When the message was viewed
    /// </summary>
    public DateTimeOffset ViewedAt { get; set; }

    /// <summary>
    /// How long the message was viewed (milliseconds)
    /// </summary>
    public long ViewDurationMs { get; set; }

    /// <summary>
    /// Client IP address
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Client user agent
    /// </summary>
    public string? ClientUserAgent { get; set; }

    /// <summary>
    /// Whether this view triggered timer start
    /// </summary>
    public bool TriggeredTimer { get; set; }
}