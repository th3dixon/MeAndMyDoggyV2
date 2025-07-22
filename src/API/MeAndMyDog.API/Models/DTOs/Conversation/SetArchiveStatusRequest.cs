namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for setting archive status
/// </summary>
public class SetArchiveStatusRequest
{
    /// <summary>
    /// Whether to archive the conversation
    /// </summary>
    public bool IsArchived { get; set; }
}