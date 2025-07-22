using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to rate translation quality
/// </summary>
public class RateTranslationRequest
{
    /// <summary>
    /// Translation ID to rate
    /// </summary>
    [Required]
    public string TranslationId { get; set; } = string.Empty;

    /// <summary>
    /// Quality rating (1-5 stars)
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>
    /// Optional feedback comment
    /// </summary>
    [StringLength(500)]
    public string? Feedback { get; set; }
}