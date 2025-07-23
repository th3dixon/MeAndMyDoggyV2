using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth
{
    /// <summary>
    /// DTO for disabling 2FA
    /// </summary>
    public class Disable2FADto
    {
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Type { get; set; } // "sms" or "app"
    }
}