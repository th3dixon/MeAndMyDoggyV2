using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.UserProfile
{
    /// <summary>
    /// DTO for user profile information
    /// </summary>
    public class UserProfileDto
    {
        public string Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        [StringLength(100)]
        public string TimeZone { get; set; }
        
        [StringLength(10)]
        public string PreferredLanguage { get; set; }
        
        public string ProfilePhotoUrl { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastSeenAt { get; set; }
    }
}