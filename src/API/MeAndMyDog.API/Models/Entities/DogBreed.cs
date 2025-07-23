using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a dog breed for autocomplete functionality
/// </summary>
public class DogBreed
{
    /// <summary>
    /// Unique identifier for the dog breed
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The name of the dog breed
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Alternative names or spellings for the breed (comma-separated)
    /// </summary>
    [MaxLength(500)]
    public string? AlternativeNames { get; set; }

    /// <summary>
    /// Size category of the breed (Small, Medium, Large, Giant)
    /// </summary>
    [MaxLength(20)]
    public string? SizeCategory { get; set; }

    /// <summary>
    /// Whether this breed is commonly recognized
    /// </summary>
    public bool IsCommon { get; set; } = true;
}