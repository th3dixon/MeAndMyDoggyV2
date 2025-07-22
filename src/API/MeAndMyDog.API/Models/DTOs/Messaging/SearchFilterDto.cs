namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Search filter options
/// </summary>
public class SearchFilterDto
{
    /// <summary>
    /// Filter by file attachments
    /// </summary>
    public bool? HasAttachments { get; set; }

    /// <summary>
    /// Filter by specific attachment types
    /// </summary>
    public List<string>? AttachmentTypes { get; set; }

    /// <summary>
    /// Filter by messages with reactions
    /// </summary>
    public bool? HasReactions { get; set; }

    /// <summary>
    /// Filter by forwarded messages
    /// </summary>
    public bool? IsForwarded { get; set; }

    /// <summary>
    /// Filter by reply messages
    /// </summary>
    public bool? IsReply { get; set; }

    /// <summary>
    /// Filter by edited messages
    /// </summary>
    public bool? IsEdited { get; set; }

    /// <summary>
    /// Filter by minimum message length
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Filter by maximum message length
    /// </summary>
    public int? MaxLength { get; set; }
}