using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.PetCareReminders;

/// <summary>
/// Request model for creating/updating reminders
/// </summary>
public class CreateReminderRequest
{
    /// <summary>
    /// Reminder title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of care
    /// </summary>
    [Required(ErrorMessage = "Care type is required")]
    [StringLength(50, ErrorMessage = "Care type cannot exceed 50 characters")]
    public string CareType { get; set; } = string.Empty;
    
    /// <summary>
    /// Priority level
    /// </summary>
    [StringLength(20, ErrorMessage = "Priority cannot exceed 20 characters")]
    public string Priority { get; set; } = "Medium";
    
    /// <summary>
    /// Due date
    /// </summary>
    [Required(ErrorMessage = "Due date is required")]
    public DateTimeOffset DueDate { get; set; }
    
    /// <summary>
    /// Frequency
    /// </summary>
    [StringLength(20, ErrorMessage = "Frequency cannot exceed 20 characters")]
    public string Frequency { get; set; } = "Once";
    
    /// <summary>
    /// Interval for recurring reminders
    /// </summary>
    [Range(1, 365, ErrorMessage = "Interval must be between 1 and 365")]
    public int Interval { get; set; } = 1;
    
    /// <summary>
    /// Minutes before due date to send notification
    /// </summary>
    [Range(0, 10080, ErrorMessage = "Notification minutes must be between 0 and 10080 (7 days)")]
    public int NotificationMinutes { get; set; } = 60;
    
    /// <summary>
    /// Whether notifications are enabled
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;
}