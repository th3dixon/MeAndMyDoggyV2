namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Response after configuring self-destruct
/// </summary>
public class SelfDestructResponse
{
    /// <summary>
    /// Self-destruct configuration
    /// </summary>
    public SelfDestructMessageDto SelfDestruct { get; set; } = null!;

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