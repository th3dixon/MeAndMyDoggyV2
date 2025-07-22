namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Context message for search results
/// </summary>
public class MessageContextDto
{
    /// <summary>
    /// Message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// Message content (truncated)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the message was sent
    /// </summary>
    public DateTimeOffset SentAt { get; set; }

    /// <summary>
    /// Position relative to matched message (-1 = before, 1 = after)
    /// </summary>
    public int Position { get; set; }
}