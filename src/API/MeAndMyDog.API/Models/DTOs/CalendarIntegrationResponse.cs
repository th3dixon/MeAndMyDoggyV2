namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after calendar integration action
/// </summary>
public class CalendarIntegrationResponse
{
    /// <summary>
    /// Integration details
    /// </summary>
    public CalendarIntegrationDto Integration { get; set; } = null!;

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

    /// <summary>
    /// OAuth authorization URL (if needed)
    /// </summary>
    public string? AuthorizationUrl { get; set; }
}