using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.PetMedications;

/// <summary>
/// Request model for adding/updating medication
/// </summary>
public class AddMedicationRequest
{
    /// <summary>
    /// Name of the medication
    /// </summary>
    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(200, ErrorMessage = "Medication name cannot exceed 200 characters")]
    public string MedicationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Generic name
    /// </summary>
    [StringLength(200, ErrorMessage = "Generic name cannot exceed 200 characters")]
    public string? GenericName { get; set; }
    
    /// <summary>
    /// Type of medication
    /// </summary>
    [Required(ErrorMessage = "Medication type is required")]
    [StringLength(100, ErrorMessage = "Medication type cannot exceed 100 characters")]
    public string MedicationType { get; set; } = string.Empty;
    
    /// <summary>
    /// Dosage amount
    /// </summary>
    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(50, ErrorMessage = "Dosage cannot exceed 50 characters")]
    public string Dosage { get; set; } = string.Empty;
    
    /// <summary>
    /// Unit of measurement
    /// </summary>
    [Required(ErrorMessage = "Dosage unit is required")]
    [StringLength(20, ErrorMessage = "Dosage unit cannot exceed 20 characters")]
    public string DosageUnit { get; set; } = string.Empty;
    
    /// <summary>
    /// Frequency of administration
    /// </summary>
    [Required(ErrorMessage = "Frequency is required")]
    [StringLength(100, ErrorMessage = "Frequency cannot exceed 100 characters")]
    public string Frequency { get; set; } = string.Empty;
    
    /// <summary>
    /// Route of administration
    /// </summary>
    [Required(ErrorMessage = "Administration route is required")]
    [StringLength(50, ErrorMessage = "Administration route cannot exceed 50 characters")]
    public string AdministrationRoute { get; set; } = string.Empty;
    
    /// <summary>
    /// Start date
    /// </summary>
    [Required(ErrorMessage = "Start date is required")]
    public DateTimeOffset StartDate { get; set; }
    
    /// <summary>
    /// End date (if applicable)
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
    
    /// <summary>
    /// Reason for medication
    /// </summary>
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? Reason { get; set; }
    
    /// <summary>
    /// Prescribing veterinarian
    /// </summary>
    [StringLength(200, ErrorMessage = "Prescribing vet cannot exceed 200 characters")]
    public string? PrescribingVet { get; set; }
    
    /// <summary>
    /// Prescribing clinic
    /// </summary>
    [StringLength(200, ErrorMessage = "Prescribing clinic cannot exceed 200 characters")]
    public string? PrescribingClinic { get; set; }
    
    /// <summary>
    /// Prescription number
    /// </summary>
    [StringLength(50, ErrorMessage = "Prescription number cannot exceed 50 characters")]
    public string? PrescriptionNumber { get; set; }
    
    /// <summary>
    /// Administration instructions
    /// </summary>
    [StringLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters")]
    public string? Instructions { get; set; }
    
    /// <summary>
    /// Known side effects
    /// </summary>
    [StringLength(500, ErrorMessage = "Side effects cannot exceed 500 characters")]
    public string? SideEffects { get; set; }
    
    /// <summary>
    /// Food interactions
    /// </summary>
    [StringLength(500, ErrorMessage = "Food interactions cannot exceed 500 characters")]
    public string? FoodInteractions { get; set; }
    
    /// <summary>
    /// Drug interactions
    /// </summary>
    [StringLength(500, ErrorMessage = "Drug interactions cannot exceed 500 characters")]
    public string? DrugInteractions { get; set; }
    
    /// <summary>
    /// Cost
    /// </summary>
    [Range(0, 10000, ErrorMessage = "Cost must be between 0 and 10000")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Refills remaining
    /// </summary>
    [Range(0, 100, ErrorMessage = "Refills remaining must be between 0 and 100")]
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
    public List<string>? ReminderTimes { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}