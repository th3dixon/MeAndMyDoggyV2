using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.PetPhotos;

/// <summary>
/// Request model for reordering photos
/// </summary>
public class ReorderPhotosRequest
{
    /// <summary>
    /// Photo IDs in the new order
    /// </summary>
    [Required]
    public List<string> PhotoIds { get; set; } = new();
}