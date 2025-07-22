using MeAndMyDog.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to translate arbitrary text
/// </summary>
public class TranslateTextRequest
{
    /// <summary>
    /// Text to translate
    /// </summary>
    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Target language code (ISO 639-1)
    /// </summary>
    [Required]
    [StringLength(10, MinimumLength = 2)]
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Source language code (optional, will auto-detect if not provided)
    /// </summary>
    [StringLength(10, MinimumLength = 2)]
    public string? SourceLanguage { get; set; }

    /// <summary>
    /// Preferred translation provider
    /// </summary>
    public TranslationProvider? PreferredProvider { get; set; }

    /// <summary>
    /// Whether to use cached translation if available
    /// </summary>
    public bool UseCache { get; set; } = true;

    /// <summary>
    /// Whether to cache the translation for future use
    /// </summary>
    public bool CacheResult { get; set; } = true;
}