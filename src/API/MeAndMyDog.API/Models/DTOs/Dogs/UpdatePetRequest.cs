using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Request model for updating an existing pet profile
/// </summary>
public class UpdatePetRequest
{
    /// <summary>
    /// Pet's name (required)
    /// </summary>
    [Required(ErrorMessage = "Pet name is required")]
    [StringLength(100, ErrorMessage = "Pet name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary breed
    /// </summary>
    [StringLength(100, ErrorMessage = "Breed cannot exceed 100 characters")]
    public string? Breed { get; set; }
    
    /// <summary>
    /// Secondary breed for mixed breeds
    /// </summary>
    [StringLength(100, ErrorMessage = "Secondary breed cannot exceed 100 characters")]
    public string? SecondaryBreed { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTimeOffset? DateOfBirth { get; set; }
    
    /// <summary>
    /// Weight in kilograms
    /// </summary>
    [Range(0.1, 200, ErrorMessage = "Weight must be between 0.1 and 200 kg")]
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Height in centimeters
    /// </summary>
    [Range(1, 150, ErrorMessage = "Height must be between 1 and 150 cm")]
    public decimal? Height { get; set; }
    
    /// <summary>
    /// Gender (Male, Female, Unknown)
    /// </summary>
    [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters")]
    public string? Gender { get; set; }
    
    /// <summary>
    /// Whether the pet is neutered/spayed
    /// </summary>
    public bool? IsNeutered { get; set; }
    
    /// <summary>
    /// Coat color
    /// </summary>
    [StringLength(50, ErrorMessage = "Coat color cannot exceed 50 characters")]
    public string? CoatColor { get; set; }
    
    /// <summary>
    /// Coat type
    /// </summary>
    [StringLength(50, ErrorMessage = "Coat type cannot exceed 50 characters")]
    public string? CoatType { get; set; }
    
    /// <summary>
    /// Eye color
    /// </summary>
    [StringLength(30, ErrorMessage = "Eye color cannot exceed 30 characters")]
    public string? EyeColor { get; set; }
    
    /// <summary>
    /// Microchip number
    /// </summary>
    [StringLength(50, ErrorMessage = "Microchip number cannot exceed 50 characters")]
    public string? MicrochipNumber { get; set; }
    
    /// <summary>
    /// Registration number
    /// </summary>
    [StringLength(50, ErrorMessage = "Registration number cannot exceed 50 characters")]
    public string? RegistrationNumber { get; set; }
    
    /// <summary>
    /// Dietary requirements
    /// </summary>
    [StringLength(500, ErrorMessage = "Dietary requirements cannot exceed 500 characters")]
    public string? DietaryRequirements { get; set; }
    
    /// <summary>
    /// Known allergies
    /// </summary>
    [StringLength(500, ErrorMessage = "Allergies cannot exceed 500 characters")]
    public string? Allergies { get; set; }
    
    /// <summary>
    /// Temperament description
    /// </summary>
    [StringLength(500, ErrorMessage = "Temperament cannot exceed 500 characters")]
    public string? Temperament { get; set; }
    
    /// <summary>
    /// Energy level (1-10)
    /// </summary>
    [Range(1, 10, ErrorMessage = "Energy level must be between 1 and 10")]
    public int? EnergyLevel { get; set; }
    
    /// <summary>
    /// Socialization level (1-10)
    /// </summary>
    [Range(1, 10, ErrorMessage = "Socialization level must be between 1 and 10")]
    public int? SocializationLevel { get; set; }
    
    /// <summary>
    /// Training level (1-10)
    /// </summary>
    [Range(1, 10, ErrorMessage = "Training level must be between 1 and 10")]
    public int? TrainingLevel { get; set; }
    
    /// <summary>
    /// Emergency contact information
    /// </summary>
    [StringLength(200, ErrorMessage = "Emergency contact cannot exceed 200 characters")]
    public string? EmergencyContact { get; set; }
    
    /// <summary>
    /// Emergency contact phone
    /// </summary>
    [StringLength(20, ErrorMessage = "Emergency contact phone cannot exceed 20 characters")]
    public string? EmergencyContactPhone { get; set; }
    
    /// <summary>
    /// Preferred veterinarian
    /// </summary>
    [StringLength(200, ErrorMessage = "Preferred vet cannot exceed 200 characters")]
    public string? PreferredVet { get; set; }
    
    /// <summary>
    /// Preferred veterinarian phone
    /// </summary>
    [StringLength(20, ErrorMessage = "Preferred vet phone cannot exceed 20 characters")]
    public string? PreferredVetPhone { get; set; }
    
    /// <summary>
    /// Insurance provider
    /// </summary>
    [StringLength(100, ErrorMessage = "Insurance provider cannot exceed 100 characters")]
    public string? InsuranceProvider { get; set; }
    
    /// <summary>
    /// Insurance policy number
    /// </summary>
    [StringLength(50, ErrorMessage = "Insurance policy number cannot exceed 50 characters")]
    public string? InsurancePolicyNumber { get; set; }
    
    /// <summary>
    /// Special notes about the pet
    /// </summary>
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}