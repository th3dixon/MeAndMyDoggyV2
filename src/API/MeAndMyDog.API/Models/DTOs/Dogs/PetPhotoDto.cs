namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Data transfer object for pet photos
/// </summary>
public class PetPhotoDto
{
    /// <summary>
    /// Unique identifier for the photo
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Pet ID this photo belongs to
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
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Tags associated with the photo
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Photo category
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
    /// Upload timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}