namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Platform types for push notifications
/// </summary>
public enum NotificationPlatform
{
    /// <summary>
    /// iOS devices (Apple Push Notification Service)
    /// </summary>
    iOS,

    /// <summary>
    /// Android devices (Firebase Cloud Messaging)
    /// </summary>
    Android,

    /// <summary>
    /// Web browsers (Web Push Protocol)
    /// </summary>
    Web,

    /// <summary>
    /// Windows devices (Windows Notification Service)
    /// </summary>
    Windows,

    /// <summary>
    /// macOS devices (Apple Push Notification Service)
    /// </summary>
    macOS
}