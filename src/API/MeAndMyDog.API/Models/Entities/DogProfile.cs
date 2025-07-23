namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a dog profile in the system
/// </summary>
public class DogProfile
{
    /// <summary>
    /// Unique identifier for the dog profile
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the owner (user)
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Dog's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary breed
    /// </summary>
    public string? Breed { get; set; }
    
    /// <summary>
    /// Secondary breed (for mixed breeds)
    /// </summary>
    public string? SecondaryBreed { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTimeOffset? DateOfBirth { get; set; }
    
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
    /// Whether the dog is neutered/spayed
    /// </summary>
    public bool? IsNeutered { get; set; }
    
    /// <summary>
    /// Coat color
    /// </summary>
    public string? CoatColor { get; set; }
    
    /// <summary>
    /// Coat type (Short, Medium, Long, Curly, etc.)
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
    /// Registration number (Kennel Club, etc.)
    /// </summary>
    public string? RegistrationNumber { get; set; }
    
    /// <summary>
    /// Profile image URL
    /// </summary>
    public string? ProfileImageUrl { get; set; }
    
    /// <summary>
    /// Collection of pet photos
    /// </summary>
    public virtual ICollection<PetPhoto> Photos { get; set; } = new List<PetPhoto>();
    
    /// <summary>
    /// Special notes about the dog
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Dietary requirements or restrictions
    /// </summary>
    public string? DietaryRequirements { get; set; }
    
    /// <summary>
    /// Known allergies
    /// </summary>
    public string? Allergies { get; set; }
    
    /// <summary>
    /// Behavioral traits and temperament
    /// </summary>
    public string? Temperament { get; set; }
    
    /// <summary>
    /// Energy level (1-10 scale)
    /// </summary>
    public int? EnergyLevel { get; set; }
    
    /// <summary>
    /// Socialization level with other dogs (1-10 scale)
    /// </summary>
    public int? SocializationLevel { get; set; }
    
    /// <summary>
    /// Training level (1-10 scale)
    /// </summary>
    public int? TrainingLevel { get; set; }
    
    /// <summary>
    /// Emergency contact information
    /// </summary>
    public string? EmergencyContact { get; set; }
    
    /// <summary>
    /// Emergency contact phone number
    /// </summary>
    public string? EmergencyContactPhone { get; set; }
    
    /// <summary>
    /// Preferred veterinarian name
    /// </summary>
    public string? PreferredVet { get; set; }
    
    /// <summary>
    /// Preferred veterinarian phone number
    /// </summary>
    public string? PreferredVetPhone { get; set; }
    
    /// <summary>
    /// Insurance provider information
    /// </summary>
    public string? InsuranceProvider { get; set; }
    
    /// <summary>
    /// Insurance policy number
    /// </summary>
    public string? InsurancePolicyNumber { get; set; }
    
    /// <summary>
    /// Whether the profile is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the profile was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the profile was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the owner
    /// </summary>
    public virtual ApplicationUser Owner { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to medical records
    /// </summary>
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    
    /// <summary>
    /// Navigation property to appointments
    /// </summary>
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    /// <summary>
    /// Navigation property to AI health recommendations
    /// </summary>
    public virtual ICollection<AIHealthRecommendation> AIHealthRecommendations { get; set; } = new List<AIHealthRecommendation>();
    
    /// <summary>
    /// Navigation property to care reminders
    /// </summary>
    public virtual ICollection<PetCareReminder> CareReminders { get; set; } = new List<PetCareReminder>();
    
    /// <summary>
    /// Navigation property to vaccinations
    /// </summary>
    public virtual ICollection<PetVaccination> Vaccinations { get; set; } = new List<PetVaccination>();
    
    /// <summary>
    /// Navigation property to medications
    /// </summary>
    public virtual ICollection<PetMedication> Medications { get; set; } = new List<PetMedication>();
}