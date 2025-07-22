namespace MeAndMyDog.API.DTOs.MessageSearch;

/// <summary>
/// Response object for clearing search history
/// </summary>
public class ClearHistoryResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of entries cleared
    /// </summary>
    public int ClearedCount { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}