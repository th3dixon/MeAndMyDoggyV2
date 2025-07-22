namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for processing templates
/// </summary>
public class ProcessTemplateRequest
{
    /// <summary>
    /// Template content with variables
    /// </summary>
    public string TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Variable values
    /// </summary>
    public Dictionary<string, string>? Variables { get; set; }
}