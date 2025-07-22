namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for saved search operations
/// </summary>
public class SavedSearchResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The saved search (on success)
    /// </summary>
    public SavedSearchDto? SavedSearch { get; set; }

    /// <summary>
    /// Error message (on failure)
    /// </summary>
    public string? Message { get; set; }
}