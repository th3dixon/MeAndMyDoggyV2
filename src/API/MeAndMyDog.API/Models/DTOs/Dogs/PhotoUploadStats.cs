namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Statistics about photo upload
/// </summary>
public class PhotoUploadStats
{
    /// <summary>
    /// Original file size in bytes
    /// </summary>
    public long OriginalFileSize { get; set; }
    
    /// <summary>
    /// Processed file size in bytes
    /// </summary>
    public long ProcessedFileSize { get; set; }
    
    /// <summary>
    /// Compression ratio
    /// </summary>
    public decimal CompressionRatio { get; set; }
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
    
    /// <summary>
    /// Number of thumbnails generated
    /// </summary>
    public int ThumbnailsGenerated { get; set; }
    
    /// <summary>
    /// Original image dimensions
    /// </summary>
    public string? OriginalDimensions { get; set; }
    
    /// <summary>
    /// Processed image dimensions
    /// </summary>
    public string? ProcessedDimensions { get; set; }
}