namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Recurrence pattern validation result
/// </summary>
public class RecurrenceValidationResult
{
    /// <summary>
    /// Whether pattern is valid
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
}