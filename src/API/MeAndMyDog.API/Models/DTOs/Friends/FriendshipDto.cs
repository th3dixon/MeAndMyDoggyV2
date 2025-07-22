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

/// <summary>
/// DTO for sending a friend request using friend code
/// </summary>
public class SendFriendRequestDto
{
    /// <summary>
    /// Friend code of the user to send friend request to (8-character alphanumeric)
    /// </summary>
    public string FriendCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional message to include with the friend request
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// DTO for responding to a friend request
/// </summary>
public class FriendRequestResponseDto
{
    /// <summary>
    /// ID of the friendship to respond to
    /// </summary>
    public Guid FriendshipId { get; set; }

    /// <summary>
    /// Whether to accept (true) or reject (false) the friend request
    /// </summary>
    public bool Accept { get; set; }
}

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

/// <summary>
/// Response DTO for friends list
/// </summary>
public class FriendsListResponse
{
    /// <summary>
    /// List of accepted friendships
    /// </summary>
    public List<FriendshipDto> Friends { get; set; } = new();

    /// <summary>
    /// List of pending friend requests sent to the current user
    /// </summary>
    public List<FriendshipDto> PendingRequests { get; set; } = new();

    /// <summary>
    /// List of friend requests sent by the current user
    /// </summary>
    public List<FriendshipDto> SentRequests { get; set; } = new();

    /// <summary>
    /// Total number of friends
    /// </summary>
    public int TotalFriends { get; set; }

    /// <summary>
    /// Total number of pending incoming requests
    /// </summary>
    public int PendingRequestsCount { get; set; }
}