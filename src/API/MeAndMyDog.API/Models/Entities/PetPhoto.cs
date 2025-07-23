namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a photo of a pet with metadata and editing capabilities
/// </summary>
public class PetPhoto
{
    /// <summary>
    /// Unique identifier for the photo
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Foreign key to the pet
    /// </summary>
    public string PetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to the photo file
    /// </summary>
    public string PhotoUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to thumbnail version
    /// </summary>
    public string? ThumbnailUrl { get; set; }
    
    /// <summary>
    /// Photo caption or description
    /// </summary>
    public string? Caption { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    
    /// <summary>
    /// Image width in pixels
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Image height in pixels
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Whether this is the primary/profile photo
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Display order for sorting photos
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Tags associated with the photo (JSON array)
    /// </summary>
    public string? Tags { get; set; }
    
    /// <summary>
    /// Photo category (Profile, Health, Activity, etc.)
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Date the photo was taken
    /// </summary>
    public DateTimeOffset? DateTaken { get; set; }
    
    /// <summary>
    /// Location where photo was taken
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Whether the photo is active/visible
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the photo was uploaded
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the photo was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the pet
    /// </summary>
    public virtual DogProfile Pet { get; set; } = null!;
}