namespace MeAndMyDog.API.Models.DTOs.PetPhotos;

/// <summary>
/// Request model for updating photo metadata
/// </summary>
public class UpdatePhotoMetadataRequest
{
    /// <summary>
    /// New caption for the photo
    /// </summary>
    public string? Caption { get; set; }
    
    /// <summary>
    /// New category for the photo
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// New tags for the photo
    /// </summary>
    public List<string>? Tags { get; set; }
}