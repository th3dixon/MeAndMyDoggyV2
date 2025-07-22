using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a user's saved location bookmark
/// </summary>
public class LocationBookmark
{
    /// <summary>
    /// Unique identifier for the bookmark
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User who owns this bookmark
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Name/label for the bookmark
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
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    /// <summary>
    /// Formatted address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Place name or business name
    /// </summary>
    [MaxLength(200)]
    public string? PlaceName { get; set; }

    /// <summary>
    /// Category of the location (home, work, vet, park, etc.)
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; } = "general";

    /// <summary>
    /// Icon identifier for the bookmark
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Color for the bookmark pin
    /// </summary>
    [MaxLength(7)]
    public string? Color { get; set; }

    /// <summary>
    /// Whether this bookmark is private or can be shared
    /// </summary>
    public bool IsPrivate { get; set; } = true;

    /// <summary>
    /// Whether this bookmark is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// How many times this bookmark has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// When the bookmark was last used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// When the bookmark was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the bookmark was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}