namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Watermark configuration
/// </summary>
public class WatermarkDto
{
    /// <summary>
    /// Watermark text to display
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Watermark position
    /// </summary>
    public string Position { get; set; } = "bottom-right";

    /// <summary>
    /// Watermark opacity (0-1)
    /// </summary>
    public double Opacity { get; set; } = 0.3;

    /// <summary>
    /// Watermark color
    /// </summary>
    public string Color { get; set; } = "#888888";

    /// <summary>
    /// Font size for text watermark
    /// </summary>
    public int FontSize { get; set; } = 12;

    /// <summary>
    /// Whether watermark should be repeated
    /// </summary>
    public bool Repeat { get; set; } = false;

    /// <summary>
    /// Rotation angle in degrees
    /// </summary>
    public int Rotation { get; set; } = 0;
}