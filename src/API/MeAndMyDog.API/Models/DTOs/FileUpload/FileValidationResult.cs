using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Result of file validation
/// </summary>
public class FileValidationResult
{
    /// <summary>
    /// Whether file is valid for upload
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// Detected attachment type
    /// </summary>
    public AttachmentType AttachmentType { get; set; }
    
    /// <summary>
    /// Whether file requires premium subscription
    /// </summary>
    public bool RequiresPremium { get; set; }
    
    /// <summary>
    /// Validation warnings (non-blocking issues)
    /// </summary>
    public List<string> Warnings { get; set; } = new();
    
    /// <summary>
    /// Detected file type as string
    /// </summary>
    public string? DetectedFileType { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
}