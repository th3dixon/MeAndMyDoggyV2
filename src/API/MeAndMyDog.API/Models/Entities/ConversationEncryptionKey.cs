using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity for managing conversation-level encryption keys
/// </summary>
public class ConversationEncryptionKey
{
    /// <summary>
    /// Unique identifier for the encryption key
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the conversation this key belongs to
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the conversation
    /// </summary>
    public virtual Conversation Conversation { get; set; } = null!;

    /// <summary>
    /// User ID of the key owner/generator
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Key identifier for referencing in messages
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// Encrypted symmetric key (encrypted with participant's public keys)
    /// </summary>
    [Required]
    public string EncryptedKey { get; set; } = string.Empty;

    /// <summary>
    /// Key derivation salt (base64 encoded)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string KeySalt { get; set; } = string.Empty;

    /// <summary>
    /// Key version for rotation management
    /// </summary>
    public int KeyVersion { get; set; } = 1;

    /// <summary>
    /// Whether this key is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the key expires (null for no expiration)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// When the key was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the key was last used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// Collection of participant key shares
    /// </summary>
    public virtual ICollection<ParticipantKeyShare> ParticipantKeyShares { get; set; } = new List<ParticipantKeyShare>();
}