namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Result of pet photo edit operation
/// </summary>
public class PetPhotoEditResult
{
    /// <summary>
    /// Whether the edit was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if edit failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Original photo details
    /// </summary>
    public PetPhotoDto? OriginalPhoto { get; set; }
    
    /// <summary>
    /// Edited photo details
    /// </summary>
    public PetPhotoDto? EditedPhoto { get; set; }
    
    /// <summary>
    /// Edit processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
}