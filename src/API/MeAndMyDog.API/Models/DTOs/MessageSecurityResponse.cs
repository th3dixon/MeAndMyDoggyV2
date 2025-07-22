namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after configuring message security
/// </summary>
public class MessageSecurityResponse
{
    /// <summary>
    /// Security configuration
    /// </summary>
    public MessageSecurityDto Security { get; set; } = null!;

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