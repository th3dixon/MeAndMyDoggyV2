namespace MeAndMyDog.API.Models.DTOs.Friends;

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