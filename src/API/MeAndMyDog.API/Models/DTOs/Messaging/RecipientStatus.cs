namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Recipient status for message delivery
/// </summary>
public class RecipientStatus
{
    /// <summary>
    /// Recipient user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Recipient display name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Whether message was delivered to recipient
    /// </summary>
    public bool IsDelivered { get; set; }

    /// <summary>
    /// Whether message was read by recipient
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// When message was read (if read)
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }

    /// <summary>
    /// Device information for read receipt
    /// </summary>
    public string? DeviceInfo { get; set; }
}