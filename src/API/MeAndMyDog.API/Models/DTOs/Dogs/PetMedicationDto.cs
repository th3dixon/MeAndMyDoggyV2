namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Data transfer object for pet medications
/// </summary>
public class PetMedicationDto
{
    /// <summary>
    /// Unique identifier for the medication record
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet ID this medication belongs to
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the medication
    /// </summary>
    public string MedicationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Generic name
    /// </summary>
    public string? GenericName { get; set; }
    
    /// <summary>
    /// Type of medication
    /// </summary>
    public string MedicationType { get; set; } = string.Empty;
    
    /// <summary>
    /// Dosage amount
    /// </summary>
    public string Dosage { get; set; } = string.Empty;
    
    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string DosageUnit { get; set; } = string.Empty;
    
    /// <summary>
    /// Frequency of administration
    /// </summary>
    public string Frequency { get; set; } = string.Empty;
    
    /// <summary>
    /// Route of administration
    /// </summary>
    public string AdministrationRoute { get; set; } = string.Empty;
    
    /// <summary>
    /// Start date
    /// </summary>
    public DateTimeOffset StartDate { get; set; }
    
    /// <summary>
    /// End date (if applicable)
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    
    /// <summary>
    /// Reason for medication
    /// </summary>
    public string? Reason { get; set; }
    
    /// <summary>
    /// Prescribing veterinarian
    /// </summary>
    public string? PrescribingVet { get; set; }
    
    /// <summary>
    /// Prescribing clinic
    /// </summary>
    public string? PrescribingClinic { get; set; }
    
    /// <summary>
    /// Prescription number
    /// </summary>
    public string? PrescriptionNumber { get; set; }
    
    /// <summary>
    /// Administration instructions
    /// </summary>
    public string? Instructions { get; set; }
    
    /// <summary>
    /// Known side effects
    /// </summary>
    public string? SideEffects { get; set; }
    
    /// <summary>
    /// Food interactions
    /// </summary>
    public string? FoodInteractions { get; set; }
    
    /// <summary>
    /// Drug interactions
    /// </summary>
    public string? DrugInteractions { get; set; }
    
    /// <summary>
    /// Cost
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Refills remaining
    /// </summary>
    public int? RefillsRemaining { get; set; }
    
    /// <summary>
    /// Whether currently taking
    /// </summary>
    public bool IsCurrentlyTaking { get; set; } = true;
    
    /// <summary>
    /// Whether reminders are enabled
    /// </summary>
    public bool ReminderEnabled { get; set; } = true;
    
    /// <summary>
    /// Reminder times
    /// </summary>
    public List<string> ReminderTimes { get; set; } = new();
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Days remaining if end date is set
    /// </summary>
    public int? DaysRemaining { get; set; }
    
    /// <summary>
    /// Whether refill is needed soon
    /// </summary>
    public bool NeedsRefill { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}