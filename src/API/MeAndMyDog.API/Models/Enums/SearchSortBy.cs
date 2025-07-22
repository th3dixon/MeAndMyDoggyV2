namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Sorting options for message search results
/// </summary>
public enum SearchSortBy
{
    /// <summary>
    /// Sort by relevance score (default)
    /// </summary>
    Relevance = 0,
    
    /// <summary>
    /// Sort by message date (newest first)
    /// </summary>
    DateDesc = 1,
    
    /// <summary>
    /// Sort by message date (oldest first)
    /// </summary>
    DateAsc = 2,
    
    /// <summary>
    /// Sort by sender name (alphabetical)
    /// </summary>
    SenderName = 3,
    
    /// <summary>
    /// Sort by conversation name
    /// </summary>
    ConversationName = 4,
    
    /// <summary>
    /// Sort by message type
    /// </summary>
    MessageType = 5,
    
    /// <summary>
    /// Sort by message length (longest first)
    /// </summary>
    LengthDesc = 6,
    
    /// <summary>
    /// Sort by message length (shortest first)
    /// </summary>
    LengthAsc = 7
}