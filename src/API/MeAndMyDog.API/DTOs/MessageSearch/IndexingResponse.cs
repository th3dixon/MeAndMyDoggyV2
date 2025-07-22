namespace MeAndMyDog.API.DTOs.MessageSearch;

/// <summary>
/// Response object for indexing operations
/// </summary>
public class IndexingResponse
{
    /// <summary>
    /// Whether indexing was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Indexing message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}