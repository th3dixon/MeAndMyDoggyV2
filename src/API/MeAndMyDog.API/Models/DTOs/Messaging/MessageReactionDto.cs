namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for message reactions
/// </summary>
public class MessageReactionDto
{
    /// <summary>
    /// Reaction emoji or identifier
    /// </summary>
    public string Reaction { get; set; } = string.Empty;

    /// <summary>
    /// Number of users who reacted with this reaction
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// List of user IDs who reacted
    /// </summary>
    public List<string> UserIds { get; set; } = new();
}