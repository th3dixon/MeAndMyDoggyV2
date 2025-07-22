namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Status of a scheduled message
/// </summary>
public enum ScheduledMessageStatus
{
    /// <summary>
    /// Message is scheduled and waiting to be sent
    /// </summary>
    Pending,

    /// <summary>
    /// Message is being processed for sending
    /// </summary>
    Processing,

    /// <summary>
    /// Message has been sent successfully
    /// </summary>
    Sent,

    /// <summary>
    /// Message sending failed
    /// </summary>
    Failed,

    /// <summary>
    /// Message was cancelled before sending
    /// </summary>
    Cancelled,

    /// <summary>
    /// Message expired (past scheduled time with no retry)
    /// </summary>
    Expired,

    /// <summary>
    /// Recurring message is paused
    /// </summary>
    Paused
}