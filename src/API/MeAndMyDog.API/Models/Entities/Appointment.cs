namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a scheduled appointment between a pet owner and service provider
/// </summary>
public class Appointment
{
    /// <summary>
    /// Unique identifier for the appointment
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    /// <summary>
    /// Foreign key to the pet owner (user)
    /// </summary>
    public string PetOwnerId { get; set; } = string.Empty;
    /// <summary>
    /// Foreign key to the dog (optional)
    /// </summary>
    public string? DogId { get; set; }
    /// <summary>
    /// Type of service being provided
    /// </summary>
    public string ServiceType { get; set; } = string.Empty;
    /// <summary>
    /// Scheduled start time of the appointment
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
    /// <summary>
    /// Scheduled end time of the appointment
    /// </summary>
    public DateTimeOffset EndTime { get; set; }
    /// <summary>
    /// Current status of the appointment (Scheduled, Confirmed, InProgress, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = "Scheduled";
    /// <summary>
    /// Location where the appointment will take place
    /// </summary>
    public string? Location { get; set; }
    /// <summary>
    /// Additional notes about the appointment
    /// </summary>
    public string? Notes { get; set; }
    /// <summary>
    /// Price for the service in GBP
    /// </summary>
    public decimal? Price { get; set; }
    /// <summary>
    /// Payment status (Pending, Paid, Failed, Refunded)
    /// </summary>
    public string? PaymentStatus { get; set; }
    /// <summary>
    /// Payment method used (Card, Cash, Bank Transfer)
    /// </summary>
    public string? PaymentMethod { get; set; }
    /// <summary>
    /// Google Calendar event ID for calendar integration
    /// </summary>
    public string? GoogleCalendarEventId { get; set; }
    /// <summary>
    /// When the appointment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// When the appointment was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    /// <summary>
    /// Navigation property to the pet owner
    /// </summary>
    public virtual ApplicationUser PetOwner { get; set; } = null!;
    /// <summary>
    /// Navigation property to the dog (optional)
    /// </summary>
    public virtual DogProfile? Dog { get; set; }
}