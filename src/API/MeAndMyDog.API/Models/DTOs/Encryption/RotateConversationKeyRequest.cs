namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for key rotation
/// </summary>
public class RotateConversationKeyRequest
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Reason for key rotation
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether to revoke old keys
    /// </summary>
    public bool RevokeOldKeys { get; set; } = true;
}