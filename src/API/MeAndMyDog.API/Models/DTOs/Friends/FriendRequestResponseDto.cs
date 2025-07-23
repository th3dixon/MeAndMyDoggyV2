namespace MeAndMyDog.API.Models.DTOs.Friends;

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