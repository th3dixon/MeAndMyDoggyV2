using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a county/state in the address lookup system
/// </summary>
[Table("Counties")]
public class County
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int CountyId { get; set; }

    /// <summary>
    /// Foreign key to Country
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// County name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CountyName { get; set; } = string.Empty;

    /// <summary>
    /// County code (optional)
    /// </summary>
    [StringLength(10)]
    public string? CountyCode { get; set; }

    /// <summary>
    /// Whether this county is active for lookups
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("CountryId")]
    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}