namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to create a saved search
/// </summary>
public class CreateSavedSearchRequest
{
    /// <summary>
    /// Name for the saved search
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Search criteria to save
    /// </summary>
    public SearchMessageRequest SearchCriteria { get; set; } = new();
}