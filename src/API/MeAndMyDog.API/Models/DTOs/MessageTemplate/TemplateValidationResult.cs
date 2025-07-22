namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Template validation result
/// </summary>
public class TemplateValidationResult
{
    /// <summary>
    /// Whether template is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Detected variables in template
    /// </summary>
    public List<string> DetectedVariables { get; set; } = new();
}