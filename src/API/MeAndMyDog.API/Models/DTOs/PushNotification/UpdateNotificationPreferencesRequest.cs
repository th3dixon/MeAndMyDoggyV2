using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for updating notification preferences
/// </summary>
public class UpdateNotificationPreferencesRequest
{
    /// <summary>
    /// List of notification preferences to update
    /// </summary>
    [Required]
    public List<NotificationPreferenceUpdateDto> Preferences { get; set; } = new();
}