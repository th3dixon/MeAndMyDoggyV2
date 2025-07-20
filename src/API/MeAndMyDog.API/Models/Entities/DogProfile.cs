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
    /// Special notes about the dog
    /// </summary>
    public string? Notes { get; set; }
    
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
}