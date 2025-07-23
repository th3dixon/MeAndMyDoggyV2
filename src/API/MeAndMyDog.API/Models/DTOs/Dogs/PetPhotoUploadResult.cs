namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Result of pet photo upload operation
/// </summary>
public class PetPhotoUploadResult
{
    /// <summary>
    /// Whether the upload was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if upload failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Photo details if upload succeeded
    /// </summary>
    public PetPhotoDto? Photo { get; set; }
    
    /// <summary>
    /// Upload statistics
    /// </summary>
    public PhotoUploadStats? Stats { get; set; }
}