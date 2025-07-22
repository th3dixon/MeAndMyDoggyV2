namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// File information DTO
/// </summary>
public class FileInfoDto
{
    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// File URL
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail URL (if applicable)
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Image width (if applicable)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Image height (if applicable)
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// When the file was uploaded
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }

    /// <summary>
    /// MD5 hash of the file
    /// </summary>
    public string? Hash { get; set; }
    
    /// <summary>
    /// File identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// File extension
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;
    
    /// <summary>
    /// Content type (alias for MimeType)
    /// </summary>
    public string ContentType
    {
        get => MimeType;
        set => MimeType = value;
    }
    
    /// <summary>
    /// File URL (alias for FileUrl)
    /// </summary>
    public string Url
    {
        get => FileUrl;
        set => FileUrl = value;
    }
    
    /// <summary>
    /// User who uploaded the file
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Upload type
    /// </summary>
    public string UploadType { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether file is publicly accessible
    /// </summary>
    public bool IsPublic { get; set; } = false;
    
    /// <summary>
    /// File description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// File tags
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Download count
    /// </summary>
    public int DownloadCount { get; set; } = 0;
    
    /// <summary>
    /// Whether file is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; } = false;
    
    /// <summary>
    /// Scan status
    /// </summary>
    public string ScanStatus { get; set; } = "pending";
    
    /// <summary>
    /// File metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}