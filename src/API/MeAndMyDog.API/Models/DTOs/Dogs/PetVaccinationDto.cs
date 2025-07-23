namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Data transfer object for pet vaccinations
/// </summary>
public class PetVaccinationDto
{
    /// <summary>
    /// Unique identifier for the vaccination record
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet ID this vaccination belongs to
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the vaccine
    /// </summary>
    public string VaccineName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type/category of vaccination
    /// </summary>
    public string VaccineType { get; set; } = string.Empty;
    
    /// <summary>
    /// Manufacturer
    /// </summary>
    public string? Manufacturer { get; set; }
    
    /// <summary>
    /// Batch/lot number
    /// </summary>
    public string? BatchNumber { get; set; }
    
    /// <summary>
    /// Date administered
    /// </summary>
    public DateTimeOffset DateAdministered { get; set; }
    
    /// <summary>
    /// Next due date
    /// </summary>
    public DateTimeOffset? NextDueDate { get; set; }
    
    /// <summary>
    /// Expiration date
    /// </summary>
    public DateTimeOffset? ExpirationDate { get; set; }
    
    /// <summary>
    /// Veterinarian name
    /// </summary>
    public string? VeterinarianName { get; set; }
    
    /// <summary>
    /// Clinic name
    /// </summary>
    public string? ClinicName { get; set; }
    
    /// <summary>
    /// Clinic address
    /// </summary>
    public string? ClinicAddress { get; set; }
    
    /// <summary>
    /// Clinic contact
    /// </summary>
    public string? ClinicContact { get; set; }
    
    /// <summary>
    /// Cost
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Adverse reactions
    /// </summary>
    public string? AdverseReactions { get; set; }
    
    /// <summary>
    /// Certificate URL
    /// </summary>
    public string? CertificateUrl { get; set; }
    
    /// <summary>
    /// Whether reminder is set
    /// </summary>
    public bool ReminderSet { get; set; }
    
    /// <summary>
    /// Days before next due date to remind
    /// </summary>
    public int ReminderDaysBefore { get; set; } = 30;
    
    /// <summary>
    /// Days until next vaccination due (calculated)
    /// </summary>
    public int? DaysUntilNextDue { get; set; }
    
    /// <summary>
    /// Whether next vaccination is overdue
    /// </summary>
    public bool IsOverdue { get; set; }
    
    /// <summary>
    /// Whether vaccination is current/valid
    /// </summary>
    public bool IsCurrent { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}