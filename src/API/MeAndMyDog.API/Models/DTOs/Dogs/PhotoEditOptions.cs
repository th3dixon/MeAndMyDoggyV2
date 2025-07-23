namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Options for editing pet photos
/// </summary>
public class PhotoEditOptions
{
    /// <summary>
    /// Crop area (if cropping is requested)
    /// </summary>
    public PhotoCropArea? CropArea { get; set; }
    
    /// <summary>
    /// Resize dimensions (if resizing is requested)
    /// </summary>
    public PhotoResizeOptions? ResizeOptions { get; set; }
    
    /// <summary>
    /// Rotation angle in degrees (0, 90, 180, 270)
    /// </summary>
    public int? RotationAngle { get; set; }
    
    /// <summary>
    /// Whether to flip horizontally
    /// </summary>
    public bool FlipHorizontal { get; set; }
    
    /// <summary>
    /// Whether to flip vertically
    /// </summary>
    public bool FlipVertical { get; set; }
    
    /// <summary>
    /// Brightness adjustment (-100 to 100)
    /// </summary>
    public int? Brightness { get; set; }
    
    /// <summary>
    /// Contrast adjustment (-100 to 100)
    /// </summary>
    public int? Contrast { get; set; }
    
    /// <summary>
    /// Saturation adjustment (-100 to 100)
    /// </summary>
    public int? Saturation { get; set; }
    
    /// <summary>
    /// Apply blur effect (0-10)
    /// </summary>
    public float? BlurRadius { get; set; }
    
    /// <summary>
    /// Apply sharpen effect (0-10)
    /// </summary>
    public float? SharpenAmount { get; set; }
    
    /// <summary>
    /// Apply grayscale filter
    /// </summary>
    public bool ApplyGrayscale { get; set; }
    
    /// <summary>
    /// Apply sepia filter
    /// </summary>
    public bool ApplySepia { get; set; }
    
    /// <summary>
    /// Output quality (1-100)
    /// </summary>
    public int Quality { get; set; } = 85;
    
    /// <summary>
    /// Output format (jpeg, png, webp)
    /// </summary>
    public string OutputFormat { get; set; } = "jpeg";
}