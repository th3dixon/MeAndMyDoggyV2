namespace MeAndMyDog.API.Models.DTOs.Friends
{
    /// <summary>
    /// DTO for friend information
    /// </summary>
    public class FriendDto
    {
        /// <summary>
        /// Friend's user ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Friend's display name
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Friend's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Friend's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Friend's profile photo URL
        /// </summary>
        public string? ProfilePhotoUrl { get; set; }

        /// <summary>
        /// Friend's unique friend code
        /// </summary>
        public string FriendCode { get; set; } = string.Empty;

        /// <summary>
        /// Friendship status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// When the friendship was established
        /// </summary>
        public DateTime? AcceptedAt { get; set; }

        /// <summary>
        /// Last seen timestamp
        /// </summary>
        public DateTime? LastSeenAt { get; set; }

        /// <summary>
        /// Whether the friend is currently online
        /// </summary>
        public bool IsOnline { get; set; }
    }
}