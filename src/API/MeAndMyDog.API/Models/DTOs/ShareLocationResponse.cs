namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response for location sharing
/// </summary>
public class ShareLocationResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The shared location details
    /// </summary>
    public LocationShareDto? LocationShare { get; set; }

    /// <summary>
    /// The message that was created
    /// </summary>
    public MessageDto? Message { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? Error { get; set; }
}