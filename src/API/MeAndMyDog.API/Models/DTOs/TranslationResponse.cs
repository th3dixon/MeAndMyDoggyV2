namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for translation operations
/// </summary>
public class TranslationResponse
{
    /// <summary>
    /// Whether the translation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Translation details
    /// </summary>
    public MessageTranslationDto? Translation { get; set; }

    /// <summary>
    /// Error message if translation failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Warning messages
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Alternative translations (if available)
    /// </summary>
    public List<string> Alternatives { get; set; } = new();
}