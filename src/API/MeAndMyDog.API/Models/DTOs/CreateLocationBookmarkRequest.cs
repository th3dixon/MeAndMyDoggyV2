using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a location bookmark
/// </summary>
public class CreateLocationBookmarkRequest
{
    /// <summary>
    /// Name for the bookmark
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// Formatted address (optional)
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Place name (optional)
    /// </summary>
    [MaxLength(200)]
    public string? PlaceName { get; set; }

    /// <summary>
    /// Bookmark category
    /// </summary>
    public LocationCategory Category { get; set; } = LocationCategory.General;

    /// <summary>
    /// Icon identifier
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Color for the bookmark (hex color)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether bookmark should be private
    /// </summary>
    public bool IsPrivate { get; set; } = true;
}