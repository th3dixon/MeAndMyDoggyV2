using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth
{
    /// <summary>
    /// DTO for enabling two-factor authentication
    /// </summary>
    public class Enable2FADto
    {
        [Required]
        public string Type { get; set; } // "sms" or "app"
        
        [Phone]
        public string PhoneNumber { get; set; } // Required for SMS 2FA
        
        public string VerificationCode { get; set; } // For app-based 2FA verification
    }
}