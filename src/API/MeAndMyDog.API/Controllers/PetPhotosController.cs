using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Models.DTOs.Dogs;
using MeAndMyDog.API.Models.DTOs.PetPhotos;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing pet photos with advanced editing capabilities
/// </summary>
[ApiController]
[Route("api/v1/pets/{petId}/photos")]
[Authorize]
public class PetPhotosController : ControllerBase
{
    private readonly IPetPhotoService _petPhotoService;
    private readonly ILogger<PetPhotosController> _logger;

    /// <summary>
    /// Initializes a new instance of PetPhotosController
    /// </summary>
    public PetPhotosController(IPetPhotoService petPhotoService, ILogger<PetPhotosController> logger)
    {
        _petPhotoService = petPhotoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all photos for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="category">Optional category filter</param>
    /// <returns>List of pet photos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PetPhotoDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPetPhotos(string petId, [FromQuery] string? category = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var photos = await _petPhotoService.GetPetPhotosAsync(petId, userId, category);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving photos for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while retrieving pet photos");
        }
    }

    /// <summary>
    /// Upload a photo for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="file">Photo file to upload</param>
    /// <param name="caption">Optional caption</param>
    /// <param name="category">Photo category</param>
    /// <param name="tags">Comma-separated tags</param>
    /// <returns>Upload result</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(PetPhotoUploadResult), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UploadPhoto(
        string petId,
        [Required] IFormFile file,
        [FromForm] string? caption = null,
        [FromForm] string? category = null,
        [FromForm] string? tags = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Parse tags if provided
            var tagList = string.IsNullOrEmpty(tags) 
                ? new List<string>() 
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(t => t.Trim())
                      .Where(t => !string.IsNullOrEmpty(t))
                      .ToList();

            var result = await _petPhotoService.UploadPetPhotoAsync(
                file, petId, userId, caption, category, tagList);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetPetPhotos), new { petId }, result);
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while uploading the photo");
        }
    }

    /// <summary>
    /// Upload and crop a photo for a pet
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="file">Photo file to upload</param>
    /// <param name="cropArea">Crop area details</param>
    /// <param name="caption">Optional caption</param>
    /// <param name="category">Photo category</param>
    /// <param name="tags">Comma-separated tags</param>
    /// <returns>Upload result with cropped photo</returns>
    [HttpPost("upload-and-crop")]
    [ProducesResponseType(typeof(PetPhotoUploadResult), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UploadAndCropPhoto(
        string petId,
        [Required] IFormFile file,
        [FromForm, Required] PhotoCropArea cropArea,
        [FromForm] string? caption = null,
        [FromForm] string? category = null,
        [FromForm] string? tags = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tagList = string.IsNullOrEmpty(tags) 
                ? new List<string>() 
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(t => t.Trim())
                      .Where(t => !string.IsNullOrEmpty(t))
                      .ToList();

            var result = await _petPhotoService.UploadAndCropPetPhotoAsync(
                file, petId, userId, cropArea, caption, category, tagList);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetPetPhotos), new { petId }, result);
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading and cropping photo for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while uploading and cropping the photo");
        }
    }

    /// <summary>
    /// Edit an existing pet photo
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="photoId">Photo ID to edit</param>
    /// <param name="editOptions">Edit options</param>
    /// <returns>Edit result</returns>
    [HttpPost("{photoId}/edit")]
    [ProducesResponseType(typeof(PetPhotoEditResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> EditPhoto(
        string petId,
        string photoId,
        [FromBody] PhotoEditOptions editOptions)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _petPhotoService.EditPetPhotoAsync(photoId, userId, editOptions);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing photo {PhotoId} for pet {PetId}", photoId, petId);
            return StatusCode(500, "An error occurred while editing the photo");
        }
    }

    /// <summary>
    /// Set a photo as the primary/profile photo
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="photoId">Photo ID to set as primary</param>
    /// <returns>Success result</returns>
    [HttpPost("{photoId}/set-primary")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetPrimaryPhoto(string petId, string photoId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _petPhotoService.SetPrimaryPhotoAsync(photoId, petId, userId);

            if (success)
            {
                return Ok(new { message = "Primary photo updated successfully" });
            }

            return NotFound("Photo not found or access denied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary photo {PhotoId} for pet {PetId}", photoId, petId);
            return StatusCode(500, "An error occurred while setting the primary photo");
        }
    }

    /// <summary>
    /// Update photo metadata
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="photoId">Photo ID to update</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated photo details</returns>
    [HttpPut("{photoId}/metadata")]
    [ProducesResponseType(typeof(PetPhotoDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePhotoMetadata(
        string petId,
        string photoId,
        [FromBody] UpdatePhotoMetadataRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var photo = await _petPhotoService.UpdatePhotoMetadataAsync(
                photoId, userId, request.Caption, request.Category, request.Tags);

            if (photo != null)
            {
                return Ok(photo);
            }

            return NotFound("Photo not found or access denied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata for photo {PhotoId}", photoId);
            return StatusCode(500, "An error occurred while updating photo metadata");
        }
    }

    /// <summary>
    /// Delete a pet photo
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="photoId">Photo ID to delete</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{photoId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeletePhoto(string petId, string photoId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _petPhotoService.DeletePetPhotoAsync(photoId, userId);

            if (success)
            {
                return NoContent();
            }

            return NotFound("Photo not found or access denied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting photo {PhotoId} for pet {PetId}", photoId, petId);
            return StatusCode(500, "An error occurred while deleting the photo");
        }
    }

    /// <summary>
    /// Reorder pet photos
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="request">Reorder request with photo IDs in new order</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("reorder")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ReorderPhotos(
        string petId,
        [FromBody] ReorderPhotosRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _petPhotoService.ReorderPhotosAsync(petId, userId, request.PhotoIds);

            if (success)
            {
                return Ok(new { message = "Photos reordered successfully" });
            }

            return NotFound("Pet not found or access denied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering photos for pet {PetId}", petId);
            return StatusCode(500, "An error occurred while reordering photos");
        }
    }

    /// <summary>
    /// Generate optimized thumbnails for a photo
    /// </summary>
    /// <param name="petId">Pet ID</param>
    /// <param name="photoId">Photo ID</param>
    /// <param name="request">Thumbnail generation request</param>
    /// <returns>Thumbnail URLs</returns>
    [HttpPost("{photoId}/thumbnails")]
    [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GenerateThumbnails(
        string petId,
        string photoId,
        [FromBody] GenerateThumbnailsRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var thumbnails = await _petPhotoService.GenerateOptimizedThumbnailsAsync(
                photoId, userId, request.Sizes);

            return Ok(new { thumbnails });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating thumbnails for photo {PhotoId}", photoId);
            return StatusCode(500, "An error occurred while generating thumbnails");
        }
    }
}