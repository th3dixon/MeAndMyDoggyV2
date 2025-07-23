namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Options for resizing photos
/// </summary>
public class PhotoResizeOptions
{
    /// <summary>
    /// Target width in pixels
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Target height in pixels
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Resize mode (Crop, Pad, Stretch, Max)
    /// </summary>
    public string ResizeMode { get; set; } = "Max";
    
    /// <summary>
    /// Whether to maintain aspect ratio
    /// </summary>
    public bool MaintainAspectRatio { get; set; } = true;
    
    /// <summary>
    /// Background color for padding (hex color)
    /// </summary>
    public string? BackgroundColor { get; set; } = "#FFFFFF";
}