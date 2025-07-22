namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for template processing
/// </summary>
public class ProcessTemplateResponse
{
    /// <summary>
    /// Whether processing was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Processed content with variables replaced
    /// </summary>
    public string ProcessedContent { get; set; } = string.Empty;

    /// <summary>
    /// Any processing messages
    /// </summary>
    public string? Message { get; set; }
}