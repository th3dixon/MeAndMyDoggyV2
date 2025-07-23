namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Data transfer object for pet care reminders
/// </summary>
public class PetCareReminderDto
{
    /// <summary>
    /// Unique identifier for the reminder
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet ID this reminder belongs to
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Reminder title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of care (Feeding, Grooming, Exercise, Medication, Vet, etc.)
    /// </summary>
    public string CareType { get; set; } = string.Empty;
    
    /// <summary>
    /// Priority level (Low, Medium, High, Critical)
    /// </summary>
    public string Priority { get; set; } = "Medium";
    
    /// <summary>
    /// When the reminder is due
    /// </summary>
    public DateTimeOffset DueDate { get; set; }
    
    /// <summary>
    /// Frequency (Once, Daily, Weekly, Monthly, Yearly)
    /// </summary>
    public string Frequency { get; set; } = "Once";
    
    /// <summary>
    /// Interval for recurring reminders
    /// </summary>
    public int Interval { get; set; } = 1;
    
    /// <summary>
    /// Minutes before due date to send notification
    /// </summary>
    public int NotificationMinutes { get; set; } = 60;
    
    /// <summary>
    /// Whether the reminder is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// When completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
    
    /// <summary>
    /// Completion notes
    /// </summary>
    public string? CompletionNotes { get; set; }
    
    /// <summary>
    /// Whether notifications are enabled
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;
    
    /// <summary>
    /// Next due date for recurring reminders
    /// </summary>
    public DateTimeOffset? NextDueDate { get; set; }
    
    /// <summary>
    /// Days until due (calculated field)
    /// </summary>
    public int DaysUntilDue { get; set; }
    
    /// <summary>
    /// Whether this reminder is overdue
    /// </summary>
    public bool IsOverdue { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}