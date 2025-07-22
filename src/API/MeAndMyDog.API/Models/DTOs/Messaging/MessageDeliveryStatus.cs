using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Message delivery status information
/// </summary>
public class MessageDeliveryStatus
{
    /// <summary>
    /// Message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Overall message status
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// When message was sent
    /// </summary>
    public DateTimeOffset SentAt { get; set; }

    /// <summary>
    /// List of delivery/read receipts per recipient
    /// </summary>
    public List<RecipientStatus> Recipients { get; set; } = new();
}