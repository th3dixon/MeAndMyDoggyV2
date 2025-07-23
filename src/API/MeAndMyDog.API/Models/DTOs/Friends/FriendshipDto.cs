using MeAndMyDog.API.Models.DTOs.Auth;

namespace MeAndMyDog.API.Models.DTOs.Friends;

/// <summary>
/// DTO for friendship information
/// </summary>
public class FriendshipDto
{
    /// <summary>
    /// Unique identifier for the friendship
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The friend user information
    /// </summary>
    public UserDto Friend { get; set; } = null!;

    /// <summary>
    /// Current status of the friendship (Pending, Accepted, Blocked)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// When the friendship was requested
    /// </summary>
    public DateTime RequestedAt { get; set; }

    /// <summary>
    /// When the friendship was accepted (null if not accepted)
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    /// <summary>
    /// Whether the current user sent the friend request
    /// </summary>
    public bool IsSentByMe { get; set; }

    /// <summary>
    /// Optional note from the friend request
    /// </summary>
    public string? RequestNote { get; set; }

    /// <summary>
    /// Whether the friend is currently online (if available)
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Last seen time (if available and friends)
    /// </summary>
    public DateTime? LastSeen { get; set; }
}