namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Participant activity information
/// </summary>
public class ParticipantActivity
{
    /// <summary>
    /// Participant user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Participant display name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Number of messages sent
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Percentage of total conversation messages
    /// </summary>
    public double MessagePercentage { get; set; }

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTimeOffset? LastActivityAt { get; set; }
}