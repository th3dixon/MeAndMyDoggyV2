using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a postcode sector for sector-level lookups (e.g., 'SW1A 1')
/// </summary>
[Table("PostcodeSectors")]
public class PostcodeSector
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int PostcodeSectorId { get; set; }

    /// <summary>
    /// Full sector code (e.g., 'SW1A 1')
    /// </summary>
    [Required]
    [StringLength(6)]
    [Column("PostcodeSector")]
    public string PostcodeSectorCode { get; set; } = string.Empty;

    /// <summary>
    /// Postcode area (e.g., 'SW')
    /// </summary>
    [Required]
    [StringLength(2)]
    public string PostcodeArea { get; set; } = string.Empty;

    /// <summary>
    /// Postcode district (e.g., 'SW1')
    /// </summary>
    [Required]
    [StringLength(4)]
    public string PostcodeDistrict { get; set; } = string.Empty;

    /// <summary>
    /// Sector number/letter (e.g., '1')
    /// </summary>
    [Required]
    [StringLength(2)]
    public string SectorCode { get; set; } = string.Empty;

    /// <summary>
    /// Center latitude for the sector
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? CenterLatitude { get; set; }

    /// <summary>
    /// Center longitude for the sector
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal? CenterLongitude { get; set; }

    /// <summary>
    /// Approximate postcode count in this sector
    /// </summary>
    public int? PostcodeCount { get; set; }
}