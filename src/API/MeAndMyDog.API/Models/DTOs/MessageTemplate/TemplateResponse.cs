using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for template operations
/// </summary>
public class TemplateResponse
{
    /// <summary>
    /// Whether operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Template ID if created/updated
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// Template data
    /// </summary>
    public MessageTemplateDto? Template { get; set; }
}