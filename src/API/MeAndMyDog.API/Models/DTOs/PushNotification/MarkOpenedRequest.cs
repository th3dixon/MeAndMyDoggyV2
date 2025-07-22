namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for marking notification as opened
/// </summary>
public class MarkOpenedRequest
{
    /// <summary>
    /// Device ID that opened the notification
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;
}