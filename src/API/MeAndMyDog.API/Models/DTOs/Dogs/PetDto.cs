namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Data transfer object for pet information
/// </summary>
public class PetDto
{
    /// <summary>
    /// Unique identifier for the pet
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet owner's user ID
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary breed
    /// </summary>
    public string? Breed { get; set; }
    
    /// <summary>
    /// Secondary breed for mixed breeds
    /// </summary>
    public string? SecondaryBreed { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTimeOffset? DateOfBirth { get; set; }
    
    /// <summary>
    /// Age in years (calculated from DateOfBirth)
    /// </summary>
    public int? Age { get; set; }
    
    /// <summary>
    /// Weight in kilograms
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Height in centimeters
    /// </summary>
    public decimal? Height { get; set; }
    
    /// <summary>
    /// Gender (Male, Female, Unknown)
    /// </summary>
    public string? Gender { get; set; }
    
    /// <summary>
    /// Whether the pet is neutered/spayed
    /// </summary>
    public bool? IsNeutered { get; set; }
    
    /// <summary>
    /// Coat color
    /// </summary>
    public string? CoatColor { get; set; }
    
    /// <summary>
    /// Coat type
    /// </summary>
    public string? CoatType { get; set; }
    
    /// <summary>
    /// Eye color
    /// </summary>
    public string? EyeColor { get; set; }
    
    /// <summary>
    /// Microchip number
    /// </summary>
    public string? MicrochipNumber { get; set; }
    
    /// <summary>
    /// Registration number
    /// </summary>
    public string? RegistrationNumber { get; set; }
    
    /// <summary>
    /// Profile image URL
    /// </summary>
    public string? ProfileImageUrl { get; set; }
    
    /// <summary>
    /// Dietary requirements
    /// </summary>
    public string? DietaryRequirements { get; set; }
    
    /// <summary>
    /// Known allergies
    /// </summary>
    public string? Allergies { get; set; }
    
    /// <summary>
    /// Temperament description
    /// </summary>
    public string? Temperament { get; set; }
    
    /// <summary>
    /// Energy level (1-10)
    /// </summary>
    public int? EnergyLevel { get; set; }
    
    /// <summary>
    /// Socialization level (1-10)
    /// </summary>
    public int? SocializationLevel { get; set; }
    
    /// <summary>
    /// Training level (1-10)
    /// </summary>
    public int? TrainingLevel { get; set; }
    
    /// <summary>
    /// Emergency contact information
    /// </summary>
    public string? EmergencyContact { get; set; }
    
    /// <summary>
    /// Emergency contact phone
    /// </summary>
    public string? EmergencyContactPhone { get; set; }
    
    /// <summary>
    /// Preferred veterinarian
    /// </summary>
    public string? PreferredVet { get; set; }
    
    /// <summary>
    /// Preferred veterinarian phone
    /// </summary>
    public string? PreferredVetPhone { get; set; }
    
    /// <summary>
    /// Insurance provider
    /// </summary>
    public string? InsuranceProvider { get; set; }
    
    /// <summary>
    /// Insurance policy number
    /// </summary>
    public string? InsurancePolicyNumber { get; set; }
    
    /// <summary>
    /// Special notes about the pet
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Whether the profile is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Collection of pet photos
    /// </summary>
    public List<PetPhotoDto> Photos { get; set; } = new();
    
    /// <summary>
    /// Recent medical records
    /// </summary>
    public List<MedicalRecordSummaryDto> RecentMedicalRecords { get; set; } = new();
    
    /// <summary>
    /// Upcoming care reminders
    /// </summary>
    public List<PetCareReminderDto> UpcomingReminders { get; set; } = new();
    
    /// <summary>
    /// Current medications
    /// </summary>
    public List<PetMedicationDto> CurrentMedications { get; set; } = new();
    
    /// <summary>
    /// Recent vaccinations
    /// </summary>
    public List<PetVaccinationDto> RecentVaccinations { get; set; } = new();
}