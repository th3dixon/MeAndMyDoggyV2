using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for message attachments
/// </summary>
public class MessageAttachmentDto
{
    /// <summary>
    /// Attachment unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// URL to access the file
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL to thumbnail (for images/videos)
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Type of attachment (Image, Document, Audio, Video)
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
}