using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a postcode area for area-level lookups (e.g., 'SW', 'M', 'B')
/// </summary>
[Table("PostcodeAreas")]
public class PostcodeArea
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int PostcodeAreaId { get; set; }

    /// <summary>
    /// Postcode area code (e.g., 'SW', 'M', 'B')
    /// </summary>
    [Required]
    [StringLength(2)]
    [Column("PostcodeArea")]
    public string PostcodeAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable area name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string AreaName { get; set; } = string.Empty;

    /// <summary>
    /// Region this area belongs to
    /// </summary>
    [StringLength(50)]
    public string? Region { get; set; }

    /// <summary>
    /// Center latitude for the area
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? CenterLatitude { get; set; }

    /// <summary>
    /// Center longitude for the area
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? CenterLongitude { get; set; }
}