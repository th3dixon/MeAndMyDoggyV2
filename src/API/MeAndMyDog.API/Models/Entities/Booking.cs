namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a booking made by a customer with a service provider
/// Used for availability checking and conflict prevention
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier for the booking
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Reference number for customer communication
    /// </summary>
    public string BookingReference { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the service provider
    /// </summary>
    public string ServiceProviderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the customer/pet owner
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to the dog profile (if applicable)
    /// </summary>
    public string? DogId { get; set; }
    
    /// <summary>
    /// Foreign key to the specific service being booked
    /// </summary>
    public string? ServiceId { get; set; }
    
    /// <summary>
    /// Foreign key to the service category
    /// </summary>
    public Guid? ServiceCategoryId { get; set; }
    
    /// <summary>
    /// Foreign key to the sub-service
    /// </summary>
    public Guid? SubServiceId { get; set; }
    
    /// <summary>
    /// Start date and time of the booking
    /// </summary>
    public DateTimeOffset StartDateTime { get; set; }
    
    /// <summary>
    /// End date and time of the booking
    /// </summary>
    public DateTimeOffset EndDateTime { get; set; }
    
    /// <summary>
    /// Duration in minutes (calculated field for easier queries)
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Number of pets involved in this booking
    /// </summary>
    public int PetCount { get; set; } = 1;
    
    /// <summary>
    /// Booking status: Pending, Confirmed, In_Progress, Completed, Cancelled, No_Show
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Payment status: Pending, Paid, Partial, Refunded, Failed
    /// </summary>
    public string PaymentStatus { get; set; } = "Pending";
    
    /// <summary>
    /// Total price for the booking
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// Base price before surcharges
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Weekend surcharge amount
    /// </summary>
    public decimal? WeekendSurcharge { get; set; }
    
    /// <summary>
    /// Evening surcharge amount
    /// </summary>
    public decimal? EveningSurcharge { get; set; }
    
    /// <summary>
    /// Emergency service surcharge
    /// </summary>
    public decimal? EmergencySurcharge { get; set; }
    
    /// <summary>
    /// Service location address
    /// </summary>
    public string? ServiceLocation { get; set; }
    
    /// <summary>
    /// Special instructions from customer
    /// </summary>
    public string? SpecialInstructions { get; set; }
    
    /// <summary>
    /// Notes from the service provider
    /// </summary>
    public string? ProviderNotes { get; set; }
    
    /// <summary>
    /// Cancellation reason (if cancelled)
    /// </summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>
    /// Who cancelled the booking: Customer, Provider, System
    /// </summary>
    public string? CancelledBy { get; set; }
    
    /// <summary>
    /// When the booking was cancelled
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }
    
    /// <summary>
    /// When the booking was confirmed by the provider
    /// </summary>
    public DateTimeOffset? ConfirmedAt { get; set; }
    
    /// <summary>
    /// When the service was completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
    
    /// <summary>
    /// Whether this booking was created as an emergency request
    /// </summary>
    public bool IsEmergency { get; set; } = false;
    
    /// <summary>
    /// Whether this is a recurring booking
    /// </summary>
    public bool IsRecurring { get; set; } = false;
    
    /// <summary>
    /// Recurrence pattern (if recurring)
    /// </summary>
    public string? RecurrencePattern { get; set; }
    
    /// <summary>
    /// Parent booking ID (for recurring bookings)
    /// </summary>
    public string? ParentBookingId { get; set; }
    
    /// <summary>
    /// External calendar event ID (Google Calendar, etc.)
    /// </summary>
    public string? ExternalCalendarEventId { get; set; }
    
    /// <summary>
    /// When the booking was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the booking was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the service provider
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public virtual ApplicationUser Customer { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the dog profile
    /// </summary>
    public virtual DogProfile? Dog { get; set; }
    
    /// <summary>
    /// Navigation property to the service
    /// </summary>
    public virtual Service? Service { get; set; }
    
    /// <summary>
    /// Navigation property to the service category
    /// </summary>
    public virtual ServiceCategory? ServiceCategory { get; set; }
    
    /// <summary>
    /// Navigation property to the sub-service
    /// </summary>
    public virtual SubService? SubService { get; set; }
}