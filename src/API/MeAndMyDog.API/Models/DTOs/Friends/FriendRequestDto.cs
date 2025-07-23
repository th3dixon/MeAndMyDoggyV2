namespace MeAndMyDog.API.Models.DTOs.Friends
{
    /// <summary>
    /// DTO for friend request information
    /// </summary>
    public class FriendRequestDto
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Requester's user ID
        /// </summary>
        public string RequesterId { get; set; } = string.Empty;

        /// <summary>
        /// Receiver's user ID
        /// </summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>
        /// Requester's information
        /// </summary>
        public UserBasicInfoDto Requester { get; set; } = new();

        /// <summary>
        /// Optional note with the request
        /// </summary>
        public string? RequestNote { get; set; }

        /// <summary>
        /// Request status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// When the request was made
        /// </summary>
        public DateTime RequestedAt { get; set; }
    }
}