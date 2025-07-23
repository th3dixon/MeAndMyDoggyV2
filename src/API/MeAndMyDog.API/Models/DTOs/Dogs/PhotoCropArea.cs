using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Represents a crop area for photo editing
/// </summary>
public class PhotoCropArea
{
    /// <summary>
    /// X coordinate of the crop area (in pixels)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "X coordinate must be non-negative")]
    public int X { get; set; }
    
    /// <summary>
    /// Y coordinate of the crop area (in pixels)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Y coordinate must be non-negative")]
    public int Y { get; set; }
    
    /// <summary>
    /// Width of the crop area (in pixels)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Width must be positive")]
    public int Width { get; set; }
    
    /// <summary>
    /// Height of the crop area (in pixels)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Height must be positive")]
    public int Height { get; set; }
    
    /// <summary>
    /// Quality of the output image (1-100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Quality must be between 1 and 100")]
    public int Quality { get; set; } = 85;
    
    /// <summary>
    /// Output format (jpeg, png, webp)
    /// </summary>
    public string OutputFormat { get; set; } = "jpeg";
    
    /// <summary>
    /// Whether to maintain aspect ratio
    /// </summary>
    public bool MaintainAspectRatio { get; set; } = true;
}