namespace MeAndMyDog.API.DTOs.Location;

/// <summary>
/// Response object for cleanup operations
/// </summary>
public class CleanupResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of items cleaned up
    /// </summary>
    public int CleanedUpCount { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}