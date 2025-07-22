namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request to update a saved search
/// </summary>
public class UpdateSavedSearchRequest
{
    /// <summary>
    /// Updated name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Updated search criteria
    /// </summary>
    public SearchMessageRequest? SearchCriteria { get; set; }

    /// <summary>
    /// Whether to pin/unpin the search
    /// </summary>
    public bool? IsPinned { get; set; }
}