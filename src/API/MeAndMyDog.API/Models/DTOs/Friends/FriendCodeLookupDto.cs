using MeAndMyDog.API.Models.DTOs.Auth;

namespace MeAndMyDog.API.Models.DTOs.Friends;

/// <summary>
/// DTO for friend code lookup result
/// </summary>
public class FriendCodeLookupDto
{
    /// <summary>
    /// Whether the friend code was found
    /// </summary>
    public bool Found { get; set; }

    /// <summary>
    /// User information if friend code was found
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Current friendship status with this user (null if no relationship)
    /// </summary>
    public string? FriendshipStatus { get; set; }

    /// <summary>
    /// Whether a friend request can be sent to this user
    /// </summary>
    public bool CanSendFriendRequest { get; set; }

    /// <summary>
    /// Whether this user can be messaged (i.e., are friends)
    /// </summary>
    public bool CanMessage { get; set; }

    /// <summary>
    /// Message explaining the current status
    /// </summary>
    public string Message { get; set; } = string.Empty;
}