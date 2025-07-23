namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a medication record for a pet
/// </summary>
public class PetMedication
{
    /// <summary>
    /// Unique identifier for the medication record
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the pet
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the medication
    /// </summary>
    public string MedicationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Generic name of the medication
    /// </summary>
    public string? GenericName { get; set; }
    
    /// <summary>
    /// Type of medication (Prescription, Over-the-counter, Supplement, etc.)
    /// </summary>
    public string MedicationType { get; set; } = string.Empty;
    
    /// <summary>
    /// Dosage amount
    /// </summary>
    public string Dosage { get; set; } = string.Empty;
    
    /// <summary>
    /// Unit of measurement for dosage (mg, ml, tablets, etc.)
    /// </summary>
    public string DosageUnit { get; set; } = string.Empty;
    
    /// <summary>
    /// Frequency of administration (Once daily, Twice daily, As needed, etc.)
    /// </summary>
    public string Frequency { get; set; } = string.Empty;
    
    /// <summary>
    /// Route of administration (Oral, Topical, Injection, etc.)
    /// </summary>
    public string AdministrationRoute { get; set; } = string.Empty;
    
    /// <summary>
    /// Date medication was started
    /// </summary>
    public DateTimeOffset StartDate { get; set; }
    
    /// <summary>
    /// Date medication ends (if applicable)
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    
    /// <summary>
    /// Reason for prescribing the medication
    /// </summary>
    public string? Reason { get; set; }
    
    /// <summary>
    /// Prescribing veterinarian
    /// </summary>
    public string? PrescribingVet { get; set; }
    
    /// <summary>
    /// Veterinary clinic that prescribed the medication
    /// </summary>
    public string? PrescribingClinic { get; set; }
    
    /// <summary>
    /// Prescription number or reference
    /// </summary>
    public string? PrescriptionNumber { get; set; }
    
    /// <summary>
    /// Special instructions for administration
    /// </summary>
    public string? Instructions { get; set; }
    
    /// <summary>
    /// Known side effects to watch for
    /// </summary>
    public string? SideEffects { get; set; }
    
    /// <summary>
    /// Food interactions or dietary restrictions
    /// </summary>
    public string? FoodInteractions { get; set; }
    
    /// <summary>
    /// Drug interactions to be aware of
    /// </summary>
    public string? DrugInteractions { get; set; }
    
    /// <summary>
    /// Cost of the medication
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Number of refills remaining
    /// </summary>
    public int? RefillsRemaining { get; set; }
    
    /// <summary>
    /// Whether the medication is currently active
    /// </summary>
    public bool IsCurrentlyTaking { get; set; } = true;
    
    /// <summary>
    /// Whether reminders are set for this medication
    /// </summary>
    public bool ReminderEnabled { get; set; } = true;
    
    /// <summary>
    /// Times of day to take medication (JSON array)
    /// </summary>
    public string? ReminderTimes { get; set; }
    
    /// <summary>
    /// Additional notes about the medication
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Whether the medication record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the medication record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the medication record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the pet
    /// </summary>
    public virtual DogProfile Pet { get; set; } = null!;
}