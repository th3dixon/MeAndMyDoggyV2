namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity for tracking file uploads
/// </summary>
public class FileUploadRecord
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User who uploaded the file
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Message this file is attached to (if any)
    /// </summary>
    public string? MessageId { get; set; }
    
    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique filename on server
    /// </summary>
    public string UniqueFileName { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to access the file
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to thumbnail (if applicable)
    /// </summary>
    public string? ThumbnailUrl { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// MIME type
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    
    /// <summary>
    /// Content type (alias for MimeType)
    /// </summary>
    public string ContentType
    {
        get => MimeType;
        set => MimeType = value;
    }
    
    /// <summary>
    /// Type of attachment (Image, Video, Audio, Document)
    /// </summary>
    public string AttachmentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Image/video width (if applicable)
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Image/video height (if applicable)
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// When uploaded
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }
    
    /// <summary>
    /// Whether file is processed (for images/videos)
    /// </summary>
    public bool IsProcessed { get; set; }
    
    /// <summary>
    /// File processing status
    /// </summary>
    public string? ProcessingStatus { get; set; }
    
    /// <summary>
    /// MD5 hash of the file
    /// </summary>
    public string? FileHash { get; set; }
    
    /// <summary>
    /// User who uploaded the file (alias for UserId)
    /// </summary>
    public string UploadedBy
    {
        get => UserId;
        set => UserId = value;
    }
    
    /// <summary>
    /// Whether the file record is deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>
    /// Public URL for file access (alias for FileUrl)
    /// </summary>
    public string PublicUrl
    {
        get => FileUrl;
        set => FileUrl = value;
    }
    
    /// <summary>
    /// File tags as JSON string
    /// </summary>
    public string? TagsJson { get; set; }
    
    /// <summary>
    /// File metadata as JSON string
    /// </summary>
    public string? MetadataJson { get; set; }
    
    /// <summary>
    /// File scan status (pending, scanning, clean, infected, error)
    /// </summary>
    public string ScanStatus { get; set; } = "pending";
    
    /// <summary>
    /// Scan result details (JSON)
    /// </summary>
    public string? ScanResultJson { get; set; }
    
    /// <summary>
    /// When file was scanned
    /// </summary>
    public DateTimeOffset? ScannedAt { get; set; }
    
    /// <summary>
    /// When file was deleted
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
    
    /// <summary>
    /// User who deleted the file
    /// </summary>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// File path for physical file location
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// File extension
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;
    
    /// <summary>
    /// Upload type (alias for AttachmentType)
    /// </summary>
    public string UploadType
    {
        get => AttachmentType;
        set => AttachmentType = value;
    }
    
    /// <summary>
    /// Whether file is publicly accessible
    /// </summary>
    public bool IsPublic { get; set; } = false;
    
    /// <summary>
    /// File description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Number of times file has been downloaded
    /// </summary>
    public int DownloadCount { get; set; } = 0;
    
    /// <summary>
    /// Whether file is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; } = false;
}