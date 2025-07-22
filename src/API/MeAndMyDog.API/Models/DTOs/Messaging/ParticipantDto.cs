using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Data transfer object for conversation participants
/// </summary>
public class ParticipantDto
{
    /// <summary>
    /// User ID of the participant
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the participant
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Role of the participant in the conversation
    /// </summary>
    public ConversationRole Role { get; set; }

    /// <summary>
    /// When the participant joined the conversation
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }

    /// <summary>
    /// When the participant last read messages
    /// </summary>
    public DateTimeOffset? LastReadAt { get; set; }
}