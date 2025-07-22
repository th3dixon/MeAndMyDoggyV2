namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for language preference operations
/// </summary>
public class LanguagePreferenceResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Language preference details
    /// </summary>
    public LanguagePreferenceDto? Preference { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Validation messages
    /// </summary>
    public List<string> Messages { get; set; } = new();
}