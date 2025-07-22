namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for template validation
/// </summary>
public class ValidateTemplateRequest
{
    /// <summary>
    /// Template content to validate
    /// </summary>
    public string TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Template variables
    /// </summary>
    public List<TemplateVariableDto>? Variables { get; set; }
}