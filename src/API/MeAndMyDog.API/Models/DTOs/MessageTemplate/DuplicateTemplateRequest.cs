namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for template duplication
/// </summary>
public class DuplicateTemplateRequest
{
    /// <summary>
    /// Name for the new template
    /// </summary>
    public string NewName { get; set; } = string.Empty;
}