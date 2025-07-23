namespace MeAndMyDog.API.Models.DTOs.Dogs;

/// <summary>
/// Thumbnail size options
/// </summary>
public class ThumbnailSize
{
    /// <summary>
    /// Size name (e.g., "small", "medium", "large")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Width in pixels
    /// </summary>
    public int Width { get; set; }
    
    /// <summary>
    /// Height in pixels
    /// </summary>
    public int Height { get; set; }
    
    /// <summary>
    /// Quality (1-100)
    /// </summary>
    public int Quality { get; set; } = 80;
}