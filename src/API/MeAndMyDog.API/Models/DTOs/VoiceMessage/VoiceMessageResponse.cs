namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response object for voice message operations
/// </summary>
public class VoiceMessageResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Voice message data
    /// </summary>
    public VoiceMessageDto? VoiceMessage { get; set; }

    /// <summary>
    /// Upload progress (0-100)
    /// </summary>
    public int? UploadProgress { get; set; }

    /// <summary>
    /// Processing progress (0-100)
    /// </summary>
    public int? ProcessingProgress { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    public object? Data { get; set; }
}