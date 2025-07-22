using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a UK postcode in the address lookup system
/// </summary>
[Table("Postcodes")]
public class Postcode
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int PostcodeId { get; set; }

    /// <summary>
    /// Postcode without spaces (e.g., 'SW1A1AA')
    /// </summary>
    [Required]
    [StringLength(8)]
    [Column("Postcode")]
    public string PostcodeCode { get; set; } = string.Empty;

    /// <summary>
    /// Formatted postcode with space (e.g., 'SW1A 1AA')
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("PostcodeFormatted")]
    public string PostcodeFormatted { get; set; } = string.Empty;

    /// <summary>
    /// Outward code - first part (e.g., 'SW1A')
    /// </summary>
    [Required]
    [StringLength(4)]
    public string OutwardCode { get; set; } = string.Empty;

    /// <summary>
    /// Inward code - second part (e.g., '1AA')
    /// </summary>
    [Required]
    [StringLength(3)]
    public string InwardCode { get; set; } = string.Empty;

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
    /// Postcode sector (e.g., 'SW1A 1')
    /// </summary>
    [Required]
    [StringLength(6)]
    public string PostcodeSector { get; set; } = string.Empty;

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    [Column(TypeName = "decimal(9,6)")]
    public decimal Longitude { get; set; }

    /// <summary>
    /// Easting coordinate (British National Grid)
    /// </summary>
    public int? Easting { get; set; }

    /// <summary>
    /// Northing coordinate (British National Grid)
    /// </summary>
    public int? Northing { get; set; }

    /// <summary>
    /// Grid reference
    /// </summary>
    [StringLength(10)]
    public string? GridReference { get; set; }

    /// <summary>
    /// Whether this postcode is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when postcode was introduced
    /// </summary>
    public DateTime? DateIntroduced { get; set; }

    /// <summary>
    /// Date when postcode was terminated
    /// </summary>
    public DateTime? DateTerminated { get; set; }
}