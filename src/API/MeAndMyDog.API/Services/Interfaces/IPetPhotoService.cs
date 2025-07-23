using MeAndMyDog.API.Models.DTOs.Dogs;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for pet photo management with advanced editing capabilities
/// </summary>
public interface IPetPhotoService
{
    /// <summary>
    /// Upload a pet photo with automatic thumbnail generation
    /// </summary>
    /// <param name="file">Photo file to upload</param>
    /// <param name="petId">Pet ID to associate photo with</param>
    /// <param name="userId">User uploading the photo</param>
    /// <param name="caption">Optional caption for the photo</param>
    /// <param name="category">Photo category (Profile, Health, Activity, etc.)</param>
    /// <param name="tags">Optional tags for the photo</param>
    /// <returns>Upload result with photo details</returns>
    Task<PetPhotoUploadResult> UploadPetPhotoAsync(
        IFormFile file, 
        string petId, 
        string userId, 
        string? caption = null, 
        string? category = null, 
        List<string>? tags = null);

    /// <summary>
    /// Upload and crop a pet photo
    /// </summary>
    /// <param name="file">Photo file to upload</param>
    /// <param name="petId">Pet ID to associate photo with</param>
    /// <param name="userId">User uploading the photo</param>
    /// <param name="cropArea">Crop area coordinates and dimensions</param>
    /// <param name="caption">Optional caption for the photo</param>
    /// <param name="category">Photo category</param>
    /// <param name="tags">Optional tags for the photo</param>
    /// <returns>Upload result with cropped photo details</returns>
    Task<PetPhotoUploadResult> UploadAndCropPetPhotoAsync(
        IFormFile file,
        string petId,
        string userId,
        PhotoCropArea cropArea,
        string? caption = null,
        string? category = null,
        List<string>? tags = null);

    /// <summary>
    /// Edit an existing pet photo (crop, resize, filters)
    /// </summary>
    /// <param name="photoId">Photo ID to edit</param>
    /// <param name="userId">User requesting the edit</param>
    /// <param name="editOptions">Photo editing options</param>
    /// <returns>Edit result with new photo details</returns>
    Task<PetPhotoEditResult> EditPetPhotoAsync(
        string photoId,
        string userId,
        PhotoEditOptions editOptions);

    /// <summary>
    /// Set a photo as the primary/profile photo for a pet
    /// </summary>
    /// <param name="photoId">Photo ID to set as primary</param>
    /// <param name="petId">Pet ID</param>
    /// <param name="userId">User making the change</param>
    /// <returns>Success result</returns>
    Task<bool> SetPrimaryPhotoAsync(string photoId, string petId, string userId);

    /// <summary>
    /// Update photo metadata (caption, tags, category)
    /// </summary>
    /// <param name="photoId">Photo ID to update</param>
    /// <param name="userId">User making the update</param>
    /// <param name="caption">New caption</param>
    /// <param name="category">New category</param>
    /// <param name="tags">New tags</param>
    /// <returns>Updated photo details</returns>
    Task<PetPhotoDto?> UpdatePhotoMetadataAsync(
        string photoId,
        string userId,
        string? caption = null,
        string? category = null,
        List<string>? tags = null);

    /// <summary>
    /// Delete a pet photo
    /// </summary>
    /// <param name="photoId">Photo ID to delete</param>
    /// <param name="userId">User requesting deletion</param>
    /// <returns>Success result</returns>
    Task<bool> DeletePetPhotoAsync(string photoId, string userId);

    /// <summary>
    /// Get all photos for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="userId">User requesting photos</param>
    /// <param name="category">Optional category filter</param>
    /// <returns>List of pet photos</returns>
    Task<List<PetPhotoDto>> GetPetPhotosAsync(string petId, string userId, string? category = null);

    /// <summary>
    /// Reorder pet photos
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="userId">User reordering photos</param>
    /// <param name="photoOrders">List of photo IDs in new order</param>
    /// <returns>Success result</returns>
    Task<bool> ReorderPhotosAsync(string petId, string userId, List<string> photoOrders);

    /// <summary>
    /// Generate optimized thumbnails for existing photos
    /// </summary>
    /// <param name="photoId">Photo ID to generate thumbnails for</param>
    /// <param name="userId">User requesting thumbnails</param>
    /// <param name="sizes">List of thumbnail sizes to generate</param>
    /// <returns>Dictionary of size to thumbnail URL mappings</returns>
    Task<Dictionary<string, string>> GenerateOptimizedThumbnailsAsync(
        string photoId,
        string userId,
        List<ThumbnailSize> sizes);
}