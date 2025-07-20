namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Represents a file attachment to a message
/// </summary>
public class MessageAttachment
{
    /// <summary>
    /// Unique identifier for the attachment
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Foreign key to the message this attachment belongs to
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
    /// <summary>
    /// Original filename of the uploaded file
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// URL where the file is stored (blob storage)
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;
    /// <summary>
    /// MIME type of the file (image/jpeg, application/pdf, etc.)
    /// </summary>
    public string FileType { get; set; } = string.Empty;
    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }
    /// <summary>
    /// When the file was uploaded
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }
    
    /// <summary>
    /// Navigation property to the message
    /// </summary>
    public virtual Message Message { get; set; } = null!;
}