using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Entity representing a saved search for messages
/// </summary>
public class MessageSearch
{
    /// <summary>
    /// Unique identifier for the search
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User who owns this search
    /// </summary>
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the saved search
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Search query text
    /// </summary>
    [MaxLength(500)]
    public string? Query { get; set; }

    /// <summary>
    /// Conversation ID filter (if searching within specific conversation)
    /// </summary>
    [MaxLength(450)]
    public string? ConversationId { get; set; }

    /// <summary>
    /// Sender ID filter
    /// </summary>
    [MaxLength(450)]
    public string? SenderId { get; set; }

    /// <summary>
    /// Message type filter (text, image, voice, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? MessageType { get; set; }

    /// <summary>
    /// Date range start for filtering
    /// </summary>
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>
    /// Date range end for filtering
    /// </summary>
    public DateTimeOffset? DateTo { get; set; }

    /// <summary>
    /// Include attachments in search
    /// </summary>
    public bool IncludeAttachments { get; set; } = true;

    /// <summary>
    /// Include voice messages in search
    /// </summary>
    public bool IncludeVoiceMessages { get; set; } = true;

    /// <summary>
    /// Include encrypted messages in search
    /// </summary>
    public bool IncludeEncryptedMessages { get; set; } = true;

    /// <summary>
    /// Tags associated with messages to search
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Whether this search is pinned for quick access
    /// </summary>
    public bool IsPinned { get; set; } = false;

    /// <summary>
    /// Whether this search is active/saved
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Number of times this search has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// When the search was last used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// When the search was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the search was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to conversation (if specific conversation search)
    /// </summary>
    [ForeignKey(nameof(ConversationId))]
    public Conversation? Conversation { get; set; }
}