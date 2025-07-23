namespace MeAndMyDog.API.Models.DTOs.Friends;

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