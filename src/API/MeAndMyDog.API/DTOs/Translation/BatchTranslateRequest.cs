namespace MeAndMyDog.API.DTOs.Translation;

/// <summary>
/// Request for batch translation
/// </summary>
public class BatchTranslateRequest
{
    /// <summary>
    /// List of message IDs to translate
    /// </summary>
    public List<string> MessageIds { get; set; } = new();

    /// <summary>
    /// Target language for all translations
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Source language (optional, will auto-detect if not provided)
    /// </summary>
    public string? SourceLanguage { get; set; }
}