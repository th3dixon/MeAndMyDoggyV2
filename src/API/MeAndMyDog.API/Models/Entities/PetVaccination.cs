namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a vaccination record for a pet
/// </summary>
public class PetVaccination
{
    /// <summary>
    /// Unique identifier for the vaccination record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the pet
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the vaccine
    /// </summary>
    public string VaccineName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type/category of vaccination (Core, Non-core, Required, etc.)
    /// </summary>
    public string VaccineType { get; set; } = string.Empty;
    
    /// <summary>
    /// Manufacturer of the vaccine
    /// </summary>
    public string? Manufacturer { get; set; }
    
    /// <summary>
    /// Batch/lot number of the vaccine
    /// </summary>
    public string? BatchNumber { get; set; }
    
    /// <summary>
    /// Date the vaccination was administered
    /// </summary>
    public DateTimeOffset DateAdministered { get; set; }
    
    /// <summary>
    /// Date when the next dose is due
    /// </summary>
    public DateTimeOffset? NextDueDate { get; set; }
    
    /// <summary>
    /// Date when the vaccination expires
    /// </summary>
    public DateTimeOffset? ExpirationDate { get; set; }
    
    /// <summary>
    /// Name of the veterinarian who administered the vaccine
    /// </summary>
    public string? VeterinarianName { get; set; }
    
    /// <summary>
    /// Veterinary clinic where vaccination was given
    /// </summary>
    public string? ClinicName { get; set; }
    
    /// <summary>
    /// Clinic address
    /// </summary>
    public string? ClinicAddress { get; set; }
    
    /// <summary>
    /// Clinic contact information
    /// </summary>
    public string? ClinicContact { get; set; }
    
    /// <summary>
    /// Cost of the vaccination
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Additional notes about the vaccination
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Any adverse reactions noted
    /// </summary>
    public string? AdverseReactions { get; set; }
    
    /// <summary>
    /// Certificate or document URL
    /// </summary>
    public string? CertificateUrl { get; set; }
    
    /// <summary>
    /// Whether a reminder is set for the next dose
    /// </summary>
    public bool ReminderSet { get; set; }
    
    /// <summary>
    /// How many days before next due date to send reminder
    /// </summary>
    public int ReminderDaysBefore { get; set; } = 30;
    
    /// <summary>
    /// Whether the vaccination record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the vaccination record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the vaccination record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the pet
    /// </summary>
    public virtual DogProfile Pet { get; set; } = null!;
}