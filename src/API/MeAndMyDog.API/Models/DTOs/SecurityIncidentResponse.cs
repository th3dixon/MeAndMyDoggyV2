namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after incident operation
/// </summary>
public class SecurityIncidentResponse
{
    /// <summary>
    /// Incident details
    /// </summary>
    public SecurityIncidentDto Incident { get; set; } = null!;

    /// <summary>
    /// Success indicator
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Success or error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Any validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();
}