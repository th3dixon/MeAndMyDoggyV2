namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Types of push notifications
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// New message received
    /// </summary>
    NewMessage,

    /// <summary>
    /// Message mention
    /// </summary>
    MessageMention,

    /// <summary>
    /// Incoming call
    /// </summary>
    IncomingCall,

    /// <summary>
    /// Missed call
    /// </summary>
    MissedCall,

    /// <summary>
    /// Voice message received
    /// </summary>
    VoiceMessage,

    /// <summary>
    /// New booking request
    /// </summary>
    BookingRequest,

    /// <summary>
    /// Booking confirmation
    /// </summary>
    BookingConfirmed,

    /// <summary>
    /// Booking cancelled
    /// </summary>
    BookingCancelled,

    /// <summary>
    /// Booking reminder
    /// </summary>
    BookingReminder,

    /// <summary>
    /// Payment received
    /// </summary>
    PaymentReceived,

    /// <summary>
    /// Payment reminder
    /// </summary>
    PaymentReminder,

    /// <summary>
    /// New review received
    /// </summary>
    NewReview,

    /// <summary>
    /// System announcement
    /// </summary>
    SystemAnnouncement,

    /// <summary>
    /// Security alert
    /// </summary>
    SecurityAlert,

    /// <summary>
    /// Account verification
    /// </summary>
    AccountVerification,

    /// <summary>
    /// Password reset
    /// </summary>
    PasswordReset,

    /// <summary>
    /// Welcome message for new users
    /// </summary>
    Welcome,

    /// <summary>
    /// General notification
    /// </summary>
    General
}