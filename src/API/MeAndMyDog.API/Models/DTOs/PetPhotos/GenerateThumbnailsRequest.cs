using System.ComponentModel.DataAnnotations;
using MeAndMyDog.API.Models.DTOs.Dogs;

namespace MeAndMyDog.API.Models.DTOs.PetPhotos;

/// <summary>
/// Request model for generating thumbnails
/// </summary>
public class GenerateThumbnailsRequest
{
    /// <summary>
    /// List of thumbnail sizes to generate
    /// </summary>
    [Required]
    public List<ThumbnailSize> Sizes { get; set; } = new();
}