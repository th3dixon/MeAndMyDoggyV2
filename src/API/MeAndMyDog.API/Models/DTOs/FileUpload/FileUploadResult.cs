using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Result of file upload operation
/// </summary>
public class FileUploadResult
{
    /// <summary>
    /// Whether upload was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if upload failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// URL to access the uploaded file
    /// </summary>
    public string? FileUrl { get; set; }
    
    /// <summary>
    /// URL to thumbnail (for images/videos)
    /// </summary>
    public string? ThumbnailUrl { get; set; }
    
    /// <summary>
    /// Original filename
    /// </summary>
    public string? FileName { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string? MimeType { get; set; }
    
    /// <summary>
    /// Type of attachment
    /// </summary>
    public AttachmentType AttachmentType { get; set; }
    
    /// <summary>
    /// Image width (for images)
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Image height (for images)
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Duration in seconds (for audio/video)
    /// </summary>
    public int? Duration { get; set; }
    
    /// <summary>
    /// Unique file identifier
    /// </summary>
    public string? FileId { get; set; }
    
    /// <summary>
    /// File information details
    /// </summary>
    public FileInfoDto? FileInfo { get; set; }
    
    /// <summary>
    /// Upload URL for direct access
    /// </summary>
    public string? UploadUrl { get; set; }
    
    /// <summary>
    /// File processing status
    /// </summary>
    public string? ProcessingStatus { get; set; }
}