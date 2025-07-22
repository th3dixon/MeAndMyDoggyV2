namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Template usage information
/// </summary>
public class TemplateUsageDto
{
    /// <summary>
    /// Template ID
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Template name
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Usage count
    /// </summary>
    public int UsageCount { get; set; }
}