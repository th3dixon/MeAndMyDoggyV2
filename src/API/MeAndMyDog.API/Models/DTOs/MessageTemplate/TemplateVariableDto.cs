using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for template variables
/// </summary>
public class TemplateVariableDto
{
    /// <summary>
    /// Variable name/key
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Variable display label
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Variable type (text, number, date, etc.)
    /// </summary>
    public string Type { get; set; } = "text";

    /// <summary>
    /// Default value for the variable
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Whether variable is required
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Variable description/help text
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Validation pattern (regex)
    /// </summary>
    public string? ValidationPattern { get; set; }

    /// <summary>
    /// Possible values for dropdown/select variables
    /// </summary>
    public List<string>? PossibleValues { get; set; }
}