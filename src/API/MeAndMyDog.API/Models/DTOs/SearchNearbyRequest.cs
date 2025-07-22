using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to search nearby locations
/// </summary>
public class SearchNearbyRequest
{
    /// <summary>
    /// Center latitude for search
    /// </summary>
    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// Center longitude for search
    /// </summary>
    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// Search radius in meters
    /// </summary>
    [Range(100, 50000, ErrorMessage = "Radius must be between 100 and 50,000 meters")]
    public double RadiusMeters { get; set; } = 1000;

    /// <summary>
    /// Category filter
    /// </summary>
    public LocationCategory? Category { get; set; }

    /// <summary>
    /// Search query text
    /// </summary>
    [MaxLength(100)]
    public string? Query { get; set; }

    /// <summary>
    /// Maximum results to return
    /// </summary>
    [Range(1, 50)]
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Include user's private bookmarks
    /// </summary>
    public bool IncludePrivate { get; set; } = true;

    /// <summary>
    /// Include public/shared bookmarks
    /// </summary>
    public bool IncludePublic { get; set; } = true;
}