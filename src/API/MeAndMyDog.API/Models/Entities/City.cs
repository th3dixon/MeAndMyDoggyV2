using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a city in the address lookup system
/// </summary>
[Table("Cities")]
public class City
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int CityId { get; set; }

    /// <summary>
    /// Foreign key to County
    /// </summary>
    public int CountyId { get; set; }

    /// <summary>
    /// City name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// Alternative city name or alias
    /// </summary>
    [StringLength(100)]
    public string? AlternativeName { get; set; }

    /// <summary>
    /// Latitude coordinate (optional for privacy/approximate locations)
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate (optional for privacy/approximate locations)
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// Population (optional)
    /// </summary>
    public int? Population { get; set; }

    /// <summary>
    /// Whether this city is active for lookups
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("CountyId")]
    public virtual County County { get; set; } = null!;
}