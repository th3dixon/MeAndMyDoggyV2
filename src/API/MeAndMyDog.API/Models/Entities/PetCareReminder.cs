namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a care reminder for a pet (grooming, feeding, exercise, etc.)
/// </summary>
public class PetCareReminder
{
    /// <summary>
    /// Unique identifier for the reminder
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the pet
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Reminder title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the care task
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
    /// When the reminder should trigger
    /// </summary>
    public DateTimeOffset DueDate { get; set; }
    
    /// <summary>
    /// Frequency of the reminder (Once, Daily, Weekly, Monthly, Yearly)
    /// </summary>
    public string Frequency { get; set; } = "Once";
    
    /// <summary>
    /// Interval for recurring reminders (e.g., every 2 weeks = 2)
    /// </summary>
    public int Interval { get; set; } = 1;
    
    /// <summary>
    /// When to start reminder notifications (minutes before due date)
    /// </summary>
    public int NotificationMinutes { get; set; } = 60;
    
    /// <summary>
    /// Whether the reminder is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// When the reminder was completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
    
    /// <summary>
    /// Notes added when completing the reminder
    /// </summary>
    public string? CompletionNotes { get; set; }
    
    /// <summary>
    /// Whether notifications are enabled for this reminder
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;
    
    /// <summary>
    /// Last time a notification was sent
    /// </summary>
    public DateTimeOffset? LastNotificationSent { get; set; }
    
    /// <summary>
    /// Next scheduled date for recurring reminders
    /// </summary>
    public DateTimeOffset? NextDueDate { get; set; }
    
    /// <summary>
    /// Whether the reminder is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the reminder was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the reminder was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the pet
    /// </summary>
    public virtual DogProfile Pet { get; set; } = null!;
}