using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// DTO for location bookmarks
/// </summary>
public class LocationBookmarkDto
{
    /// <summary>
    /// Bookmark ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns the bookmark
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Bookmark name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Latitude coordinate
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Formatted address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Place name
    /// </summary>
    public string? PlaceName { get; set; }

    /// <summary>
    /// Bookmark category
    /// </summary>
    public LocationCategory Category { get; set; }

    /// <summary>
    /// Icon identifier
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Color for the bookmark
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Whether bookmark is private
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Usage count
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Last used timestamp
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Updated timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Distance from user's current location (if available)
    /// </summary>
    public double? DistanceFromUser { get; set; }
}
