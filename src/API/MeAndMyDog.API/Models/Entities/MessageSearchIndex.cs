using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeAndMyDog.API.Models.Entities;

/// <summary>
/// Search index for messages to enable fast full-text search
/// </summary>
[Table("MessageSearchIndex")]
public class MessageSearchIndex
{
    /// <summary>
    /// Unique index entry identifier
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Message ID being indexed
    /// </summary>
    [Required]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Conversation ID for filtering
    /// </summary>
    [Required]
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Message sender ID
    /// </summary>
    [Required]
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Searchable content (preprocessed)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string SearchableContent { get; set; } = string.Empty;

    /// <summary>
    /// Keywords extracted from content
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string Keywords { get; set; } = string.Empty;

    /// <summary>
    /// Content language (for language-specific search)
    /// </summary>
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Message type for filtering
    /// </summary>
    [MaxLength(50)]
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Whether message has attachments
    /// </summary>
    public bool HasAttachments { get; set; }

    /// <summary>
    /// Attachment file names for searching
    /// </summary>
    [MaxLength(2000)]
    public string? AttachmentNames { get; set; }

    /// <summary>
    /// Message timestamp for sorting
    /// </summary>
    public DateTimeOffset MessageTimestamp { get; set; }

    /// <summary>
    /// When this index entry was created
    /// </summary>
    public DateTimeOffset IndexedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When this index entry was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Search relevance boost factor
    /// </summary>
    public double RelevanceBoost { get; set; } = 1.0;

    /// <summary>
    /// Whether message is deleted (for cleanup)
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    #region Navigation Properties

    /// <summary>
    /// Reference to the original message
    /// </summary>
    [ForeignKey(nameof(MessageId))]
    public virtual Message? Message { get; set; }

    /// <summary>
    /// Reference to the conversation
    /// </summary>
    [ForeignKey(nameof(ConversationId))]
    public virtual Conversation? Conversation { get; set; }

    /// <summary>
    /// Reference to the sender
    /// </summary>
    [ForeignKey(nameof(SenderId))]
    public virtual ApplicationUser? Sender { get; set; }

    #endregion
}