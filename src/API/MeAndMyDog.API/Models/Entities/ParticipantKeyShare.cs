using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity for managing per-participant key shares
/// </summary>
public class ParticipantKeyShare
{
    /// <summary>
    /// Unique identifier for the key share
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the conversation encryption key
    /// </summary>
    [Required]
    public string ConversationEncryptionKeyId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the conversation encryption key
    /// </summary>
    public virtual ConversationEncryptionKey ConversationEncryptionKey { get; set; } = null!;

    /// <summary>
    /// User ID of the participant
    /// </summary>
    [Required]
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the participant user
    /// </summary>
    public virtual ApplicationUser Participant { get; set; } = null!;

    /// <summary>
    /// Encrypted key share for this participant (encrypted with their public key)
    /// </summary>
    [Required]
    public string EncryptedKeyShare { get; set; } = string.Empty;

    /// <summary>
    /// Public key fingerprint used for encryption
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PublicKeyFingerprint { get; set; } = string.Empty;

    /// <summary>
    /// Whether this participant has acknowledged receiving the key
    /// </summary>
    public bool IsAcknowledged { get; set; }

    /// <summary>
    /// When the key share was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the participant acknowledged the key
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; set; }
}