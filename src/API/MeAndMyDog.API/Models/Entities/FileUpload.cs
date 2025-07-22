using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// File upload entity for tracking uploaded files
/// </summary>
[Table("FileUploads")]
public class FileUpload
{
    /// <summary>
    /// Unique file identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Original file name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File extension (including the dot)
    /// </summary>
    [MaxLength(10)]
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MD5 hash of the file content
    /// </summary>
    [MaxLength(32)]
    public string? FileHash { get; set; }

    /// <summary>
    /// File storage path or URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Public access URL
    /// </summary>
    [MaxLength(500)]
    public string? PublicUrl { get; set; }

    /// <summary>
    /// Thumbnail URL if applicable
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// User who uploaded the file
    /// </summary>
    [Required]
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// File upload type (message attachment, profile picture, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UploadType { get; set; } = string.Empty;

    /// <summary>
    /// Whether file is publicly accessible
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Whether file is temporarily stored (will be deleted after expiration)
    /// </summary>
    public bool IsTemporary { get; set; } = false;

    /// <summary>
    /// Expiration date for temporary files
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// File description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// File tags (JSON array)
    /// </summary>
    [MaxLength(2000)]
    public string? TagsJson { get; set; }

    /// <summary>
    /// File metadata (JSON object)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Number of times file has been downloaded
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// Last download timestamp
    /// </summary>
    public DateTimeOffset? LastDownloadedAt { get; set; }

    /// <summary>
    /// Whether file is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; } = false;

    /// <summary>
    /// Encryption key ID if encrypted
    /// </summary>
    public string? EncryptionKeyId { get; set; }

    /// <summary>
    /// File scan status (pending, scanning, clean, infected, error)
    /// </summary>
    [MaxLength(20)]
    public string ScanStatus { get; set; } = "pending";

    /// <summary>
    /// Scan result details (JSON)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ScanResultJson { get; set; }

    /// <summary>
    /// When file was scanned
    /// </summary>
    public DateTimeOffset? ScannedAt { get; set; }

    /// <summary>
    /// Whether file has been deleted (soft delete)
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// When file was deleted
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// User who deleted the file
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Storage provider used (local, azure, aws, etc.)
    /// </summary>
    [MaxLength(50)]
    public string StorageProvider { get; set; } = "local";

    /// <summary>
    /// Storage container/bucket name
    /// </summary>
    [MaxLength(100)]
    public string? StorageContainer { get; set; }

    /// <summary>
    /// Image dimensions if applicable (JSON: {width: 1920, height: 1080})
    /// </summary>
    [MaxLength(100)]
    public string? ImageDimensions { get; set; }

    /// <summary>
    /// Audio/video duration in seconds if applicable
    /// </summary>
    public double? DurationSeconds { get; set; }

    /// <summary>
    /// Related message ID if this is a message attachment
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Related conversation ID for easier querying
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// File processing status for files that require processing
    /// </summary>
    [MaxLength(20)]
    public string ProcessingStatus { get; set; } = "completed";

    /// <summary>
    /// Processing error details if any
    /// </summary>
    [MaxLength(1000)]
    public string? ProcessingError { get; set; }

    #region Navigation Properties

    /// <summary>
    /// User who uploaded the file
    /// </summary>
    [ForeignKey(nameof(UploadedBy))]
    public virtual ApplicationUser? Uploader { get; set; }

    /// <summary>
    /// Related message if this is an attachment
    /// </summary>
    [ForeignKey(nameof(MessageId))]
    public virtual Message? Message { get; set; }

    /// <summary>
    /// Related conversation for easier access
    /// </summary>
    [ForeignKey(nameof(ConversationId))]
    public virtual Conversation? Conversation { get; set; }

    #endregion
}