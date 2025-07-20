namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a service provider in the platform
/// </summary>
public class ServiceProvider
{
    /// <summary>
    /// Unique identifier for the service provider
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the user account
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Business name
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;
    
    /// <summary>
    /// Business description
    /// </summary>
    public string? BusinessDescription { get; set; }
    
    /// <summary>
    /// Business address
    /// </summary>
    public string? BusinessAddress { get; set; }
    
    /// <summary>
    /// Business phone number
    /// </summary>
    public string? BusinessPhone { get; set; }
    
    /// <summary>
    /// Business email address
    /// </summary>
    public string? BusinessEmail { get; set; }
    
    /// <summary>
    /// Business website URL
    /// </summary>
    public string? BusinessWebsite { get; set; }
    
    /// <summary>
    /// Business license number
    /// </summary>
    public string? BusinessLicense { get; set; }
    
    /// <summary>
    /// Insurance policy information
    /// </summary>
    public string? InsurancePolicy { get; set; }
    
    /// <summary>
    /// Years of experience
    /// </summary>
    public int? YearsOfExperience { get; set; }
    
    /// <summary>
    /// Specializations (JSON array)
    /// </summary>
    public string? Specializations { get; set; }
    
    /// <summary>
    /// Service areas covered (JSON array)
    /// </summary>
    public string? ServiceAreas { get; set; }
    
    /// <summary>
    /// Standard hourly rate
    /// </summary>
    public decimal? HourlyRate { get; set; }
    
    /// <summary>
    /// Average rating from reviews
    /// </summary>
    public decimal Rating { get; set; } = 0.0m;
    
    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; } = 0;
    
    /// <summary>
    /// Average response time in hours
    /// </summary>
    public decimal ResponseTimeHours { get; set; } = 0.0m;
    
    /// <summary>
    /// Reliability score (0-1)
    /// </summary>
    public decimal ReliabilityScore { get; set; } = 0.0m;
    
    /// <summary>
    /// Time zone
    /// </summary>
    public string TimeZone { get; set; } = "UTC";
    
    /// <summary>
    /// Google Calendar ID for availability
    /// </summary>
    public string? GoogleCalendarId { get; set; }
    
    /// <summary>
    /// Whether the service provider is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether the service provider is verified
    /// </summary>
    public bool IsVerified { get; set; } = false;
    
    /// <summary>
    /// When the profile was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the profile was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to services offered
    /// </summary>
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    
    /// <summary>
    /// Navigation property to reviews
    /// </summary>
    public virtual ICollection<ServiceProviderReview> Reviews { get; set; } = new List<ServiceProviderReview>();
    
    /// <summary>
    /// Navigation property to appointments
    /// </summary>
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    /// <summary>
    /// Navigation property to availability slots
    /// </summary>
    public virtual ICollection<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();
}