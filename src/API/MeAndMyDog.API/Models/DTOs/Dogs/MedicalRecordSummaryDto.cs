namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Summary data transfer object for medical records
/// </summary>
public class MedicalRecordSummaryDto
{
    /// <summary>
    /// Unique identifier for the medical record
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet ID this record belongs to
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of medical record (Checkup, Emergency, Surgery, etc.)
    /// </summary>
    public string RecordType { get; set; } = string.Empty;
    
    /// <summary>
    /// Brief description or title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Date of the medical event
    /// </summary>
    public DateTimeOffset RecordDate { get; set; }
    
    /// <summary>
    /// Veterinarian or clinic name
    /// </summary>
    public string? VetOrClinic { get; set; }
    
    /// <summary>
    /// Brief summary of the visit/treatment
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// Diagnosis or findings
    /// </summary>
    public string? Diagnosis { get; set; }
    
    /// <summary>
    /// Treatment provided
    /// </summary>
    public string? Treatment { get; set; }
    
    /// <summary>
    /// Follow-up required
    /// </summary>
    public bool FollowUpRequired { get; set; }
    
    /// <summary>
    /// Follow-up date if required
    /// </summary>
    public DateTimeOffset? FollowUpDate { get; set; }
    
    /// <summary>
    /// Cost of the visit/treatment
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}