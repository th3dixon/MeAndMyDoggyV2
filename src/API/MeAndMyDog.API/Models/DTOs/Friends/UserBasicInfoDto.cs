namespace MeAndMyDog.API.Models.DTOs.Friends
{
    /// <summary>
    /// DTO for basic user information
    /// </summary>
    public class UserBasicInfoDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Profile photo URL
        /// </summary>
        public string? ProfilePhotoUrl { get; set; }

        /// <summary>
        /// Unique friend code
        /// </summary>
        public string FriendCode { get; set; } = string.Empty;
    }
}