using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update a location bookmark
/// </summary>
public class UpdateLocationBookmarkRequest
{
    /// <summary>
    /// Updated name
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Updated description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Updated latitude
    /// </summary>
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Updated longitude
    /// </summary>
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Updated address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Updated place name
    /// </summary>
    [MaxLength(200)]
    public string? PlaceName { get; set; }

    /// <summary>
    /// Updated category
    /// </summary>
    public LocationCategory? Category { get; set; }

    /// <summary>
    /// Updated icon
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Updated color
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color")]
    public string? Color { get; set; }

    /// <summary>
    /// Updated privacy setting
    /// </summary>
    public bool? IsPrivate { get; set; }
}