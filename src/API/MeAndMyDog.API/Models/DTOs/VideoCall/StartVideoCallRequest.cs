namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for initiating a video call
/// </summary>
public class StartVideoCallRequest
{
    /// <summary>
    /// ID of the conversation to start the call in
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;
    
    /// <summary>
    /// List of user IDs to invite to the call
    /// </summary>
    public List<string> ParticipantIds { get; set; } = new();
    
    /// <summary>
    /// Whether to record the call
    /// </summary>
    public bool RecordCall { get; set; } = false;
    
    /// <summary>
    /// Maximum number of participants allowed
    /// </summary>
    public int MaxParticipants { get; set; } = 2;
}