using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a country in the address lookup system
/// </summary>
[Table("Countries")]
public class Country
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int CountryId { get; set; }

    /// <summary>
    /// ISO country code (e.g., 'GB', 'US')
    /// </summary>
    [Required]
    [StringLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Full country name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this country is active for lookups
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<County> Counties { get; set; } = new List<County>();
}