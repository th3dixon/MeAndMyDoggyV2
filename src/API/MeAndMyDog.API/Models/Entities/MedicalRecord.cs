namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a medical record for a dog
/// </summary>
public class MedicalRecord
{
    /// <summary>
    /// Unique identifier for the medical record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the dog
    /// </summary>
    public string DogId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of medical record (Vaccination, Treatment, Checkup, etc.)
    /// </summary>
    public string RecordType { get; set; } = string.Empty;
    
    /// <summary>
    /// Title/name of the medical record
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the medical record
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Date of the medical event
    /// </summary>
    public DateTimeOffset EventDate { get; set; }
    
    /// <summary>
    /// Veterinarian or provider name
    /// </summary>
    public string? VeterinarianName { get; set; }
    
    /// <summary>
    /// Clinic or facility name
    /// </summary>
    public string? ClinicName { get; set; }
    
    /// <summary>
    /// Cost of the treatment/service
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Medications prescribed
    /// </summary>
    public string? Medications { get; set; }
    
    /// <summary>
    /// Follow-up instructions
    /// </summary>
    public string? FollowUpInstructions { get; set; }
    
    /// <summary>
    /// Next appointment date (if applicable)
    /// </summary>
    public DateTimeOffset? NextAppointmentDate { get; set; }
    
    /// <summary>
    /// Attached documents/images (JSON array of URLs)
    /// </summary>
    public string? Attachments { get; set; }
    
    /// <summary>
    /// Whether this record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the dog
    /// </summary>
    public virtual DogProfile Dog { get; set; } = null!;
}