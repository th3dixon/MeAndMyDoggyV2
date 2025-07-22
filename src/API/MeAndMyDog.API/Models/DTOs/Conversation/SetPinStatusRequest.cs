namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for setting pin status
/// </summary>
public class SetPinStatusRequest
{
    /// <summary>
    /// Whether to pin the conversation
    /// </summary>
    public bool IsPinned { get; set; }
}